using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ExceptionUtils;

namespace TraineeManagement.Api.FileServices;

public record SavedFileResult(string StorageName, string OriginalFileName, long SizeInBytes, string Checksum , string ContentType);

public class LocalStorageFileService : IFileStorageService
{
    private readonly string _localStoragePath;
    private readonly int _bufferSize;
    private readonly long _maxAllowedSize;


    private static readonly Dictionary<string, byte[][]> AllowedSignatures = new()
    {
        [".pdf"]  = new[] { new byte[] { 0x25, 0x50, 0x44, 0x46 } },                 
        [".png"]  = new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
        [".jpg"]  = new[]
        {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xDB }
        },
        [".zip"]  = new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } },
        [".docx"] = new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } },                 
        // .txt has no real signature
    };

    private static readonly Dictionary<string, string> ExtensionToContentType = new()
    {
        [".pdf"]  = "application/pdf",
        [".png"]  = "image/png",
        [".jpg"]  = "image/jpeg",
        [".zip"]  = "application/zip",
        [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        [".txt"]  = "text/plain",
    };

    private static string GetContentType(string extension) =>
        ExtensionToContentType.TryGetValue(extension, out string? contentType)
            ? contentType
            : "application/octet-stream"; 

    private const int HeaderPeekSize = 8;

    public LocalStorageFileService(IConfiguration config)
    {
        _localStoragePath = config["StorageSettings:StorageRoot"]
            ?? throw new ServerCredentialException(ErrorCodes.INVALID_CREDENTIALS);
        _bufferSize = int.Parse(config["StorageSettings:Buffer_Size"]
            ?? throw new ServerCredentialException(ErrorCodes.INVALID_CREDENTIALS));
        _maxAllowedSize = long.Parse(config["StorageSettings:Max_Buffer_Size"]
            ?? throw new ServerCredentialException(ErrorCodes.INVALID_CREDENTIALS));

        if (!Directory.Exists(_localStoragePath))
        {
            Directory.CreateDirectory(_localStoragePath);
        }
    }

    private static string GetUniqFileName(string extension)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        return $"{timestamp}_{Guid.NewGuid()}{extension}";   
    }

    public async Task<SavedFileResult> SaveAsync(Stream content, string boundary, CancellationToken cancellationToken)
    {
        MultipartReader reader = new MultipartReader(boundary, content);
        MultipartSection? section;
        SavedFileResult? result = null;
        string? savedTargetPath = null;

        if (!Directory.Exists(_localStoragePath))
        {
            Directory.CreateDirectory(_localStoragePath);
        }
        // Read the multipart sections and process the file section
        while ((section = await reader.ReadNextSectionAsync(cancellationToken)) != null)
        {
            ContentDispositionHeaderValue? contentDisposition = section.GetContentDispositionHeader();
            
            // Currently we are only use the files frm the form data , 
            //  we can futher implement if we wanted to get the key - value paired data
            if (contentDisposition == null || !contentDisposition.IsFileDisposition())
            {
                continue;
            }

            // Currently only one file upload
            if (result != null)
            {
                if (savedTargetPath != null && File.Exists(savedTargetPath))
                {
                    File.Delete(savedTargetPath);
                }
                throw new BadRequestException(ErrorCodes.INVALID_FILE);
            }

            string? rawFileName = HeaderUtilities.RemoveQuotes(contentDisposition.FileName).Value;
            string originalFileName = rawFileName ?? string.Empty;
            string fileExtension = Path.GetExtension(originalFileName).ToLowerInvariant();

            // Extension allow-list. Must happen before we touch any bytes.
            bool isAllowedExtension = AllowedSignatures.ContainsKey(fileExtension) || fileExtension == ".txt";
            if (string.IsNullOrEmpty(fileExtension) || !isAllowedExtension)
            {
                throw new BadRequestException(ErrorCodes.INVALID_FILE);
            }

            string storageName = GetUniqFileName(fileExtension);
            string targetPath = Path.Combine(_localStoragePath, storageName);
            try
            {
                (long  totalBytes, string checksum) = await WriteSectionToDiskAsync(
                    section.Body, targetPath, fileExtension, cancellationToken);

                if (totalBytes == 0)
                {
                    throw new BadRequestException(ErrorCodes.INVALID_FILE);
                }

                string contentType = GetContentType(fileExtension);
                result = new SavedFileResult(storageName, originalFileName, totalBytes, checksum, contentType);
                savedTargetPath = targetPath;
            }
            catch
            {
                // Any failure after this point (validation, size limit, disconnect) —
                // don't leave a partial/invalid file sitting on disk.
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                throw;
            }
        }
        
        if(result is null)
        {
            throw new BadRequestException(ErrorCodes.INVALID_FILE);
        }
        return result;
    }

    private async Task<(long TotalBytes, string Checksum)> WriteSectionToDiskAsync(
        Stream sectionBody, string targetPath, string extension, CancellationToken cancellationToken)
    {
        // Reading the initial bytes
        byte[] headerBuffer = new byte[HeaderPeekSize];
        int headerBytesRead = await ReadExactAsync(sectionBody, headerBuffer, cancellationToken);

        // Verify the magic string signature
        if (AllowedSignatures.TryGetValue(extension, out byte[][]? signatures))
        {
            bool matches = signatures.Any(sig =>
                headerBytesRead >= sig.Length && headerBuffer.Take(sig.Length).SequenceEqual(sig));

            if (!matches)
            {
                throw new BadRequestException(ErrorCodes.INVALID_FILE);
            }
        }
        else if (extension == ".txt")
        {
            if (headerBuffer.Take(headerBytesRead).Any(b => b == 0x00))
            {
                throw new BadRequestException(ErrorCodes.INVALID_FILE);
            }
        }

        // File Stream init
        using FileStream targetStream = new FileStream(
            targetPath, 
            FileMode.Create, 
            FileAccess.Write, 
            FileShare.None,
            bufferSize: _bufferSize, 
            useAsync: true);

        // Hash init 
        using SHA256 sha256 = SHA256.Create();

        // Hashs the intial bytes used for the magic string
        sha256.TransformBlock(headerBuffer, 0, headerBytesRead, null, 0);
        
        // Streams the initial bytes to save to the localstorage
        await targetStream.WriteAsync(headerBuffer, 0, headerBytesRead, cancellationToken);
        
        long totalBytes = headerBytesRead;

        byte[] buffer = new byte[_bufferSize];
        int bytesRead;

        // Read the rest of the stream in chunks, updating the checksum and writing to disk
        while ((bytesRead = await sectionBody.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            totalBytes += bytesRead;

            
            if (totalBytes > _maxAllowedSize)
            {
                throw new BadRequestException(ErrorCodes.CONTENT_TOO_LARGE);
            }

            sha256.TransformBlock(buffer, 0, bytesRead, null, 0);
            await targetStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
        }
        

        sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        string checksum = Convert.ToHexString(sha256.Hash!);

        return (totalBytes, checksum);
    }

    private static async Task<int> ReadExactAsync(Stream stream, byte[] buffer, CancellationToken cancellationToken)
    {
        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer, totalRead, buffer.Length - totalRead, cancellationToken);
            if (read == 0) break; 
            totalRead += read;
        }
        return totalRead;
    }

    public Task<Stream> OpenReadAsync(string storageName, CancellationToken cancellationToken)
    {
        string fullPath = Path.Combine(_localStoragePath, storageName);
        
        return Task.FromResult<Stream>(File.OpenRead(fullPath));
    }

    public Task<bool> ExistsAsync(string storageName, CancellationToken cancellationToken)
    {
        return Task.FromResult(File.Exists(Path.Combine(_localStoragePath, storageName)));
    }

    public Task DeleteAsync(string storageName, CancellationToken cancellationToken)
    {
        string filePath = Path.Combine(_localStoragePath, storageName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }
}