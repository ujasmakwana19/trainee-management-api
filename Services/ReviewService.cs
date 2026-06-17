using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.ExceptionUtils;
using TraineeManagement.Api.ReviewModel;
namespace TraineeManagement.Api.ReviewService;
public class ReviewService : IReviewService
{   
    private readonly AppDbContext _context;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(AppDbContext context, ILogger<ReviewService> logger)
    {
        _context = context;
        _logger = logger;
    }

    private async Task<Review> FetchReview(long id)
    {
        Review? t = await _context.Reviews.FirstOrDefaultAsync(t => t.Id == id);
        if(t is null)
        {
            throw new NotFoundException("Review Not Found");
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
        return ToResponse(review);
    }

    public async Task<ReviewResponse> GetById(long Id)
    {
        ReviewResponse? review = await _context.Reviews
                                .Select(t => new ReviewResponse(
                                    t.Id,
                                    t.SubmissionId,
                                    t.MentorId,
                                    t.Feedback,
                                    t.Score,
                                    t.ReviewStatus,
                                    t.ReviewedDate
                                ))
                                .FirstOrDefaultAsync(t => t.Id == Id);
        if(review is null)
            throw new NotFoundException("Review Not Found");
        return review;
    }

    public async Task<IEnumerable<ReviewResponse>> GetAll()
    {
        IEnumerable<ReviewResponse> reviews = await _context.Reviews
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
        return reviews;
    }
}