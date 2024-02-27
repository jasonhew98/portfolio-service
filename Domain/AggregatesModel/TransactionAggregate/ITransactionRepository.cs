using Domain.Seedwork;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.AggregatesModel.TransactionAggregate
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<List<Transaction>> GetTransactions(
            int limit,
            int offset,
            string sortBy,
            int sortOrder);

        Task<long> GetTransactionCount();

        Task<Transaction> GetTransaction(
            string userId = null);

        Task<bool> UpdateTransaction(
            Transaction transaction,
            (string id, string name) currentUser,
            string userId);
    }
}
