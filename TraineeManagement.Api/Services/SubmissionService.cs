using Microsoft.EntityFrameworkCore;
using TraineeManagement.Messaging;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.ErrorCodesUtils;
using TraineeManagement.Api.ExceptionUtils;
using TraineeManagement.Api.SubmissionDTO;
using TraineeManagement.Api.SubmissionModel;
using TraineeManagement.Contracts.Events;

namespace TraineeManagement.Api.SubmissionService;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionService> _logger;
    private readonly IEventPublisher _publishService;

    public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger, IEventPublisher publishService)
    {
        _context = context;
        _logger = logger;
        _publishService = publishService;
    }

    private async Task<Submission> FetchSubmission(long id)
    {
        Submission? s = await _context.Submissions.FirstOrDefaultAsync(t => t.Id == id);

        if(s is null)
            throw new NotFoundException(ErrorCodes.NOT_FOUND_SUBMISSION);

        return s;
    }

    private SubmissionResponse ToResponse(Submission task)
    {
        return new SubmissionResponse(
            task.Id,
            task.TaskAssignmentId,
            task.SubmissionUrl,
            task.Notes,
            task.SubmittedDate,
            task.Status
        );
    }

    public async Task<SubmissionResponse> CreateSubmission(SubmissionRequestBody body)
    {
        Submission s = new Submission
        {
            TaskAssignmentId = body.TaskAssignmentId,
            SubmissionUrl = body.SubmissionUrl,
            Notes = body.Notes,
            SubmittedDate = body.SubmittedDate,
            Status = body.Status   
        };
        _context.Submissions.Add(s);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Submission {SubmissionId} created successfully", s.Id);

        try
        {
            SubmissionProcessingRequested sr = new SubmissionProcessingRequested(
                MessageId: Guid.NewGuid().ToString(),
                CorrelationId: Guid.NewGuid().ToString(), 
                TaskAssignmentId: s.TaskAssignmentId,
                RequestedAt: DateTime.UtcNow,
                ContractVersion: "1.0"
            );
            
            await _publishService.PublishAsync<SubmissionProcessingRequested>(sr, routingKey: "submission.requested");
            _logger.LogInformation("Published processing request event for TaskAssignmentId {Id}", s.TaskAssignmentId);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to RabbitMQ for Submission {SubmissionId}", s.Id);
            throw;
        }
        return ToResponse(s);
    }

    public async Task<SubmissionResponse> GetSubmissionById(long id)
    {
        SubmissionResponse? s = await _context.Submissions
                                        .Where(t => t.Id == id)
                                        .Select(t => new SubmissionResponse(
                                            t.Id,
                                            t.TaskAssignmentId,
                                            t.SubmissionUrl,
                                            t.Notes,
                                            t.SubmittedDate,
                                            t.Status
                                        ))
                                        .FirstOrDefaultAsync();
        if(s is null)
            throw new NotFoundException(ErrorCodes.NOT_FOUND_SUBMISSION);
        return s;
    }

    public async Task<IEnumerable<SubmissionResponse>> GetAll()
    {
        List<SubmissionResponse> submissions = await _context.Submissions
                                        .Select(t => new SubmissionResponse(
                                            t.Id,
                                            t.TaskAssignmentId,
                                            t.SubmissionUrl,
                                            t.Notes,
                                            t.SubmittedDate,
                                            t.Status
                                        ))
                                        .ToListAsync();

        return submissions;
    }
}