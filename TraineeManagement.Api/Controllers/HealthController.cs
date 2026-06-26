using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Data.DataBaseContext;
namespace TraineeManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
  private readonly AppDbContext _context;
  public HealthController(AppDbContext context)
  {
    _context = context;
  }
  [HttpGet]
  public ActionResult GetMessage()
  {
    bool isDatabaseConnected = _context.Database.CanConnect();

    return Ok(new
    {
      status = "running",
      application = "Trainee Management API",
      // To get the timestamp in the ISO8601 format
      timestamp = DateTime.Now.ToString("s"),
      database = isDatabaseConnected ? "connected" : "not connected"
    });
  }
}
