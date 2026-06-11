using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace TraineeManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
  [HttpGet]
  public ActionResult GetMessage()
  {

    return Ok(new
    {
      status = "running",
      application = "Trainee Management API",
      // To get the timestamp in the ISO8601 format
      timestamp = DateTime.Now.ToString("s")
    });
  }
}
