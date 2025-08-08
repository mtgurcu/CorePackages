using CorePackages.Persistance.Entity;

namespace CorePackages.Persistance.Interfaces
{
    public interface IOutboxRepository
    {
        Task<List<OutboxEntity>> GetInitialTransactions(int batchSize, string jobKey);
        Task<OutboxEntity> CreateTransaction(OutboxEntity transaction);
        Task<OutboxEntity> UpdateTransaction(OutboxEntity transaction);
    }
}
