using CorePackages.Persistance.Entity;
using CorePackages.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CorePackages.Persistance.Repositories
{
    public class OutboxRepository : Repository<OutboxEntity>, IOutboxRepository
    {
        private readonly DbContext _dbContext;
        public OutboxRepository(DbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OutboxEntity> CreateTransaction(OutboxEntity transaction)
        {
            _dbContext.Set<OutboxEntity>().Add(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        }
        public async Task<List<OutboxEntity>> GetInitialTransactions(int batchSize, string jobKey)
        {
            var query = $@"
                WITH cot AS (
                    SELECT ot.*
                    FROM ""public"".""outbox_entity"" ot
                    WHERE ot.""status"" = 'Initial' 
                      AND ot.""is_locked"" = false
                      AND ot.""type"" = '{jobKey}'
                    ORDER BY ot.""created_date"" DESC
                    LIMIT {batchSize}
                    FOR UPDATE SKIP LOCKED
                )
                UPDATE ""public"".""outbox_entity"" ot
                SET ""is_locked"" = true
                FROM cot
                WHERE ot.""id"" = cot.""id""
                RETURNING ot.*;
            
            ";

            var initialTransactions = await _dbContext.Set<OutboxEntity>().FromSqlRaw(query).IgnoreQueryFilters().ToListAsync();

            return initialTransactions;
        }
        public async Task<OutboxEntity> UpdateTransaction(OutboxEntity transaction)
        {
            _dbContext.Update(transaction);

            await _dbContext.SaveChangesAsync();

            return transaction;
        }
    }
}
