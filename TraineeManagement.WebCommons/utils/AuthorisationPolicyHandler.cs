using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ExceptionUtils;
using Microsoft.IdentityModel.Tokens;

//  It handles the policy handling where when it breaks, 
//  To respond to the user
namespace TraineeManagement.WebCommons.AuthorizationHandler;
public class RoleAuthorizationHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _default = new AuthorizationMiddlewareResultHandler();

    private static bool IsExpiredMessage(object failure)
    {
        return failure?.ToString()?.Contains("expired", StringComparison.OrdinalIgnoreCase) ?? false;
    }
    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult
    )
    {
        
        if (authorizeResult.Forbidden)
            throw new ForbiddenException(ErrorCodes.ROLE_FORBIDDEN);

        if (authorizeResult.Challenged)
        {
            Exception? authFailure = context.Items["JwtBearerException"] as Exception;
            
            if(authFailure is null)
            {
                throw new UnauthorizedException(ErrorCodes.TOKEN_FORBIDDEN);
            }
            
            if (authFailure is SecurityTokenExpiredException || IsExpiredMessage(authFailure))
            {
                throw new UnauthorizedException(ErrorCodes.TOKEN_EXPIRED);
            }
            
            Console.WriteLine("Ujassssss");
            throw new UnauthorizedException(ErrorCodes.TOKEN_FORBIDDEN);
        }
        
        await _default.HandleAsync(next, context, policy, authorizeResult);
    }
}