using CorePackages.Infrastructure.Dto;
using CorePackages.Infrastructure.Dto.Exceptions;
using CorePackages.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CorePackages.Infrastructure
{
    public class KeycloakService : IKeycloakService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfigurationHelper<KeycloakConfig> _keycloakConfig;
        private readonly ILogger<KeycloakService> _logger;
        private readonly HttpClient _httpClient;
        public KeycloakService(IHttpClientFactory httpClientFactory, IConfigurationHelper<KeycloakConfig> keycloakConfig, ILogger<KeycloakService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _keycloakConfig = keycloakConfig;
            _logger = logger;
            _httpClient = _httpClientFactory.CreateClient("KeycloakClient");
        }

        public async Task<ApiResponse<object>> CreateUserAsync(CreateUserRequest request)
        {
            var keycloakConfig = await _keycloakConfig.GetConfigurationAsync("KeycloakConfig");

            var user = await GetUserByUsername(request, keycloakConfig);

            if (user.Any())
                await UpdateKeycloakUser(request, keycloakConfig, user.FirstOrDefault().Id);
            else
                await CreateKeycloakUser(request, keycloakConfig);

            return new ApiResponse<object>(true, "");
        }

        private async Task CreateKeycloakUser(CreateUserRequest request, KeycloakConfig keycloakConfig)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

            var url = $"/admin/realms/{keycloakConfig.RealmName}/users";

            var response = await _httpClient.PostAsync(url, jsonContent);

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                throw new BusinessException(Errors.Patient.PatientCreateError);
            }
        }

        private async Task UpdateKeycloakUser(CreateUserRequest request, KeycloakConfig keycloakConfig, string id)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

            var url = $"/admin/realms/{keycloakConfig.RealmName}/users/{id}";

            var response = await _httpClient.PutAsync(url, jsonContent);

            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw new BusinessException(Errors.Patient.PatientUpdateError);
            }
        }

        private async Task<List<KeycloakUserResponse>> GetUserByUsername(CreateUserRequest request, KeycloakConfig keycloakConfig)
        {
            var token = await GetTokenAsync(keycloakConfig);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"/admin/realms/{keycloakConfig.RealmName}/users?username={request.username}";
            var searchResponse = await _httpClient.GetAsync(url);

            var keycloakUser = JsonConvert.DeserializeObject<List<KeycloakUserResponse>>(await searchResponse.Content.ReadAsStringAsync());

            return keycloakUser;
        }

        private async Task<string> GetTokenAsync(KeycloakConfig keycloakConfig)
        {

            var client = _httpClientFactory.CreateClient("KeycloakClient");

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"/realms/{keycloakConfig.RealmName}/protocol/openid-connect/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", keycloakConfig.ClientId),
                new KeyValuePair<string, string>("client_secret", keycloakConfig.ClientSecret)
            })
            };

            _logger.LogInformation($"GetTokenAsync requestString: {JsonConvert.SerializeObject(tokenRequest)}");

            var response = await client.SendAsync(tokenRequest);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"GetTokenAsync response: {responseContent}");

            var token = JsonConvert.DeserializeObject<KeycloakTokenResponse>(responseContent)?.access_token;
            return token ?? "";
        }

    }
}
