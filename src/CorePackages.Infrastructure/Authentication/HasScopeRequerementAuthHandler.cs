using Microsoft.AspNetCore.Authorization;

namespace CorePackages.Infrastructure.Authentication
{
    public class HasScopeRequerementAuthHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "scope"))
            {
                var scopeClaim = context.User.FindFirst(c => c.Type == "scope");
                var scopes = scopeClaim.Value.Split(' ');

                if (scopes.Contains(requirement.Scope))
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
