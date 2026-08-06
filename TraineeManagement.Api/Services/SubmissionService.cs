using Microsoft.EntityFrameworkCore;
using TraineeManagement.Messaging;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ExceptionUtils;
using TraineeManagement.Data.SubmissionDTO;
using TraineeManagement.Data.SubmissionModel;
using TraineeManagement.Data.CacheServices;
using TraineeManagement.WebCommons.AuthClaims;
using TraineeManagement.Data.UserModel;
using TraineeManagement.Data.MentorModel;
using TraineeManagement.Data.TrackTaskModel;

namespace TraineeManagement.Api.SubmissionService;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionService> _logger;

    private readonly ICurrentUserAccessor _currentUser;
    private readonly ICacheService _cache;
    private readonly IEventPublisher _publishService;

    public SubmissionService(AppDbContext context, ICurrentUserAccessor currentUser , ILogger<SubmissionService> logger, ICacheService cache, IEventPublisher publishService)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
        _cache = cache;
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
        long userId = _currentUser.Id;
        string userRole = _currentUser.Role;

        if(userRole == UserRole.Mentor.ToString())
        {
            bool taskExists = await _context.TrackTasks.AnyAsync(t => t.Id == body.TaskAssignmentId && t.MentorId == userId);

            if (!taskExists)
            {
                throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK_ASSIGNMENT);
            }

        }
        else if (userRole == UserRole.Trainee.ToString())
        {
            bool taskExists = await _context.TrackTasks.AnyAsync(t => t.Id == body.TaskAssignmentId && t.TraineeId == userId);

            if (!taskExists)
            {
                throw new NotFoundException(ErrorCodes.NOT_FOUND_TASK_ASSIGNMENT);
            }
        }
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
        return ToResponse(s);
    }

    public async Task<SubmissionResponse> GetSubmissionById(long id)
    {
        string cacheKey = CacheKey.submissionId + $"{id}";
        SubmissionResponse? s = await _cache.GetAsync<SubmissionResponse>(cacheKey);

        if(s is null)
        {
            s = await _context.Submissions
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

            await _cache.SetAsync<SubmissionResponse>(cacheKey, s, CacheTTL.GETS_TTL_MIN);
        }
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