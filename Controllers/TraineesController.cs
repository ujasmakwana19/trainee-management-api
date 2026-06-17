using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.TraineeDTO;
using TraineeManagement.Api.TraineeServices;
using TraineeManagement.Api.ExceptionUtils;
using Microsoft.AspNetCore.Http.HttpResults;
using TraineeManagement.Api.ResponseHandlerUtil;
using TraineeManagement.Api.ErrorCodesUtils;

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
  [HttpGet("getall")]
  public async Task<ActionResult<IEnumerable<TraineeResponse>>> GetTrainee()
  {
    IEnumerable<TraineeResponse> traineesVal = await _service.GetAllTraineesService();
    return Ok(traineesVal);
  }

  // Get Trainee by unique ID
  // GET /api/trainees/:id
  [HttpGet("{id}")]
  public async Task<ActionResult<TraineeResponse>> GetTraineeById(long id)
  {
    
    TraineeResponse traineeDto = await _service.GetTraineeResponseByIdService(id);
    // return ResponseHandler.CreateResponse(HttpContext, StatusCodes.Status400BadRequest,  ErrorCodes.INVALID_MODEL, ["aa","error","chhe"]);
    return ResponseHandler.SuccessResponse(HttpContext , traineeDto ,ErrorCodes.SUCCESS);
  }

  // To add the Trainee
  // POST /api/trainees
  [HttpPost]
  public async Task<ActionResult<TraineeResponse>> CreateTrainee([FromBody] CreateTraineeRequest trainee)
  {
    System.Console.WriteLine("Ram");
    if (!ModelState.IsValid)
    {
      return Ok(new {message = "Nathi valid"});
    }
    TraineeResponse traineeDto = await _service.CreateTraineeService(trainee);

    // The nameof use to give compile-time safety to the action name, so if we rename the GetTraineeById method, this will not lead to a runtime error.

    // CreatedAtAction is used to return a 201 Created response, along with a Location header that points to the newly created resource. The first parameter is the name of the action to which the client can make a GET request to retrieve the created resource, the second parameter is an anonymous object that contains the route values (in this case, the id of the created trainee), and the third parameter is the created trainee object itself.
    return CreatedAtAction(nameof(GetTraineeById), new { id = traineeDto.Id }, traineeDto);
  }

  // To Update the Trainee
  // PUT /api/Trainee/:id
  [HttpPut("{id}")]
  public async Task<ActionResult<TraineeResponse>> UpdateTrainee(long id, [FromBody] UpdateTraineeRequest trainee)
  {
    TraineeResponse traineeDto = await _service.UpdateTraineeService(id, trainee);
    return CreatedAtAction(nameof(GetTraineeById), new { id = traineeDto.Id }, traineeDto);
  }

  // To Delete the Trainee
  // DELETE /api/Trainee/:is
  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteTrainee(long id)
  {
    await _service.DeleteTraineeService(id);
    return NoContent();
  }

  // To search the substring in FirstName, LastName, TechStack, Email
  // GET api/Trainee?search=value
  [HttpGet]
  public async Task<ActionResult<TraineeResponse>> GetSearch([FromQuery] String? search)
  {
    if (search == null)
    {
      throw new BadRequestException("Please provide a search query parameter");
    }
    IEnumerable<TraineeResponse>? traineeDto = await _service.SearchTraineeService(search);
    return Ok(traineeDto);
  }

  // To search the substring in FirstName, LastName, TechStack, Email
  // GET /api/trainees?pageNumber=1&pageSize=10&search=amit&status=Active
  [HttpGet("getSearch")]
  public async Task<ActionResult<TraineeInfoPagination>> GetSearchPagination([FromQuery] int pageNumber,int pageSize, String search, String status)
  {
    if (search == null || status == null)
    {
      throw new BadRequestException("Please provide a all search query parameter");
    }

    TraineeInfoPagination traineeDto = await _service.SearchTraineePaginationService(pageNumber, pageSize, search, status);
    return Ok(traineeDto);
  }

}


