using System.IO.Pipelines;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace TraineeManagement.Api.FileServices
{
    public class FileManagerService
    {
        private int BufferSize = 16777216; 
        private string UploadFilePath;
        private readonly ILogger<FileManagerService> _logger;
        private readonly IConfiguration _config;

        public FileManagerService(ILogger<FileManagerService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            BufferSize = int.Parse(_config["StorageSettings:Buffer_Size"] ?? "16777216");
            UploadFilePath = _config["StorageSettings:StorageRoot"] ?? "Uploadss";
        }

        public async Task<string> SaveViaMultipartReaderAsync(
            string boundary, 
            Stream contentStream, 
            CancellationToken cancellationToken)
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), UploadFilePath);
            // CheckAndRemoveLocalFile(targetFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string targetFilePath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), UploadFilePath),"ram");


            using FileStream outputFileStream = new FileStream(
                path: targetFilePath,
                mode: FileMode.Create,
                access: FileAccess.Write,
                share: FileShare.None,
                bufferSize: BufferSize,
                useAsync: true);

            var reader = new MultipartReader(boundary, contentStream);
            MultipartSection? section;
            long totalBytesRead = 0;

            // Process each section in the multipart body
            while ((section = await reader.ReadNextSectionAsync(cancellationToken)) != null)
            {
                // Check if the section is a file
                var contentDisposition = section.GetContentDispositionHeader();
                if (contentDisposition != null && contentDisposition.IsFileDisposition())
                {
                    _logger.LogInformation($"Processing file: {contentDisposition.FileName.Value}");

                    // Write the file content to the target file
                    await section.Body.CopyToAsync(outputFileStream, cancellationToken);
                    totalBytesRead += section.Body.Length;
                }
                else if (contentDisposition != null && contentDisposition.IsFormDisposition())
                {
                    // Handle metadata (form fields)
                    string key = contentDisposition.Name.Value!;
                    using var streamReader = new StreamReader(section.Body);
                    string value = await streamReader.ReadToEndAsync(cancellationToken);
                    _logger.LogInformation($"Received metadata: {key} = {value}");
                }
            }

            _logger.LogInformation($"File upload completed (via multipart). Total bytes read: {totalBytesRead} bytes.");
            return targetFilePath;
        }

        private void CheckAndRemoveLocalFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogDebug($"Removed existing output file: {filePath}");
            }
        }

        // public async Task<string?> SaveViaPipeReaderAsync(PipeReader contentReader, CancellationToken cancellationToken)
        // {
        //     string targetFilePath = Path.Combine(Directory.GetCurrentDirectory(), UploadFilePath);
        //     CheckAndRemoveLocalFile(targetFilePath);
        //     long totalBytesRead = 0;

        //     using FileStream outputFileStream = new FileStream(
        //         path: targetFilePath,
        //         mode: FileMode.OpenOrCreate,
        //         access: FileAccess.Write,
        //         share: FileShare.None,
        //         bufferSize: BufferSize,
        //         useAsync: true);

        //     while (true)
        //     {
        //         var readResult = await contentReader.ReadAsync();
        //         var buffer = readResult.Buffer;

        //         foreach (var memory in buffer)
        //         {
        //             await outputFileStream.WriteAsync(memory);
        //             totalBytesRead += memory.Length;
        //         }
        //         contentReader.AdvanceTo(buffer.End);

        //         if (readResult.IsCompleted)
        //         {
        //             break;
        //         }
        //     }

        //     _logger.LogInformation($"File upload completed (via pipeReader). Total bytes read: {totalBytesRead} bytes.");
        //     return targetFilePath;
        // }
    }
}