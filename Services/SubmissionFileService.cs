using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
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
        var entity = new SubmissionFile
        {
            SubmissionId = submissionId,
            OriginalFileName = savedFile.OriginalFileName,
            StorageName = savedFile.StorageName,
            ContentType = savedFile.ContentType,
            SizeBytes = savedFile.SizeInBytes,
            Checksum = savedFile.Checksum,
            UploadedByUserId = uploadedByUserId
        };

        _context.SubmissionFiles.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Saved submission file metadata. SubmissionId={SubmissionId}, FileId={FileId}, Size={Size}",
            submissionId, entity.Id, entity.SizeBytes);
        // deliberately not logging OriginalFileName/StorageName/Checksum — no need to,
        // and avoids leaking anything filesystem-shaped into logs.

        return entity.Id;
    }

    public async Task<SubmissionFile?> GetByIdAsync(long fileId)
    {
        return await _context.SubmissionFiles
            .Include(f => f.Submission)
            .FirstOrDefaultAsync(f => f.Id == fileId);
    }

    public async Task DeleteMetadataAsync(long fileId, CancellationToken cancellationToken)
    {
        var entity = await _context.SubmissionFiles.FirstOrDefaultAsync(f => f.Id == fileId, cancellationToken);
        if (entity != null)
        {
            _context.SubmissionFiles.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}