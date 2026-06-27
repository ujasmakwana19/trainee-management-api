using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Contracts.ErrorCodesUtils;
using TraineeManagement.Contracts.ResponseHandlerUtil;
using TraineeManagement.Data.TrackTaskDTO;
using TraineeManagement.Api.TrackTaskService;
namespace TraineeManagement.Api.TrackTaskController;

[Authorize]
[ApiController]
[Route("api/task-assignments")]
public class TrackTaskController : ControllerBase
{
    private readonly ITrackTaskService _trackTaskService;
    private readonly ILogger<TrackTaskController> _logger;

    public TrackTaskController(ITrackTaskService trackTaskService, ILogger<TrackTaskController> logger)
    {
        _trackTaskService = trackTaskService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> CreateTrackTask([FromBody] TrackTaskRequestBody body)
    {
        if (!ModelState.IsValid )
        {
        return ResponseHandler.CreateResponse(
            StatusCodes.Status400BadRequest,
            ErrorCodes.INVALID_MODEL);
        }
        TrackTaskResponse createdTrackTask = await _trackTaskService.CreateTrackTaskAsync(body);
        return ResponseHandler.SuccessResponse(HttpContext, ErrorCodes.SUCCESS, createdTrackTask);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetTrackTaskById(long id)
    {
        if (!ModelState.IsValid || id < 1)
        {
        return ResponseHandler.CreateResponse(
            StatusCodes.Status400BadRequest,
            ErrorCodes.INVALID_PARAMS_QUERY);
        }
        TrackTaskPopulatedResponseBody trackTask = await _trackTaskService.GetTrackTaskByIdAsync(id);
        return ResponseHandler.SuccessResponse(HttpContext, ErrorCodes.SUCCESS, trackTask);
    }

    [HttpGet("getall")]
    public async Task<ActionResult> GetAllTasks()
    {
        IEnumerable<TrackTaskResponse> tasks = await _trackTaskService.GetAllTasks();
        return ResponseHandler.SuccessResponse(HttpContext, ErrorCodes.SUCCESS, tasks);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<TrackTaskResponse>> UpdateTrackTask(long id, [FromBody] TrackTaskUpdateRequestBody body)
    {
        if (!ModelState.IsValid)
        {
            return ResponseHandler.CreateResponse(
                    StatusCodes.Status400BadRequest,
                    ErrorCodes.INVALID_MODEL);
        }
        if(id < 1)
        {
        return ResponseHandler.CreateResponse(
            StatusCodes.Status400BadRequest,
            ErrorCodes.INVALID_PARAMS_QUERY
        ); 
        }
        TrackTaskResponse updatedTrackTask = await _trackTaskService.UpdateTrackTaskAsync(id, body);
        return ResponseHandler.SuccessResponse(HttpContext, ErrorCodes.SUCCESS, updatedTrackTask);
    }
}