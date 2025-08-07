using CorePackages.Infrastructure.Dto;
using CorePackages.Infrastructure.Extentions;
using CorePackages.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CorePackages.Infrastructure.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly IHttpContextService _contextService;
        public RequestResponseLoggingMiddleware(
             RequestDelegate next,
             ILogger<RequestResponseLoggingMiddleware> logger,
             IHttpContextService contextService)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _logger = logger;
            _contextService = contextService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            context.Request.Body.Position = 0;
            using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            context.Request.Body.Position = requestStream.Position = 0;
            using var requestStreamReader = new StreamReader(requestStream);
            var requestString = await requestStreamReader.ReadToEndAsync();

            var originalBodyStream = context.Response.Body;
            using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            var watch = Stopwatch.StartNew();
            await _next(context);
            watch.Stop();

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseStreamReader = new StreamReader(context.Response.Body);
            var responseString = await responseStreamReader.ReadToEndAsync();

            long elapsedMilliseconds = watch.ElapsedMilliseconds;

            HandleLoggingRequestResponseLog(context, requestString, responseString, elapsedMilliseconds);

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            await responseBody.CopyToAsync(originalBodyStream);
        }
        private void HandleLoggingRequestResponseLog(HttpContext context, string requestBody, string responseBody, long elapsedMilliseconds)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                var logModel = new RequestResponseLog
                {
                    Url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}",
                    Method = request.Method,
                    RequestHeaders = JsonConvert.SerializeObject(request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())),
                    Request = requestBody,
                    ResponseHeaders = JsonConvert.SerializeObject(response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())),
                    Response = responseBody,
                    StatusCode = response.StatusCode,
                    ElapsedMilliseconds = elapsedMilliseconds,
                    Timestamp = DateTime.UtcNow,
                    HttpContextId = _contextService.GetHttpContextId()
                };

                _logger.LogInformation(logModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling request response logging");
            }
        }
    }
}