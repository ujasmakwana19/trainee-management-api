using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using TraineeManagement.Api.UserDTO;
using TraineeManagement.Api.UserServices;

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

        if(user is null || user.Username is null || user.Password is null)
        {
            return BadRequest(new {Message = $"Please provide the username and password"});
        }

        _logger.LogInformation($"User {user.Username} Hit the Login Route");

        LoginUserResponse? u = await _service.Login(user);

        if (u is null)
        {
            return Unauthorized(new {Message = $"Invalid Credintials or User not Found, Try Forgot Password"} );
        }

        _logger.LogInformation($"User {user.Username} Logged in successfully\t");
        return Ok(u);
    }
}