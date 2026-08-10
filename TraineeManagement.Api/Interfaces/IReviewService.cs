namespace TraineeManagement.Api.ReviewService;
public interface IReviewService
{
    Task<ReviewPostResponse> CreateReview(ReviewRequestBody body);
    Task<ReviewResponse> GetById(long Id);

    Task<IEnumerable<ReviewResponse>> GetAll();
}