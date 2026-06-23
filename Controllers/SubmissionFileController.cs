using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using TraineeManagement.Api.ErrorCodesUtils;
using TraineeManagement.Api.ExceptionUtils;
using TraineeManagement.Api.FileServices;
using TraineeManagement.Api.ResponseHandlerUtil;
using TraineeManagement.Api.SubmissionFileModel;
using TraineeManagement.Api.SubmissionFileService;

namespace TraineeManagement.Api.SubmissionFileController;

[Authorize]
[ApiController]
[Route("/api/submissions")]
public class SubmissionFileController : ControllerBase
{
    private readonly ILogger<SubmissionFileController> _logger;
    private readonly IFileStorageService _fileStorageService;
    private readonly ISubmissionFileService _submissionFileService;

    public SubmissionFileController(
        ILogger<SubmissionFileController> logger,
        IFileStorageService fileStorageService,
        ISubmissionFileService submissionFileService)
    {
        _logger = logger;
        _fileStorageService = fileStorageService;
        _submissionFileService = submissionFileService;
    }

    private long GetCurrentUserId()
    {
        // The User here used is implict provided by the framework
        // such that to get the data of the claim
        string? userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
        {
            throw new UnauthorizedException(ErrorCodes.INVALID_TOKEN);
        }
        return userId;
    }

    [DisableFormValueModelBinding]
    [HttpPost("{id}/files")]
    public async Task<IActionResult> SaveSubmissionFile(long id)
    {
        if (!ModelState.IsValid || id < 1)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest, 
                ErrorCodes.INVALID_PARAMS_QUERY
            );
        }

        if (!Request.ContentType?.StartsWith("multipart/form-data") ?? true)
        {
            return ResponseHandler.CreateResponse(
                    StatusCodes.Status400BadRequest, 
                    ErrorCodes.INVALID_MODEL
                );
        }
        
        string? boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary).Value;
        if (string.IsNullOrWhiteSpace(boundary))
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest, 
                ErrorCodes.INVALID_MODEL
            );
        }

        if (!await _submissionFileService.IsSubmissionExists(id))
        {
            
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest, 
                ErrorCodes.REFERENCE_NOT_EXISTS
            );
        }

        long currentUserId = GetCurrentUserId();

        CancellationToken cancellationToken = HttpContext.RequestAborted;
        
        SavedFileResult savedFile = await _fileStorageService.SaveAsync(Request.Body, boundary, cancellationToken);

        long fileId;
        try
        {
            fileId = await _submissionFileService.SaveMetadataAsync(id, currentUserId, savedFile, cancellationToken);
        }
        catch
        {
            //if DB operation failed to delete the file for the disk
            await _fileStorageService.DeleteAsync(savedFile.StorageName, cancellationToken);
            throw;
        }

        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            new
            {
                id = fileId,
                originalFileName = savedFile.OriginalFileName,
                contentType = savedFile.ContentType,
                sizeInBytes = savedFile.SizeInBytes
            });
    }

    [HttpGet("/api/submission-files/{id}/download")]
    public async Task<IActionResult> DownloadSubmissionFile(long id)
    {
        SubmissionFile metadata = await _submissionFileService.GetByIdAsync(id);
        

        long currentUserId = GetCurrentUserId();
        bool isOwner = metadata.UploadedByUserId == currentUserId;
        
        if (!isOwner)
        {
            return ResponseHandler.CreateResponse(StatusCodes.Status401Unauthorized, ErrorCodes.UNAUTHORISE_ACCESS);
        }

        CancellationToken cancellationToken = HttpContext.RequestAborted;

        if (!await _fileStorageService.ExistsAsync(metadata.StorageName, cancellationToken))
        {
            _logger.LogWarning("Metadata exists but physical file missing. FileId={FileId}", id);
            return ResponseHandler.CreateResponse(StatusCodes.Status404NotFound, ErrorCodes.NOT_FOUND_FILE);
        }

        Stream fileStream = await _fileStorageService.OpenReadAsync(metadata.StorageName, cancellationToken);

        // Content-Disposition: attachment forces a download instead of inline rendering —
        // important for .txt/.docx/.zip to never get rendered in-browser from your domain.
        return File(fileStream, metadata.ContentType, metadata.OriginalFileName);
    }

    [HttpDelete("/api/submission-files/{id}")]
    public async Task<IActionResult> DeleteSubmissionFile(long id)
    {
        SubmissionFile metadata = await _submissionFileService.GetByIdAsync(id);

        long currentUserId = GetCurrentUserId();
        if (metadata.UploadedByUserId != currentUserId)
        {
            return ResponseHandler.CreateResponse(StatusCodes.Status401Unauthorized, ErrorCodes.UNAUTHORISE_ACCESS);
        }

        CancellationToken cancellationToken = HttpContext.RequestAborted;

        await _submissionFileService.DeleteMetadataAsync(id, cancellationToken);

        await _fileStorageService.DeleteAsync(metadata.StorageName, cancellationToken);

        return NoContent();
    }
}