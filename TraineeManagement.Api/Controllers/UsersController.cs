using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Data.UserDTO;
using TraineeManagement.Api.UserServices;
using TraineeManagement.WebCommons.ResponseHandlerUtil;
using TraineeManagement.WebCommons.ErrorCodesUtils;
namespace TraineeManagement.Api.UserController;

[ApiController]
[Route("api/auth")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _service = userService;
        _logger = logger;
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult> LoginUser([FromBody] LoginUserRequest userInfo)
    {
        if (!ModelState.IsValid)
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status400BadRequest,
                ErrorCodes.INVALID_MODEL);
        }

        LoginResult user = await _service.Login(userInfo);

        _logger.LogInformation($"User Logged in successfully\t");
        string refreshToken = user.RefreshToken;

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        });

        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            user.Response
        );
    }

    // POST api/auth/refresh
    [HttpPost("refresh")]
    public async Task<ActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) 
            || string.IsNullOrEmpty(refreshToken))
        {
            return ResponseHandler.CreateResponse(
                StatusCodes.Status401Unauthorized,
                ErrorCodes.SESSION_EXPIRED);
        }
        Console.WriteLine(refreshToken);
        LoginUserResponse user = await _service.GetToken(refreshToken);

        
        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            user
        );
    }

    // POST api/auth/logout
    [HttpPost("logout")]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("refreshToken");

        return ResponseHandler.SuccessResponse(
            HttpContext,
            ErrorCodes.SUCCESS,
            new object {}
        );
    }
}