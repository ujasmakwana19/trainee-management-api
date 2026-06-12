using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.UserDTO;
using TraineeManagement.Api.UserServices;
using TraineeManagement.Api.ExceptionUtils;
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
    public async Task<ActionResult<LoginUserResponse>> LoginUser([FromBody] LoginUserRequest user)
    {
        _logger.LogInformation($"User {user.Username} Hit the Login Route");

        LoginUserResponse u = await _service.Login(user);

        _logger.LogInformation($"User {user.Username} Logged in successfully\t");
        return Ok(u);
    }
}