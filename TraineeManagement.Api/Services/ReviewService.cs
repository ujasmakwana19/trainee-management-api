using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.CacheServices;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ExceptionUtils;
using TraineeManagement.Data.ReviewModel;
using TraineeManagement.Data.TrackTaskModel;
using TraineeManagement.WebCommons.AuthClaims;
using TraineeManagement.Data.UserModel;
using TraineeManagement.Data.SubmissionModel;

namespace TraineeManagement.Api.ReviewService;

public class ReviewService : IReviewService
{   
    private readonly AppDbContext _context;
    private readonly ILogger<ReviewService> _logger;
    private readonly ICurrentUserAccessor _currentUser;
    private readonly ICacheService _cache;

    public ReviewService(AppDbContext context, ICurrentUserAccessor currentUser, ILogger<ReviewService> logger, ICacheService cache)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
        _cache = cache;
    }

    private IQueryable<Review> GetAccessibleReviews(long userId, string role)
    {
        IQueryable<Review> query = _context.Reviews.AsNoTracking();

        return role switch
        {
            nameof(UserRole.Admin) => query,

            nameof(UserRole.Mentor) => query.Where(r => r.Mentor.UserId == userId),

            nameof(UserRole.Trainee) => query.Where(r => r.Submission.TrackTask.Trainee.UserId == userId),

            _ => query.Where(_ => false)
        };
    }

    public async Task<ReviewPostResponse> CreateReview(ReviewRequestBody body)
    {   
        long userId = _currentUser.Id;
        string role = _currentUser.Role;

        Submission? submission = await _context.Submissions
            .Include(s => s.TrackTask)
                .ThenInclude(t => t.Mentor)
            .FirstOrDefaultAsync(s => s.Id == body.SubmissionId);

        if (submission is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_REVIEW);
        }

        bool isAssignedMentor = role == nameof(UserRole.Mentor) && 
                    submission.TrackTask.Mentor.UserId == userId && 
                    submission.TrackTask.MentorId == body.MentorId;

        bool isAdmin = role == nameof(UserRole.Admin);

        if (!isAssignedMentor && !isAdmin)
        {
            throw new UnauthorizedException(ErrorCodes.NOT_OWNER_ACCESS);
        }

        Review review = new Review
        {
            SubmissionId = body.SubmissionId,
            MentorId = body.MentorId,
            Feedback = body.Feedback,
            Score = body.Score,
            ReviewStatus = body.ReviewStatus,
            ReviewedDate = body.ReviewedDate
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Review {ReviewId} created successfully", review.Id);
        await _cache.RemoveAsync(CacheKey.reviewAll);

        return new ReviewPostResponse(
            review.Id,
            review.SubmissionId,
            review.MentorId,
            review.Score,
            review.Feedback,
            review.ReviewStatus,
            review.ReviewedDate
        );
    }

    public async Task<ReviewResponse> GetById(long id)
    {
        long userId = _currentUser.Id;
        string role = _currentUser.Role;

        ReviewResponse? review = await GetAccessibleReviews(userId, role)
            .Where(r => r.Id == id)
            .Select(r => new ReviewResponse(
                r.Id,
                r.SubmissionId,
                r.Submission.TrackTask.LearningTask.Title,
                r.Submission.SubmissionUrl,
                r.MentorId,
                r.Mentor.FirstName + " " + r.Mentor.LastName,
                r.Feedback,
                r.Score,
                r.ReviewStatus,
                r.ReviewedDate
            ))
            .FirstOrDefaultAsync();

        if (review is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_REVIEW);
        }

        return review;
    }

    public async Task<IEnumerable<ReviewResponse>> GetAll()
    {
        long userId = _currentUser.Id;
        string role = _currentUser.Role;
        
        return await GetAccessibleReviews(userId, role)
            .Select(r => new ReviewResponse(
                r.Id,
                r.SubmissionId,
                r.Submission.TrackTask.LearningTask.Title,
                r.Submission.SubmissionUrl,
                r.MentorId,
                r.Mentor.FirstName + " " + r.Mentor.LastName,
                r.Feedback,
                r.Score,
                r.ReviewStatus,
                r.ReviewedDate
            ))
            .ToListAsync();
    }
}