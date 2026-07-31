using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ExceptionUtils;
using TraineeManagement.Api.FileServices;
using TraineeManagement.WebCommons.ResponseHandlerUtil;
using TraineeManagement.Data.SubmissionFileModel;
using TraineeManagement.Api.SubmissionFileService;
using TraineeManagement.Api.FileAttributeCustom;

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
        // To get the user details from the jwt , instead of taking explicitly
        string? userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
        {
            throw new UnauthorizedException(ErrorCodes.TOKEN_FORBIDDEN);
        }
        return userId;
    }

    [DisableFormValueModelBinding]
    [HttpPost("{id}/files")]
    public async Task<IActionResult> SaveSubmissionFile(long id)
    {
        // check if the provided id is valid format and type
        if (!ModelState.IsValid || id < 1)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest, 
                ErrorCodes.INVALID_PARAMS_QUERY
            );
        }

        // check the content type
        if (!Request.ContentType?.StartsWith("multipart/form-data") ?? true)
        {
            return ResponseHandler.CreateResponse(
                    StatusCodes.Status400BadRequest, 
                    ErrorCodes.INVALID_MODEL
                );
        }
        
        // parse the boundary , so that further stream can understand where to start reading
        string? boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary).Value;
        if (string.IsNullOrWhiteSpace(boundary))
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest, 
                ErrorCodes.INVALID_MODEL
            );
        }

        // check if the submission reference to this file exists
        if (!await _submissionFileService.IsSubmissionExists(id))
        {
            
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest, 
                ErrorCodes.REFERENCE_NOT_EXISTS
            );
        }

        long currentUserId = GetCurrentUserId();

        CancellationToken cancellationToken = HttpContext.RequestAborted;
        
        // Reads the file and save to the disk
        SavedFileResult savedFile = await _fileStorageService.SaveAsync(Request.Body, boundary, cancellationToken);

        long fileId;
        try
        {
            // save the meta data to the submission file metadata  
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
        if (!ModelState.IsValid || id < 1)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest, 
                ErrorCodes.INVALID_PARAMS_QUERY
            );
        }
        // To check if the submission file metadata exists
        SubmissionFile metadata = await _submissionFileService.GetByIdAsync(id);
        
        // Check Only access able by --::> Not Clearly mention to handle it Role based
        // or not , and also there was not any thing else regarding the Register User
        // long currentUserId = GetCurrentUserId();
        // bool isOwner = metadata.UploadedByUserId == currentUserId;
        
        // if (!isOwner)
        // {
        //     return ResponseHandler.CreateResponse(StatusCodes.Status401Unauthorized, ErrorCodes.UNAUTHORISE_ACCESS);
        // }

        CancellationToken cancellationToken = HttpContext.RequestAborted;

        // check the exists of the file on the disk
        if (!await _fileStorageService.ExistsAsync(metadata.StorageName, cancellationToken))
        {
            _logger.LogWarning("Metadata exists but physical file missing. FileId={FileId}", id);
            return ResponseHandler.CreateResponse(StatusCodes.Status404NotFound, ErrorCodes.NOT_FOUND_FILE);
        }

        // Openes the file stream to send to the client
        Stream fileStream = await _fileStorageService.OpenReadAsync(metadata.StorageName, cancellationToken);

        // Content-Disposition: attachment forces a download instead of inline rendering.
        return File(fileStream, metadata.ContentType, metadata.OriginalFileName);
    }

    [HttpDelete("/api/submission-files/{id}")]
    public async Task<IActionResult> DeleteSubmissionFile(long id)
    {
        if (!ModelState.IsValid || id < 1)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest, 
                ErrorCodes.INVALID_PARAMS_QUERY
            );
        }
        SubmissionFile metadata = await _submissionFileService.GetByIdAsync(id);

        // Only the User who own can delete it
        long currentUserId = GetCurrentUserId();
        if (metadata.UploadedByUserId != currentUserId)
        {
            return ResponseHandler.CreateResponse(StatusCodes.Status401Unauthorized, ErrorCodes.UNAUTHORISE_ACCESS);
        }

        CancellationToken cancellationToken = HttpContext.RequestAborted;
        if(await _submissionFileService.CheckIfReferenceExists(id, metadata.Checksum, cancellationToken))
        {
            await _fileStorageService.DeleteAsync(metadata.StorageName, cancellationToken);
        }
        // Delete the file metadata in the MySQL
        await _submissionFileService.DeleteMetadataAsync(id, cancellationToken);


        return NoContent();
    }
}