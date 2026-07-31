using TraineeManagement.WebCommons.ErrorCodesUtils;
using Microsoft.AspNetCore.Http;
using TraineeManagement.WebCommons.ExceptionUtils;

// To access the claims in the JWT token in the Http request and the services
namespace TraineeManagement.WebCommons.AuthClaims;
public interface ICurrentUserAccessor
{
    long Id {get;}
    string Role {get;}
}

public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long Id {
        get
        {
            
            string? claim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
            
            if (string.IsNullOrWhiteSpace(claim) || !long.TryParse(claim, out long userId))
                throw new UnauthorizedException(ErrorCodes.TOKEN_FORBIDDEN);
            return userId;
            
        }
    }

    public string Role {
        get {            
            string? claim = _httpContextAccessor.HttpContext?.User?.FindFirst("userRole")?.Value;
            if (string.IsNullOrWhiteSpace(claim))
                throw new UnauthorizedException(ErrorCodes.TOKEN_FORBIDDEN);
            return claim;
        }
    } 
}