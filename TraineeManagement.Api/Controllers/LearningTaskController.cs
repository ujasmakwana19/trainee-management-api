using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.LearningTaskServices;
using TraineeManagement.Data.TaskDTO;
using TraineeManagement.WebCommons.ResponseHandlerUtil;
using TraineeManagement.WebCommons.ErrorCodesUtils;

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
    [HttpGet("getall")]
    public async Task<ActionResult> GetAllTasks()
    {
        IEnumerable<TaskResponseData> tasks = await _service.GetAll();
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            tasks
        );
    }

    // /api/learning-tasks/:id
    [HttpGet("{id}")]
    public async Task<ActionResult> GetTaskById(long id)
    {
        if (!ModelState.IsValid || id < 1)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest,
                ErrorCodes.INVALID_PARAMS_QUERY);
        }
        TaskResponseData task = await _service.GetById(id);
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            task
        );
    }

    [HttpPost]
    public async Task<ActionResult> CreateTaskRequest([FromBody] TaskRequestBody taskInfo)
    {
        if (!ModelState.IsValid)
        {
            return ResponseHandler.CreateResponse(
                        StatusCodes.Status400BadRequest,
                        ErrorCodes.INVALID_MODEL);
        }

        TaskResponseData taskData = await _service.CreateTask(taskInfo);
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            taskData
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTaskRequest(long id, [FromBody] TaskRequestBody taskInfo)
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
                        ErrorCodes.INVALID_PARAMS_QUERY);
        }
        TaskResponseData taskData = await _service.UpdateTask(id,taskInfo);
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            taskData
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTaskRequest(long id)
    {
        if (!ModelState.IsValid || id < 1)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest,
                ErrorCodes.INVALID_PARAMS_QUERY);
        }
        await _service.DeleteTask(id);
        return NoContent();
    }

}