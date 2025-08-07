using CorePackages.Infrastructure.Dto;
using System.Net.Http.Json;
using System.Text.Json;
using IVaultClient = CorePackages.Infrastructure.Interfaces.IVaultClient;

namespace CorePackages.Infrastructure.Services
{
    public class VaultClient : IVaultClient
    {
        private readonly HttpClient _client;
        private readonly string? _roleId;
        private readonly string? _secretId;

        private DateTime _tokenExpireDate;
        private const string TokenHeaderName = "X-Vault-Token";

        public VaultClient(HttpClient client, string? roleId, string? secretId)
        {
            _client = client;
            _roleId = roleId;
            _secretId = secretId;
            _tokenExpireDate = DateTime.MinValue;
        }

        #region PrivateMethods
        private volatile bool _refreshing = false;
        private readonly object _lockObject = new object();

        private void RefreshToken()
        {
            if (!_refreshing)
            {
                lock (_lockObject)
                {
                    if (!_refreshing)
                    {
                        _refreshing = true;
                        try
                        {
                            if (DateTime.Now > _tokenExpireDate)
                            {
                                LoginClientAsync().GetAwaiter().GetResult();
                            }
                        }
                        finally
                        {
                            _refreshing = false;
                        }
                    }
                }
            }
        }

        private async Task LoginClientAsync()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment?.ToLowerInvariant() == "staging")
            {
                _client.DefaultRequestHeaders.Add("X-Vault-Namespace", "admin");
            }

            var response = await _client.PostAsJsonAsync<VaultLoginRequest>($"/v1/auth/approle/login", new()
            {
                role_id = _roleId,
                secret_id = _secretId
            });

            if (!response.IsSuccessStatusCode)
            {
                var failedContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login failed! {failedContent}");
            }
            var vaultLoginResponse = await response.Content.ReadFromJsonAsync<VaultLoginResponse>();

            _tokenExpireDate = DateTime.Now.AddSeconds(vaultLoginResponse?.auth.lease_duration ?? 0);

            while (_client.DefaultRequestHeaders.Contains(TokenHeaderName))
            {
                _client.DefaultRequestHeaders.Remove(TokenHeaderName);
            }

            _client.DefaultRequestHeaders.Add(TokenHeaderName, vaultLoginResponse?.auth.client_token);
        }
        #endregion

        public async Task<T?> GetSecretValueAsync<T>(string engine, string path)
        {
            RefreshToken();

            var response = await _client.GetFromJsonAsync<VaultSecretResponse>($"/v1/{engine}/data/{path}");

            var responseData = response?.data?.data;

            if (responseData?.ValueKind == JsonValueKind.Object && responseData?.ToString() == "{}")
            {
                return default;
            }

            return response != null ? response.data.data.Deserialize<T>() : default;
        }

        public T? GetSecretValue<T>(string engine, string path)
        {
            return GetSecretValueAsync<T>(engine, path).GetAwaiter().GetResult();
        }
    }
}
