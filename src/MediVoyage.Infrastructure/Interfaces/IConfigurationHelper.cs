namespace CorePackages.Infrastructure.Interfaces
{
    public interface IConfigurationHelper<T>
    {
        Task<T?> GetConfigurationAsync(string path);
    }
}
