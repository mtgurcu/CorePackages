using CorePackages.Infrastructure.Dto;
using CorePackages.Infrastructure.Dto.Exceptions;
using CorePackages.Infrastructure.Interfaces;
using CorePackages.Persistance.Entity;
using CorePackages.Persistance.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog.Context;
using System.Diagnostics;

namespace CorePackages.Infrastructure.Services
{
    public abstract class OutboxJob<T, T1> : IJob
        where T : class
    {
        protected readonly IOutboxRepository _outboxRepository;
        protected readonly ILogger<T1> _logger;
        private readonly IConfigurationHelper<QuartzConfig> _quartzConfig;
        private readonly IHttpContextService _httpContextService;
        private readonly JobType _jobType;
        protected OutboxJob(IOutboxRepository outboxManager, ILogger<T1> logger, IConfigurationHelper<QuartzConfig> quartzConfig, IHttpContextService correlationIdService, JobType jobType)
        {
            _outboxRepository = outboxManager;
            _logger = logger;
            _quartzConfig = quartzConfig;
            _httpContextService = correlationIdService;
            _jobType = jobType;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var jobKey = context.JobDetail.Key.Name;

            _logger.LogInformation($"{jobKey} starts!");

            switch (_jobType)
            {
                case JobType.ApiRequest:
                    await ProcessOutboxJobTemplateAsync(
                        config => _outboxRepository.GetInitialTransactions(config.BatchSize, jobKey),
                        "Initial", jobKey);
                    break;

                default:
                    _logger.LogWarning($"Unknown job type: {_jobType}");
                    break;
            }

            stopwatch.Stop();
            _logger.LogInformation($"{jobKey} finishes! Time taken : {stopwatch.ElapsedMilliseconds}ms");
        }
        private async Task ProcessOutboxJobTemplateAsync(
    Func<QuartzConfigurations, Task<List<OutboxEntity>>> fetchTransactionsFunc,
    string logPrefix, string jobKey)
        {
            try
            {
                var config = (await _quartzConfig.GetConfigurationAsync("QuartzConfig"))?.Configurations.FirstOrDefault(c => c.JobKey == jobKey);

                var transactions = new List<OutboxEntity>();

                do
                {
                    transactions = await fetchTransactionsFunc(config);

                    if (transactions == null || !transactions.Any())
                    {
                        _logger.LogInformation($"No {logPrefix.ToLower()} transactions found to process for {jobKey}.");
                        return;
                    }

                    await IterateTransactionAsync(config, transactions);

                } while (transactions.Any());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Process{logPrefix}JobAsync error for {jobKey}.");
            }
        }

        protected virtual async Task IterateTransactionAsync(QuartzConfigurations config, List<OutboxEntity> transactions)
        {
            foreach (var transaction in transactions)
            {
                _httpContextService.SetHttpContextId(transaction.HttpContextId);
                LogContext.PushProperty("CorrelationId", transaction.HttpContextId);

                await ProcessTransactionAsync(transaction, config);
            }
        }

        protected static T2 CastEntity<T2>(OutboxEntity outboxEntity)
        {
            T2 result = default;

            if (outboxEntity is T2 castedEntity)
                result = castedEntity;

            return result;

        }
        protected abstract Task ProcessTransactionAsync(OutboxEntity outboxEntity, QuartzConfigurations config);
        protected async Task<ApiResponse<T>> SendApiRequestAsync(OutboxEntity outboxEntity)
        {
            var result = new ApiResponse<T>(false, "");

            try
            {
                result = await HandleHttpRequestAsync(result, outboxEntity);
            }
            catch (BusinessException ex)
            {
                result.Code = ex.ErrorCode;
                result.Message = ex.Message;

                _logger.LogError(ex, "SendApiRequestAsync exception for transaction ID: {TransactionId}", outboxEntity.Id);

                outboxEntity.RetryCount++;

                _logger.LogWarning("Retry count increased to {RetryCount} for transaction ID: {TransactionId}", outboxEntity.RetryCount, outboxEntity.Id);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;

                _logger.LogError(ex, "SendApiRequestAsync exception for transaction ID: {TransactionId}", outboxEntity.Id);

                outboxEntity.RetryCount++;

                _logger.LogWarning("Retry count increased to {RetryCount} for transaction ID: {TransactionId}", outboxEntity.RetryCount, outboxEntity.Id);
            }

            return result;
        }
        protected virtual Task<ApiResponse<T>> HandleHttpRequestAsync(ApiResponse<T> result, OutboxEntity entity)
        {
            return Task.FromResult(result);
        }
    }
}
