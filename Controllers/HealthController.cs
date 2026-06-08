using Microsoft.AspNetCore.Mvc;
namespace TraineeManagement.Api.Controllers;
[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
  {
    [HttpGet]
    public ActionResult GetMessage()
    {
        var healthParameter = new { 
          status = "running", 
          application = "Trainee Management API",  
          // To get the timestamp in the ISO8601 format
          timestamp = DateTime.Now.ToString("s")
        };
        return Ok(healthParameter); 
    }

    
}
