using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.UserDTO;
using TraineeManagement.Api.UserServices;

namespace TraineeManagement.Api.UserController;

[ApiController]
[Route("api/auth/login")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService userService)
    {
        _service = userService;
    }

    [HttpPost]
    public async Task<ActionResult<LoginUserResponse>> LoginUser([FromBody] LoginUserRequest user)
    {
        LoginUserResponse? u = await _service.Login(user);

        if (u is null)
        {
            return Unauthorized();
        }
        return Ok(u);
    }
}