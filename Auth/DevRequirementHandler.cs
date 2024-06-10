using Microsoft.AspNetCore.Authorization;

namespace MultiAccountMVP.Auth;

public class DevRequirementHandler : AuthorizationHandler<DevRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DevRequirement requirement)
    {
        //this method never calls
        
        if (context.User.Claims.Any(claim => claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod" && claim.Value == CustomAuthSchemes.CookieDevAccount))
        {
            context.Succeed(requirement);
        }
        context.Fail(new AuthorizationFailureReason(this, $"AuthenticationType != {CustomAuthSchemes.CookieDevAccount}"));
        return Task.CompletedTask;
    }
}