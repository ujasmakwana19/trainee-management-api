using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using TraineeManagement.WebCommons.ErrorCodesUtils;
using TraineeManagement.WebCommons.ExceptionUtils;

//  It handles the policy handling where when it breaks, 
//  To respond to the user
namespace TraineeManagement.WebCommons.AuthorizationHandler;
public class RoleAuthorizationHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _default = new AuthorizationMiddlewareResultHandler();

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
            throw new UnauthorizedException(ErrorCodes.INVALID_TOKEN);
        
        await _default.HandleAsync(next, context, policy, authorizeResult);
    }
}