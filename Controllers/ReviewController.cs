using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    public async Task<ActionResult<ReviewResponse>> CreateTrackTask([FromBody] ReviewRequestBody body)
    {
        ReviewResponse review = await _service.CreateReview(body);
        return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponse>> GetReviewById(long id)
    {
        return Ok(await _service.GetById(id));
    }

    [HttpGet("getall")]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetAllTasks()
    {
        return Ok(await _service.GetAll());
    }
}