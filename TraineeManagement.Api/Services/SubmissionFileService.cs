using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ExceptionUtils;
using TraineeManagement.Api.FileServices;
using TraineeManagement.Data.SubmissionFileModel;
using TraineeManagement.Data.ProcessingJobModel;
using TraineeManagement.Messaging;

namespace TraineeManagement.Api.SubmissionFileService;

public class SubmissionFileService : ISubmissionFileService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionFileService> _logger;
    private readonly IEventPublisher _eventPublisher;

    public SubmissionFileService(AppDbContext context, ILogger<SubmissionFileService> logger, IEventPublisher eventPublisher)
    {
        _context = context;
        _logger = logger;
        _eventPublisher = eventPublisher;
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
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Saved submission file metadata. SubmissionId={SubmissionId}, FileId={FileId}, Size={Size}",
            submissionId, file.Id, file.SizeBytes);
        
        ProcessingJob message = new ProcessingJob
        {
            MessageId = Guid.NewGuid(),
            CoorelationId = Guid.NewGuid(),
            SubmissionId = file.Id
        };
        _context.ProcessingJobs.Add(message);
        await _context.SaveChangesAsync();
        try
        {
            _logger.LogInformation(" coorelationId:{CoorelationId} - The Job to process check sum is queuing", message.CoorelationId);
            await _eventPublisher.PublishAsync<ProcessingJob>(message, message.CoorelationId.ToString() , QueueConfig.SubmissionRouting);
        }
        catch (Exception)
        {
            // throw new QueuingOperationExeception(ErrorCodes.QUEUING_OPERATION_FAILED);
            message.Status = ProcessingJobStatus.Failed;
            message.ErrorSummary = "Failed to publish message to RabbitMQ";
            await _context.SaveChangesAsync();
            _logger.LogInformation(" coorelationId:{CoorelationId} - The Queuing Operation failed", message.CoorelationId);
            
        }

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