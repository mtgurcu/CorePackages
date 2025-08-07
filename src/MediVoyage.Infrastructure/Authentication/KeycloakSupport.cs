using Microsoft.AspNetCore.Authorization;

namespace CorePackages.Infrastructure.Authentication
{
    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public readonly string Scope;

        public HasScopeRequirement(string scope)
        {
            Scope = scope;
        }
    }
}
