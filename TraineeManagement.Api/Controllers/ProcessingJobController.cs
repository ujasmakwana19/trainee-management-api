using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ResponseHandlerUtil;
using TraineeManagement.Data.ProcessingJobModel;
namespace TraineeManagement.Api.ProcessingControllers;

[Authorize]
[ApiController]
[Route("api/processing-jobs")]
public class ProcessingController : ControllerBase
{
  private readonly AppDbContext _context;
  public ProcessingController(AppDbContext context)
  {
    _context = context;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult> ProcessingJobs(long id)
  {
    if(!ModelState.IsValid || id < 1)
    {
      return ResponseHandler.CreateResponse(
        StatusCodes.Status400BadRequest,
        ErrorCodes.INVALID_PARAMS_QUERY
      );
    }

    ProcessingJob? p = await _context.ProcessingJobs.FirstOrDefaultAsync(t => t.Id == id);
    if(p is null)
    {
      return ResponseHandler.CreateResponse(
        StatusCodes.Status400BadRequest,
        ErrorCodes.NOT_FOUND_QUEUEMESSAGE
      );
    }
    return ResponseHandler.SuccessResponse(HttpContext,ErrorCodes.SUCCESS,p);
  }
}
