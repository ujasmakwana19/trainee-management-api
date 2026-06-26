using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Contracts.ErrorCodesUtils;
using TraineeManagement.Data.MentorDTO;
using TraineeManagement.Api.MentorServices;
using TraineeManagement.Contracts.ResponseHandlerUtil;
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
    [HttpGet("getall")]
    public async Task<ActionResult> GetAllMentors()
    {
        IEnumerable<MentorResponse> mentors = await _service.GetAll();
        return ResponseHandler.SuccessResponse(HttpContext, ErrorCodes.SUCCESS, mentors);
    }

    // api/mentors/:id
    [HttpGet("{id}")]
    public async Task<ActionResult> GetMentorById(long id)
    {
        if (!ModelState.IsValid || id < 1)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest,
                ErrorCodes.INVALID_PARAMS_QUERY
            );
        }
        MentorResponse mentor = await _service.GetById(id);
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            mentor
        );
    }

    [HttpPost]
    public async Task<ActionResult<MentorResponse>> CreateMentorRequest([FromBody] MentorRequestBody mentorInfo)
    {
        if (!ModelState.IsValid)
        {
            return ResponseHandler.CreateResponse(
                        StatusCodes.Status400BadRequest,
                        ErrorCodes.INVALID_MODEL);
        }
        MentorResponse mentor = await _service.CreateMentor(mentorInfo);
        return ResponseHandler.SuccessResponse(HttpContext,ErrorCodes.SUCCESS,mentor);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MentorResponse>> UpdateMentorRequest(long id, [FromBody] MentorRequestBody mentorInfo)
    {
        if (!ModelState.IsValid)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest,
                ErrorCodes.INVALID_MODEL
            );
        }
        if(id < 1)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest,
                ErrorCodes.INVALID_PARAMS_QUERY
            );
        }
        MentorResponse mentor = await _service.UpdateMentor(id,mentorInfo);
        return ResponseHandler.SuccessResponse(HttpContext,ErrorCodes.SUCCESS,mentor);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMentorRequest(long id)
    {
        if (!ModelState.IsValid || id < 1)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest,
                ErrorCodes.INVALID_PARAMS_QUERY
            );
        }
        await _service.DeleteMentor(id);
        return NoContent();
    }

}