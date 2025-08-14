using CorePackages.Infrastructure.Dto;
using CorePackages.Infrastructure.Dto.Exceptions;
using CorePackages.Infrastructure.Dto.Keycloak;
using CorePackages.Infrastructure.Extentions;
using CorePackages.Infrastructure.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CorePackages.Infrastructure
{
    public class KeycloakService : IKeycloakService
    {
        private readonly KeycloakConfig _keycloakConfig;
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cacheService;

        public KeycloakService(
            IConfigurationHelper<KeycloakConfig> keycloakConfig,
            HttpClient httpClient,
            ICacheService cacheService)
        {
            _keycloakConfig = keycloakConfig.GetConfigurationAsync("KeycloakConfig").GetAwaiter().GetResult();
            _httpClient = httpClient;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<object>> CreateUserAsync(User request, RoleRepresantationRequest roleRepresantationRequest = null)
        {
            var user = await GetUserByUsername(request.username);

            if (user != null)
                await UpdateKeycloakUser(request, user.Id);
            else
                await CreateKeycloakUser(request);

            if (roleRepresantationRequest != null)
                await UpdateUserRoleAsync(roleRepresantationRequest);

            return new ApiResponse<object>(true, "");
        }
        public async Task<UserResponse> UpdateUserRoleAsync(RoleRepresantationRequest model)
        {
            var user = await GetUserByUsername(model.Username);
            if (user == null)
                throw new BusinessException(Errors.User.UserNotExist);

            var url = $"/admin/realms/{_keycloakConfig.RealmName}/users/{user.Id}/role-mappings/realm";

            var rolesToAdd = new List<RoleRepresantationResponse>();

            foreach (var role in model.RolesToAdd)
            {
                var roleRepresentation = await GetRoleRepresentationAsync(role);
                rolesToAdd.Add(roleRepresentation);
            }

            if (rolesToAdd.Any())
                await _httpClient.SendAsync(HttpMethod.Post, url, rolesToAdd);

            var rolesToDelete = new List<RoleRepresantationResponse>();

            foreach (var role in model.RolesToRemove)
            {
                var roleRepresentation = await GetRoleRepresentationAsync(role);
                rolesToDelete.Add(roleRepresentation);
            }

            if (rolesToDelete.Any())
                await _httpClient.SendAsync(HttpMethod.Delete, url, rolesToDelete);

            return user;
        }
        #region Internal Methods
        private async Task CreateKeycloakUser(User request)
        {
            var url = $"/admin/realms/{_keycloakConfig.RealmName}/users";
            await _httpClient.SendAsync(HttpMethod.Post, url, request);
        }
        private async Task UpdateKeycloakUser(User request, string id)
        {
            var url = $"/admin/realms/{_keycloakConfig.RealmName}/users/{id}";
            await _httpClient.SendAsync(HttpMethod.Put, url, request);
        }
        private async Task<UserResponse?> GetUserByUsername(string userName)
        {
            var token = await GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"/admin/realms/{_keycloakConfig.RealmName}/users?username={userName}";
            var users = await _httpClient.SendAsync<List<UserResponse>>(HttpMethod.Get, url);

            return users?.FirstOrDefault();
        }
        private async Task<string> GetTokenAsync()
        {
            var cachedLoginResponse = _cacheService.Get<CachedToken>("KEYCLOAK_API_TOKEN");

            if (!IsTokenExpired(cachedLoginResponse))
            {
                return cachedLoginResponse.accessToken;
            }

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"/realms/{_keycloakConfig.RealmName}/protocol/openid-connect/token")
            {
                Content = new FormUrlEncodedContent(new[]
                          {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id",_keycloakConfig.ClientId),
                new KeyValuePair<string, string>("client_secret",_keycloakConfig.ClientSecret)
            })
            };

            var url = $"/realms/{_keycloakConfig.RealmName}/protocol/openid-connect/token";

            var response = await _httpClient.SendAsync<TokenResponse>(HttpMethod.Post, url, tokenRequest);

            var token = response?.access_token;

            return token ?? "";
        }
        private async Task<RoleRepresantationResponse> GetRoleRepresentationAsync(string roleName)
        {
            var url = $"/admin/realms/{_keycloakConfig.RealmName}/roles/{roleName}";
            return await _httpClient.SendAsync<RoleRepresantationResponse>(HttpMethod.Get, url);
        }
        private static bool IsTokenExpired(CachedToken cachedToken)
        {
            if (cachedToken == null)
            {
                return true;
            }
            var buffer = TimeSpan.FromMinutes(1);

            return cachedToken.ExpireDate <= DateTime.Now.Add(buffer);
        }
        #endregion
    }
}
