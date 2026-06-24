namespace TraineeManagement.Api.ReviewService;
public interface IReviewService
{
    Task<ReviewResponse> CreateReview(ReviewRequestBody body);
    Task<ReviewResponse> GetById(long Id);

    Task<IEnumerable<ReviewResponse>> GetAll();
}