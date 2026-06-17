using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.TrackTaskDTO;
using TraineeManagement.Api.TrackTaskService;
using TraineeManagement.Api.TraineeDTO;
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
    public async Task<ActionResult<TrackTaskResponse>> CreateTrackTask([FromBody] TrackTaskRequestBody body)
    {
        TrackTaskResponse createdTrackTask = await _trackTaskService.CreateTrackTaskAsync(body);
        return CreatedAtAction(nameof(GetTrackTaskById), new { id = createdTrackTask.Id }, createdTrackTask);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TrackTaskPopulatedResponseBody>> GetTrackTaskById(long id)
    {
        TrackTaskPopulatedResponseBody trackTask = await _trackTaskService.GetTrackTaskByIdAsync(id);
        return trackTask;
    }

    [HttpGet("getall")]
    public async Task<ActionResult<IEnumerable<TrackTaskResponse>>> GetAllTasks()
    {
        IEnumerable<TrackTaskResponse> tasks = await _trackTaskService.GetAllTasks();
        return Ok(tasks);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<TrackTaskResponse>> UpdateTrackTask(long id, [FromBody] TrackTaskUpdateRequestBody body)
    {
        TrackTaskResponse updatedTrackTask = await _trackTaskService.UpdateTrackTaskAsync(id, body);
        return Ok(updatedTrackTask);
    }
}