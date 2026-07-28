using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Data.UserDTO;
using TraineeManagement.Api.UserServices;
using TraineeManagement.WebCommons.ResponseHandlerUtil;
using TraineeManagement.WebCommons.ErrorCodesUtils;
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

    // POST api/trainee/
    [HttpPost]
    public async Task<ActionResult> LoginUser([FromBody] LoginUserRequest userInfo)
    {
        if (!ModelState.IsValid)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest,
                ErrorCodes.INVALID_MODEL);
        }

        LoginUserResponse user = await _service.Login(userInfo);

        _logger.LogInformation($"User Logged in successfully\t");
        
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            user
        );
    }
}