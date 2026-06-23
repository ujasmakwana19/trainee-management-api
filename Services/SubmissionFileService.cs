using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.ErrorCodesUtils;
using TraineeManagement.Api.ExceptionUtils;
using TraineeManagement.Api.FileServices;
using TraineeManagement.Api.SubmissionFileModel;

namespace TraineeManagement.Api.SubmissionFileService;

public class SubmissionFileService : ISubmissionFileService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionFileService> _logger;

    public SubmissionFileService(AppDbContext context, ILogger<SubmissionFileService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> IsSubmissionExists(long submissionId)
    {
        return await _context.Submissions.AnyAsync(s => s.Id == submissionId);
    }


    public async Task<long> SaveMetadataAsync(
        long submissionId, long uploadedByUserId, SavedFileResult savedFile, CancellationToken cancellationToken)
    {
        SubmissionFile file = new SubmissionFile
        {
            SubmissionId = submissionId,
            OriginalFileName = savedFile.OriginalFileName,
            StorageName = savedFile.StorageName,
            ContentType = savedFile.ContentType,
            SizeBytes = savedFile.SizeInBytes,
            Checksum = savedFile.Checksum,
            UploadedByUserId = uploadedByUserId
        };

        _context.SubmissionFiles.Add(file);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Saved submission file metadata. SubmissionId={SubmissionId}, FileId={FileId}, Size={Size}",
            submissionId, file.Id, file.SizeBytes);
        

        return file.Id;
    }

    public async Task<SubmissionFile> GetByIdAsync(long fileId)
    {
        SubmissionFile? metadata = await _context.SubmissionFiles.FirstOrDefaultAsync(f => f.Id == fileId);
        if (metadata is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_FILE);
        }

        return metadata;
    }

    public async Task DeleteMetadataAsync(long fileId, CancellationToken cancellationToken)
    {
        SubmissionFile? entity = await _context.SubmissionFiles.FirstOrDefaultAsync(f => f.Id == fileId, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_FILE);
        }

        _context.SubmissionFiles.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}