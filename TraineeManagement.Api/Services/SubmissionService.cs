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

    private IQueryable<Submission> GetAccessibleSubmissions(long userId, string role)
    {
        IQueryable<Submission> query = _context.Submissions.AsNoTracking();

        return role switch
        {
            nameof(UserRole.Admin) => query,

            nameof(UserRole.Mentor) => query.Where(s =>
                s.TrackTask.Mentor.UserId == userId),

            nameof(UserRole.Trainee) => query.Where(s =>
                s.TrackTask.Trainee.UserId == userId),

            _ => query.Where(_ => false)
        };
    }

    private async Task<Submission> FetchSubmission(long id)
    {
        Submission? s = await _context.Submissions.FirstOrDefaultAsync(t => t.Id == id);

        if(s is null)
            throw new NotFoundException(ErrorCodes.NOT_FOUND_SUBMISSION);

        return s;
    }

    private static SubmissionCreateResponse ToResponse(Submission task)
    {
        return new SubmissionCreateResponse(
            task.Id,
            task.TaskAssignmentId,
            task.SubmissionUrl,
            task.Notes,
            task.SubmittedDate,
            task.Status
        );
    }

    public async Task<SubmissionCreateResponse> CreateSubmission(SubmissionRequestBody body)
    {
        long userId = _currentUser.Id;
        string role = _currentUser.Role;

        bool hasAccess = role switch
        {
            nameof(UserRole.Admin) => true,

            nameof(UserRole.Trainee) => await _context.TrackTasks
                .AnyAsync(t =>
                    t.Id == body.TaskAssignmentId &&
                    t.Trainee.UserId == userId),

            nameof(UserRole.Mentor) => await _context.TrackTasks
                .AnyAsync(t =>
                    t.Id == body.TaskAssignmentId &&
                    t.Mentor.UserId == userId),

            _ => false
        };

        if (!hasAccess)
            throw new UnauthorizedException(ErrorCodes.ROLE_FORBIDDEN);

        Submission submission = new Submission
        {
            TaskAssignmentId = body.TaskAssignmentId,
            SubmissionUrl = body.SubmissionUrl,
            Notes = body.Notes,
            SubmittedDate = body.SubmittedDate,
            Status = body.Status
        };

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Submission {SubmissionId} created successfully",
            submission.Id);

        return ToResponse(submission);
    }

    public async Task<SubmissionResponse> GetSubmissionById(long id)
    {
        long userId = _currentUser.Id;
        string role = _currentUser.Role;

        string cacheKey = CacheKey.submissionId + $"{id}";

        SubmissionResponse? submission =
            await _cache.GetAsync<SubmissionResponse>(cacheKey);

        if (submission is null)
        {
            submission = await GetAccessibleSubmissions(userId, role)
                .Where(s => s.Id == id)
                .Select(s => new SubmissionResponse(
                    s.Id,
                    s.TaskAssignmentId,
                    s.TrackTask.LearningTask.Title,
                    s.SubmissionUrl,
                    s.Notes,
                    s.SubmittedDate,
                    s.Status
                ))
                .FirstOrDefaultAsync();

            if (submission is null)
                throw new NotFoundException(ErrorCodes.NOT_FOUND_SUBMISSION);

            await _cache.SetAsync(
                cacheKey,
                submission,
                CacheTTL.GETS_TTL_MIN);
        }

        return submission;
    }

    public async Task<IEnumerable<SubmissionResponse>> GetAll()
    {
        long userId = _currentUser.Id;
        string role = _currentUser.Role;
        
        return await GetAccessibleSubmissions(userId, role)
            .Select(s => new SubmissionResponse(
                s.Id,
                s.TaskAssignmentId,
                s.TrackTask.LearningTask.Title,
                s.SubmissionUrl,
                s.Notes,
                s.SubmittedDate,
                s.Status
            ))
            .ToListAsync();
    }
}