using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TraineeManagement.Api.UserModel;
using TraineeManagement.Api.ExceptionUtils;

namespace TraineeManagement.Api.JwtServices;

public interface IJwtService
{
    string GenerateToken(User user);
    // ClaimsPrincipal? ValidateToken(string token);
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    private readonly ILogger<JwtService> _logger ;

    public JwtService(IConfiguration config , ILogger<JwtService> logger)
    {
        _config = config;
        _key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!)
        );
        _logger = logger;
    }

    public string GenerateToken(User user)
    {
        Claim[] claims = 
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("userName", user.Username),
            new Claim("role", user.Role.ToString())
        };

        SigningCredentials credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        SecurityToken token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(_config["Jwt:ExpiryMinutes"]!)
            ),
            signingCredentials: credentials 
        );
        string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        if(jwtToken is null)
        {
            _logger.LogError("Failed to created the jwt token");
            throw new JwtOperationException();
        }
        return jwtToken;
    }

    // public ClaimsPrincipal? ValidateToken(string token)
    // {
    //     var tokenHandler = new JwtSecurityTokenHandler();

    //     try
    //     {
    //         var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
    //         {
    //             ValidateIssuer = true,
    //             ValidateAudience = true,
    //             ValidateLifetime = true,
    //             ValidateIssuerSigningKey = true,
    //             ValidIssuer = _config["Jwt:Issuer"],
    //             ValidAudience = _config["Jwt:Audience"],
    //             IssuerSigningKey = _key,
    //             ClockSkew = TimeSpan.Zero  // exact expiry, no grace period
    //         }, out _);

    //         return principal;
    //     }
    //     catch
    //     {
    //         return null;
    //     }
    // }
}