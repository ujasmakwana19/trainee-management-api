using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.TraineeDTO;
using TraineeManagement.Api.TraineeServices;
using TraineeManagement.Api.ExceptionUtils;
using Microsoft.AspNetCore.Http.HttpResults;
using TraineeManagement.Api.ResponseHandlerUtil;
using TraineeManagement.Api.ErrorCodesUtils;
using TraineeManagement.Api.TraineeModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TraineeManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/trainees")]
public class TraineesController : ControllerBase
{
  // Instance of the service create when the HTTP request is made
  private readonly ITraineeService _service;
  private readonly ILogger<TraineesController> _logger;
  public TraineesController(ITraineeService traineeService, ILogger<TraineesController> logger)
  {
    _service = traineeService;
    _logger = logger;
  }

  // Get all the Trainees
  // GET /api/trainees
  // [HttpGet("getall")]
  // public async Task<ActionResult> GetTrainee()
  // {
  //   IEnumerable<TraineeResponse> traineesVal = await _service.GetAllTraineesService();
  //   return ResponseHandler.SuccessResponse(HttpContext, ErrorCodes.SUCCESS, traineesVal);
  // }

  // Get Trainee by unique ID
  // GET /api/trainees/:id
  [HttpGet("{id}")]
  public async Task<ActionResult> GetTraineeById(long id)
  {
    if (!ModelState.IsValid || id < 1)
    {
      return ResponseHandler.CreateResponse(
        StatusCodes.Status400BadRequest,
        ErrorCodes.INVALID_PARAMS_QUERY);
    }
    
    TraineeResponse traineeDto = await _service.GetTraineeResponseByIdService(id);
    return ResponseHandler.SuccessResponse(HttpContext, ErrorCodes.SUCCESS, traineeDto);
  }

  // To add the Trainee
  // POST /api/trainees
  // [HttpPost]
  // public async Task<ActionResult<TraineeResponse>> CreateTrainee([FromBody] CreateTraineeRequest trainee)
  // {

  //   if (!ModelState.IsValid)
  //   {
  //     return ResponseHandler.CreateResponse(
  //               StatusCodes.Status400BadRequest,
  //               ErrorCodes.INVALID_MODEL);
  //   }
    
  //   TraineeResponse traineeDto = await _service.CreateTraineeService(trainee);

  //   // The nameof use to give compile-time safety to the action name, so if we rename the GetTraineeById method, this will not lead to a runtime error.
  //   return ResponseHandler.SuccessResponse(HttpContext,ErrorCodes.SUCCESS,traineeDto);
  // }

  // PUT /api/trainees/:id
  // [HttpPut("{id}")]
  // public async Task<ActionResult<TraineeResponse>> UpdateTrainee(long id, [FromBody] UpdateTraineeRequest trainee)
  // {
  //   if (!ModelState.IsValid)
  //     {
  //       return ResponseHandler.CreateResponse(
  //                 StatusCodes.Status400BadRequest,
  //                 ErrorCodes.INVALID_MODEL);
  //     }
  //   if(id < 1)
  //   {
  //     return ResponseHandler.CreateResponse(
  //       StatusCodes.Status400BadRequest,
  //       ErrorCodes.INVALID_PARAMS_QUERY
  //     ); 
  //   }
  //   TraineeResponse traineeDto = await _service.UpdateTraineeService(id, trainee);
  //   return ResponseHandler.SuccessResponse(HttpContext,ErrorCodes.SUCCESS,traineeDto);
  // }

  // To Delete the Trainee
  // DELETE /api/trainees/:id
  // [HttpDelete("{id}")]
  // public async Task<ActionResult> DeleteTrainee(long id)
  // {
  //     if (!ModelState.IsValid || id < 1)
  //     {
  //       return ResponseHandler.CreateResponse(
  //                 StatusCodes.Status400BadRequest,
  //                 ErrorCodes.INVALID_PARAMS_QUERY);
  //     } 
  //   await _service.DeleteTraineeService(id);
  //   return NoContent();
  // }

  // // To search the substring in FirstName, LastName, TechStack, Email
  // // GET api/Trainee?search=value
  // [HttpGet]
  // public async Task<ActionResult<TraineeResponse>> GetSearch([FromQuery] string search)
  // {
  //   if (search == null)
  //   {
  //     throw new BadRequestException(ErrorCodes.INVALID_PARAMS_QUERY);
  //   }
  //   IEnumerable<TraineeResponse> traineeDto = await _service.SearchTraineeService(search);
  //   return ResponseHandler.SuccessResponse(HttpContext,ErrorCodes.SUCCESS,traineeDto);
  // }

  // // To search the substring in FirstName, LastName, TechStack, Email
  // // GET /api/trainees?pageNumber=1&pageSize=10&search=amit&status=Active
  // [HttpGet("getSearch")]
  // public async Task<ActionResult<TraineeInfoPagination>> GetSearchPagination([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string search, [FromQuery] StatusValue status)
  // {
  //   if (!ModelState.IsValid || pageNumber < 1 || pageSize < 1)
  //   {
  //     return ResponseHandler.CreateResponse(
  //       StatusCodes.Status400BadRequest,
  //       ErrorCodes.INVALID_PARAMS_QUERY);
  //   }
  
  //   TraineeInfoPagination traineeDto = await _service.SearchTraineePaginationService(pageNumber, pageSize, search, status);
  //   return ResponseHandler.SuccessResponse(HttpContext,ErrorCodes.SUCCESS,traineeDto);
  // }

}


