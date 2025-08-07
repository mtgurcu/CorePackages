namespace CorePackages.Infrastructure.Interfaces
{

    public interface IVaultClient
    {
        T? GetSecretValue<T>(string engine, string path);
        Task<T?> GetSecretValueAsync<T>(string engine, string path);

    }
}
