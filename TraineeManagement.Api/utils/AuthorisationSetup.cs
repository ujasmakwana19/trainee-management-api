using Microsoft.AspNetCore.Authorization;
using TraineeManagement.Data.UserModel;
using TraineeManagement.WebCommons.AuthorizationHandler;

public static class AuthorisationSetup
{
    public static IServiceCollection AddRoleAuthorisation(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("userRole", UserRole.Admin.ToString()));

            options.AddPolicy("MentorOrAdminOnly", policy =>
                policy.RequireClaim("userRole", UserRole.Mentor.ToString() , UserRole.Admin.ToString()));

        });

        services.AddSingleton<IAuthorizationMiddlewareResultHandler, RoleAuthorizationHandler>();

        return services;
    }
}