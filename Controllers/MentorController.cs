using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.MentorDTO;
using TraineeManagement.Api.MentorServices;
namespace TraineeManagement.Api.MentorControllers;

[Authorize]
[ApiController]
[Route("api/mentors")]
public class MentorController : ControllerBase
{
    private readonly IMentorService _service;
    private readonly ILogger<MentorController> _logger;

    public MentorController(IMentorService service, ILogger<MentorController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // api/mentors/getall
    [HttpGet("/api/mentors/getall")]
    public async Task<ActionResult<IEnumerable<MentorResponse>>> GetAllMentors()
    {
        return Ok(await _service.GetAll());
    }

    // api/mentors/:id
    [HttpGet("{id}")]
    public async Task<ActionResult<MentorResponse>> GetMentorById(long id)
    {
        return Ok(await _service.GetById(id));
    }

    [HttpPost]
    public async Task<ActionResult<MentorResponse>> CreateMentorRequest([FromBody] MentorRequestBody mentorInfo)
    {
        MentorResponse mentor = await _service.CreateMentor(mentorInfo);
        return CreatedAtAction(nameof(GetMentorById), new {id = mentor.Id}, mentor);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MentorResponse>> UpdateMentorRequest(long id, [FromBody] MentorRequestBody mentorInfo)
    {
        MentorResponse mentor = await _service.UpdateMentor(id,mentorInfo);
        return CreatedAtAction(nameof(GetMentorById), new {id = mentor.Id}, mentor);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMentorRequest(long id)
    {
        await _service.DeleteMentor(id);
        return NoContent();
    }

}