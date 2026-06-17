using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.SubmissionDTO;
using TraineeManagement.Api.SubmissionService;

namespace TraineeManagement.Api.SubmissionController;

[Authorize]
[ApiController]
[Route("/api/submissions")]
public class SubmitController : ControllerBase
{
    private readonly ISubmissionService _service;
    private readonly ILogger<SubmitController> _logger;

    public SubmitController(ISubmissionService service, ILogger<SubmitController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<SubmissionResponse>> CreateSubmission([FromBody] SubmissionRequestBody body)
    {
        SubmissionResponse submission = await _service.CreateSubmission(body);
        return CreatedAtAction(nameof(GetById), new { id = submission.Id }, submission);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubmissionResponse>> GetById(long id)
    {
        return Ok(await _service.GetSubmissionById(id));
    }

    [HttpGet("getall")]
    public async Task<ActionResult<IEnumerable<SubmissionResponse>>> GetAll()
    {
        return Ok(await _service.GetAll());
    }
}