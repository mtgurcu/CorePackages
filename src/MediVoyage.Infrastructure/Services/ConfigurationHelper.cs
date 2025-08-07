using CorePackages.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CorePackages.Infrastructure.Services
{
    public class ConfigurationHelper<T> : IConfigurationHelper<T> where T : new()
    {
        private readonly IConfiguration _configuration;
        private readonly IVaultClient _vaultClient;
        private readonly bool _isVaultEnabled;
        private readonly string _secretName;
        public ConfigurationHelper(IConfiguration configuration, IVaultClient vaultClient)
        {
            _configuration = configuration;
            _vaultClient = vaultClient;
            _secretName = configuration["VaultSettings:SecretName"] ?? "";
            _isVaultEnabled = Convert.ToBoolean(configuration["VaultSettings:IsVaultEnabled"]);
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment?.ToLower() == "development")
                _secretName += "-Dev";
        }
        public async Task<T?> GetConfigurationAsync(string path)
        {
            if (_isVaultEnabled)
            {
                var value = await _vaultClient.GetSecretValueAsync<T>(_secretName, path);
                return value;
            }
            var config = new T();

            _configuration.GetSection(path).Bind(config);
            return config;
        }
    }
}
