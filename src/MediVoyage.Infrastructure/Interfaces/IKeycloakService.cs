using CorePackages.Infrastructure.Dto;

namespace CorePackages.Infrastructure.Interfaces
{
    public interface IKeycloakService
    {
        Task<ApiResponse<object>> CreateUserAsync(CreateUserRequest request);
    }
}
