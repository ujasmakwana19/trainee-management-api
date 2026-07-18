using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ResponseHandlerUtil;
using TraineeManagement.Api.ReviewService;
namespace TraineeManagement.Api.ReviewControllers;

[Authorize]
[ApiController]
[Route("api/reviews")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _service;
    private readonly ILogger<ReviewController> _logger;

    public ReviewController(IReviewService service, ILogger<ReviewController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [Authorize(Policy = "MentorOrAdminOnly")]
    [HttpPost]
    public async Task<ActionResult> CreateReviewController([FromBody] ReviewRequestBody body)
    {
        if (!ModelState.IsValid)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest,
                ErrorCodes.INVALID_MODEL
            );
        }
        ReviewResponse review = await _service.CreateReview(body);
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            review
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponse>> GetReviewById(long id)
    {
        if (!ModelState.IsValid || id < 1)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest,
                ErrorCodes.INVALID_MODEL
            );
        }
        ReviewResponse review = await _service.GetById(id);
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            review
        );
    }

    [HttpGet("getall")]
    public async Task<ActionResult> GetAllReviews()
    {
        IEnumerable<ReviewResponse> reviews = await _service.GetAll();
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            reviews
        );
    }
}