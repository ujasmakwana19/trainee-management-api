using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.UserDTO;
using TraineeManagement.Api.UserServices;
using TraineeManagement.Api.ExceptionUtils;
using TraineeManagement.Api.ResponseHandlerUtil;
using TraineeManagement.Api.ErrorCodesUtils;
namespace TraineeManagement.Api.UserController;

[ApiController]
[Route("api/auth/login")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _service = userService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> LoginUser([FromBody] LoginUserRequest user)
    {
        if (!ModelState.IsValid)
        {
            return ResponseHandler.CreateResponse(HttpContext,StatusCodes.Status400BadRequest,ErrorCodes.INVALID_MODEL,UtilityHelper.GetInvalidModelStateErrors(ModelState));
        }
        _logger.LogInformation($"User {user.Username} Hit the Login Route");

        LoginUserResponse u = await _service.Login(user);

        _logger.LogInformation($"User {user.Username} Logged in successfully\t");
        return Ok(u);
    }
}