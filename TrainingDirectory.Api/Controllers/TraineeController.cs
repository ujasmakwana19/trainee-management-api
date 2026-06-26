using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Contracts.ErrorCodesUtils;
using TraineeManagement.Contracts.ResponseHandlerUtil;
using TraineeManagement.Data.TraineeDTO;
using TrainingDirectory.TraineeInterface;

namespace TrainingDirectory.TraineeControllers;

[ApiController]
[Route("api/trainees")]
public class TraineeController : ControllerBase
{
    private readonly ITraineeService _service;
    private readonly ILogger<TraineeController> _logger;
    public TraineeController(ITraineeService traineeService, ILogger<TraineeController> logger)
    {
        _service = traineeService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetTraineeById(long id)
    {
        _logger.LogInformation("I am here");
        if (!ModelState.IsValid || id < 1)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest,
                ErrorCodes.INVALID_PARAMS_QUERY);
        }

        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            await _service.GetById(id)
        );
    }
}