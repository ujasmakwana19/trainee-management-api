using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.TaskModel;
using TraineeManagement.Api.LearningTaskServices;
using TraineeManagement.Api.TaskDTO;

namespace TraineeManagement.Api.LearningTaskControllers;

[Authorize]
[ApiController]
[Route("api/learning-tasks")]
public class LearningTaskController : ControllerBase
{
    private readonly ILearningTaskService _service;
    private readonly ILogger<LearningTaskController> _logger;

    public LearningTaskController(ILearningTaskService service, ILogger<LearningTaskController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // /api/learning-tasks/getall
    [HttpGet("/api/learning-tasks/getall")]
    public async Task<ActionResult<IEnumerable<TaskResponseData>>> GetAllTasks()
    {
        return Ok(await _service.GetAll());
    }

    // /api/learning-tasks/:id
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponseData>> GetTaskById(long id)
    {
        return Ok(await _service.GetById(id));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseData>> CreateTaskRequest([FromBody] TaskRequestBody taskInfo)
    {
        TaskResponseData taskData = await _service.CreateTask(taskInfo);
        return CreatedAtAction(nameof(GetTaskById), new {id = taskData.Id}, taskData);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskResponseData>> UpdateTaskRequest(long id, [FromBody] TaskRequestBody taskInfo)
    {
        TaskResponseData taskData = await _service.UpdateTask(id,taskInfo);
        return CreatedAtAction(nameof(GetTaskById), new {id = taskData.Id}, taskData);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTaskRequest(long id)
    {
        await _service.DeleteTask(id);
        return NoContent();
    }

}