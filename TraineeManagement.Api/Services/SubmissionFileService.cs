using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ExceptionUtils;
using TraineeManagement.Api.FileServices;
using TraineeManagement.Data.SubmissionFileModel;
using TraineeManagement.Data.ProcessingJobModel;
using TraineeManagement.Messaging;
using TraineeManagement.WebCommons.CoorealationIdServices;
using System.Diagnostics.Metrics;

namespace TraineeManagement.Api.SubmissionFileService;

public class SubmissionFileService : ISubmissionFileService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionFileService> _logger;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;

    public SubmissionFileService(AppDbContext context, ILogger<SubmissionFileService> logger, IEventPublisher eventPublisher,
    ICorrelationIdAccessor correlationIdAccessor)
    {
        _context = context;
        _logger = logger;
        _eventPublisher = eventPublisher;
        _correlationIdAccessor = correlationIdAccessor;
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
            CoorelationId = string.IsNullOrEmpty(_correlationIdAccessor.CorrelationId) 
                ? Guid.NewGuid() 
                : Guid.Parse(_correlationIdAccessor.CorrelationId),
            SubmissionId = file.Id
        };
        _context.ProcessingJobs.Add(message);
        await _context.SaveChangesAsync();
        try
        {
            _logger.LogInformation("The Job to process check sum is queuing");
            await _eventPublisher.PublishAsync<ProcessingJob>(message, message.CoorelationId.ToString() , QueueConfig.SubmissionRouting);
        }
        catch (Exception ex)
        {
            message.Status = ProcessingJobStatus.Failed;
            message.ErrorSummary = "Failed to publish message to RabbitMQ";
            await _context.SaveChangesAsync();
            _logger.LogError(ex,"RabbitMq Exception:{ex}", ex.Message);
            // SOftdependency , Hard Dependency
            // throw new QueuingOperationExeception(ErrorCodes.QUEUING_OPERATION_FAILED);
            
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

    public async Task<bool> CheckIfReferenceExists(long fileId, string Checksum, CancellationToken cancellationToken)
    {
        int count = await _context.SubmissionFiles.CountAsync(f => f.Checksum == Checksum, cancellationToken);
        if (count > 1)
        {
            return false;
        }
        return true;
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