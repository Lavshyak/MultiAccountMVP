using Microsoft.AspNetCore.Authorization;

namespace MultiAccountMVP.Auth;

public class AccountRequirementHandler : AuthorizationHandler<AccountRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountRequirement requirement)
    {
        if (!context.User.Claims.Any(claim => claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod" && claim.Value == CustomAuthSchemes.CookieDevAccount))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        context.Fail(new AuthorizationFailureReason(this, $"AuthenticationType == {CustomAuthSchemes.CookieDevAccount}"));
        return Task.CompletedTask;
    }
}