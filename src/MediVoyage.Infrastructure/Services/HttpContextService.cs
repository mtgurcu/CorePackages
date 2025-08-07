using CorePackages.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace CorePackages.Infrastructure.Services
{
    public class HttpContextService : IHttpContextService
    {
        public string HttpContextId => "HttpContextId";
        private readonly static AsyncLocal<string?> _httpContextId = new();
        private readonly ILogger _logger;


        public HttpContextService(ILogger<HttpContextService> logger)
        {
            _logger = logger;
        }
        public void SetHttpContextId(string httpContextId)
        {
            if (_httpContextId.Value != null)
            {
                _logger.LogWarning("HttpContextId has already been set and supposed to set once per scope. Resetting may cause incorrect results.");
            }
            _httpContextId.Value = httpContextId;
        }

        public string? GetHttpContextId()
        {
            if (_httpContextId.Value == null)
            {
                _logger.LogWarning("HttpContextId has not been set yet, returning null");
            }
            return _httpContextId.Value;
        }
    }
}
