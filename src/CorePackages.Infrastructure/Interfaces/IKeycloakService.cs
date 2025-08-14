using CorePackages.Infrastructure.Dto;
using CorePackages.Infrastructure.Dto.Keycloak;

namespace CorePackages.Infrastructure.Interfaces
{
    public interface IKeycloakService
    {
        Task<ApiResponse<object>> CreateUserAsync(User request, RoleRepresantationRequest model = null);
        Task<UserResponse> UpdateUserRoleAsync(RoleRepresantationRequest model);
    }
}
