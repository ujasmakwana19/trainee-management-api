using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.CacheServices;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.ErrorCodesUtils;
using TraineeManagement.Api.ExceptionUtils;
using TraineeManagement.Api.ReviewModel;
namespace TraineeManagement.Api.ReviewService;
public class ReviewService : IReviewService
{   
    private readonly AppDbContext _context;
    private readonly ILogger<ReviewService> _logger;
    private readonly ICacheService _cache;

    public ReviewService(AppDbContext context, ILogger<ReviewService> logger, ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    private async Task<Review> FetchReview(long id)
    {
        Review? t = await _context.Reviews.FirstOrDefaultAsync(t => t.Id == id);
        if(t is null)
        {
            throw new NotFoundException(ErrorCodes.NOT_FOUND_REVIEW);
        }
        return t;
    }

    private ReviewResponse ToResponse(Review r)
    {
        return new ReviewResponse(
            r.Id,
            r.SubmissionId,
            r.MentorId,
            r.Feedback,
            r.Score,
            r.ReviewStatus,
            r.ReviewedDate
        );
    }

    public async Task<ReviewResponse> CreateReview(ReviewRequestBody body)
    {
        Review review = new Review
        {
            SubmissionId = body.SubmissionId,
            MentorId = body.MentorId,
            Feedback = body.Feedback,
            Score = body.Score,
            ReviewStatus = body.ReviewStatus,
            ReviewedDate = body.ReviewedDate
        };
        _context.Add(review);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Review {ReviewId} created successfully", review.Id);
        await _cache.RemoveAsync(CacheKey.reviewAll);
        return ToResponse(review);
    }

    public async Task<ReviewResponse> GetById(long Id)
    {

        ReviewResponse? review = await _context.Reviews
                                .Where(t => t.Id == Id)
                                .Select(t => new ReviewResponse(
                                    t.Id,
                                    t.SubmissionId,
                                    t.MentorId,
                                    t.Feedback,
                                    t.Score,
                                    t.ReviewStatus,
                                    t.ReviewedDate
                                ))
                                .FirstOrDefaultAsync();
        if(review is null)
            throw new NotFoundException(ErrorCodes.NOT_FOUND_REVIEW);
        return review;
    }

    public async Task<IEnumerable<ReviewResponse>> GetAll()
    {
        IEnumerable<ReviewResponse>? reviews = await _cache.GetAsync<IEnumerable<ReviewResponse>>(CacheKey.reviewAll);

        if(reviews is null)
        {

            reviews = await _context.Reviews
                                    .Select(t => new ReviewResponse(
                                        t.Id,
                                        t.SubmissionId,
                                        t.MentorId,
                                        t.Feedback,
                                        t.Score,
                                        t.ReviewStatus,
                                        t.ReviewedDate
                                    ))
                                    .ToListAsync();
            if(reviews.Any())
                await _cache.SetAsync<IEnumerable<ReviewResponse>>(CacheKey.reviewAll, reviews, CacheTTL.GETS_TTL_MIN);
        }
        return reviews;
    }
}