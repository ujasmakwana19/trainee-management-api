using TraineeManagement.Data.UserModel;
using TraineeManagement.Data.UserDTO;
using TraineeManagement.Data.DataBaseContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.JwtServices;
using TraineeManagement.Contracts.ExceptionUtils;
using TraineeManagement.Contracts.ErrorMessageUtils;
using TraineeManagement.Contracts.ErrorCodesUtils;
namespace TraineeManagement.Api.UserServices;


public class UserService : IUserService
{
    // readonly makes sure that the list of trainees cannot be reassigned to a different list, but we can still add or remove items from the list. This is useful for maintaining the integrity of the data structure while allowing for modifications to the contents of the list.
    // private readonly List<Trainee> _trainees = new();

    //  Database Instance
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;
    private readonly ILogger<UserService> _logger; 
    public UserService(AppDbContext context, IJwtService jwtService, IConfiguration config, ILogger<UserService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _config = config;
        _logger = logger;
    }

    private static LoginUserResponse ToResponse(String Token,User userInfo, int expiryMinutes)
    {   
        return new LoginUserResponse(
            Token,
            expiryMinutes * 60, // Convert to seconds
            new UserRecord
            (
                userInfo.Id,
                userInfo.Username,
                userInfo.Role
            )
        );
    } 

    private async Task<User> FetchUser(String username)
    {
        User? u = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if(u is null)
        {
            throw new UnauthorizedException(ErrorCodes.INVALID_CREDENTIALS);
        }
        return u;
    }
    
    private PasswordVerificationResult VerifyPassword(User user, String hashPass,String password)
    {
        PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
        PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(user,hashPass,password);
        return result;
    }

    public async Task<LoginUserResponse> Login(LoginUserRequest userInfo)
    {
        
        User user = await FetchUser(userInfo.Username!);

        if (user.PasswordHash is null)
        {
            _logger.LogDebug($"User : {user.Username} exists but does not has the hash password ");
            throw new UnauthorizedException(ErrorCodes.INVALID_CREDENTIALS);
        }

        PasswordVerificationResult result = VerifyPassword(user, user.PasswordHash,userInfo.Password!); 

        if(result == PasswordVerificationResult.Success)
        {
            string token = _jwtService.GenerateToken(user);
            
            return ToResponse(token, user, int.Parse(_config["Jwt:ExpiryMinutes"]!));
        }
        throw new UnauthorizedException(ErrorCodes.INVALID_CREDENTIALS);
        
    }
};

