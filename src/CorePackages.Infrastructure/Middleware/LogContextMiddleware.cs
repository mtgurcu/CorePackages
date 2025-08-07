using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace CorePackages.Infrastructure.Middleware
{
    public class LogContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly static AsyncLocal<string?> _httpContextId = new();

        public LogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var httpContextId = context.Request.Headers
                                               .FirstOrDefault(pair => string.Equals(pair.Key, "HttpContextId", StringComparison.OrdinalIgnoreCase))
                                               .Value
                                               .FirstOrDefault();
            if (string.IsNullOrEmpty(httpContextId))
            {
                httpContextId = Guid.NewGuid().ToString();
            }

            _httpContextId.Value = httpContextId;

            using (LogContext.PushProperty("HttpContextId", httpContextId))
            {
                context.Response.Headers["HttpContextId"] = httpContextId;
                await _next(context);
            }
        }
    }
}
