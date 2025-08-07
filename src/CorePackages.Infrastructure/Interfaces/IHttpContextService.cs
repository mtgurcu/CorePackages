namespace CorePackages.Infrastructure.Interfaces
{
    public interface IHttpContextService
    {

        string HttpContextId { get; }
        void SetHttpContextId(string httpContextId);
        string? GetHttpContextId();
    }
}
