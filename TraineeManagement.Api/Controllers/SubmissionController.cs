using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.ErrorCodesUtils;
using TraineeManagement.Api.ResponseHandlerUtil;
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
    public async Task<ActionResult> CreateSubmission([FromBody] SubmissionRequestBody body)
    {
        if (!ModelState.IsValid)
        {
            return ResponseHandler.CreateResponse(
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.INVALID_MODEL);
        }
        SubmissionResponse submission = await _service.CreateSubmission(body);
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.ACCEPTED,
            submission
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(long id)
    {
        if (!ModelState.IsValid || id < 1)
        {
            return ResponseHandler.CreateResponse(
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.INVALID_PARAMS_QUERY);
        }
        SubmissionResponse submission = await _service.GetSubmissionById(id);
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            submission
        );
    }

    [HttpGet("getall")]
    public async Task<ActionResult> GetAll()
    {
        IEnumerable<SubmissionResponse> submissions = await _service.GetAll();
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            submissions
        );
    }
}