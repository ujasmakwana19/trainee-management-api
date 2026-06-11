using TraineeManagement.Api.UserModel;
using TraineeManagement.Api.UserDTO;
using TraineeManagement.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.JwtServices;

namespace TraineeManagement.Api.UserServices;


public class UserService : IUserService
{
    // readonly makes sure that the list of trainees cannot be reassigned to a different list, but we can still add or remove items from the list. This is useful for maintaining the integrity of the data structure while allowing for modifications to the contents of the list.
    // private readonly List<Trainee> _trainees = new();

    // This is for the inMemory Database Instance
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;
    public UserService(AppDbContext context, IJwtService jwtService, IConfiguration config)
    {
        _context = context;
        _jwtService = jwtService;
        _config = config;
    }

    private static LoginUserResponse ToResponse(String Token,User userInfo, int expiryMinutes)
    {   
        return new LoginUserResponse(
            Token,
            expiryMinutes * 60,
            new UserRecord
            (
                userInfo.Id,
                userInfo.Username,
                userInfo.Role
            )
        );
    } 

    private async Task<User?> FetchUser(String username)
    {
        User? u = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if(u is null)
        {
            return null;
        }
        return u;
    }
    private string? CreateHash(User user, String password)
    {
        PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
        String passwordHash = passwordHasher.HashPassword(user,password);
        if(passwordHash is null)
        {
            return null;
        }
        return passwordHash;
    }
    private PasswordVerificationResult VerifyPassword(User user, String hashPass,String password)
    {
        PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
        PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(user,hashPass,password);
        
        return result;
    }

    public async Task<LoginUserResponse?> Login(LoginUserRequest userInfo)
    {
        if(userInfo is null || userInfo.Username is null || userInfo.Password is null)
        {
            return null;
        }
        User? u = await FetchUser(userInfo.Username);
        if(u is null || u.PasswordHash is null)
        {
            return null;
        }

        PasswordVerificationResult result = VerifyPassword(u, u.PasswordHash,userInfo.Password); 

        if(result == PasswordVerificationResult.Success)
        {
            string token = _jwtService.GenerateToken(u);
            return ToResponse(token, u, int.Parse(_config["Jwt:ExpiryMinutes"]!));
        }
        return null;
    }
};

