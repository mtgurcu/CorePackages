using CorePackages.Infrastructure.Dto;
using Microsoft.Extensions.Logging;

namespace CorePackages.Infrastructure.Extentions
{
    public static class LoggerExtension
    {
        public static void LogInformation(this ILogger logger, ILoggableAsInformation obj)
        {
            logger.LogInformation("{@" + obj.GetType().Name + "}", obj);
        }
    }
}