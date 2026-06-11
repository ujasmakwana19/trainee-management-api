using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.TraineeDTO;
using TraineeManagement.Api.TraineeServices;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/trainees")]
public class TraineesController : ControllerBase
{
  // Instance of the service create when the HTTP request is made
  private readonly ITraineeService _service;
  public TraineesController(ITraineeService traineeService)
  {
    _service = traineeService;
  }

  // Get all the Trainees
  // GET /api/trainees
  [Authorize]
  [HttpGet("/getall")]
  public async Task<ActionResult<IEnumerable<TraineeResponse>>> GetTrainee()
  {
    IEnumerable<TraineeResponse>? traineesVal = await _service.GetAllTraineesService();

    if (traineesVal is null)
    {
      return NotFound();
    }

    else
    {
      return Ok(traineesVal);
    }
  }

  // Get Trainee by unique ID
  // GET /api/trainees/:id
  [HttpGet("{id}")]
  public async Task<ActionResult<TraineeResponse>> GetTraineeById(long id)
  {
    TraineeResponse? traineeDto = await _service.GetTraineeResponseByIdService(id);
    if (traineeDto == null)
      return NotFound();
    return Ok(traineeDto);
  }

  // To add the Trainee
  // POST /api/trainees
  [HttpPost]
  public async Task<ActionResult<TraineeResponse>> CreateTrainee([FromBody] CreateTraineeRequest trainee)
  {
    TraineeResponse? traineeDto = await _service.CreateTraineeService(trainee);

    if (traineeDto is null)
    {
      return NotFound();
    }
    // The nameof use to give compile-time safety to the action name, so if we rename the GetTraineeById method, this will not lead to a runtime error.

    // CreatedAtAction is used to return a 201 Created response, along with a Location header that points to the newly created resource. The first parameter is the name of the action to which the client can make a GET request to retrieve the created resource, the second parameter is an anonymous object that contains the route values (in this case, the id of the created trainee), and the third parameter is the created trainee object itself.
    return CreatedAtAction(nameof(GetTraineeById), new { id = traineeDto.Id }, traineeDto);
  }

  // To Update the Trainee
  // PUT /api/Trainee/:id
  [HttpPut("{id}")]
  public async Task<ActionResult<TraineeResponse>> UpdateTrainee(long id, [FromBody] UpdateTraineeRequest trainee)
  {
    TraineeResponse? traineeDto = await _service.UpdateTraineeService(id, trainee);
    if (traineeDto is null)
    {
      return NotFound();
    }
    return CreatedAtAction(nameof(GetTraineeById), new { id = traineeDto.Id }, traineeDto);
  }

  // To Delete the Trainee
  // DELETE /api/Trainee/:is
  [HttpDelete("{id}")]
  public async Task<ActionResult<TraineeResponse>> DeleteTrainee(long id)
  {
    if (!await _service.DeleteTraineeService(id))
    {
      return NotFound();
    }
    else
    {
      return NoContent();
    }
  }

  // To search the substring in FirstName, LastName, TechStack, Email
  // GET api/Trainee?search=value
  [HttpGet]
  public async Task<ActionResult<TraineeResponse>> GetSearch([FromQuery] String? search)
  {
    if (search == null)
    {
      return NotFound(new { Message = "No Trainees Found" });
    }
    IEnumerable<TraineeResponse>? traineeDto = await _service.SearchTraineeService(search);
    if (traineeDto == null)
      return NotFound(new { Message = $"No Trainees Found with the search :{search}" });
    return Ok(traineeDto);
  }

}


