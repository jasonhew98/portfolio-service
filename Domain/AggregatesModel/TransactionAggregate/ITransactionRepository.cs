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
            int sortOrder,
            string userId = null);

        Task<long> GetTransactionCount(string userId = null);

        Task<Transaction> GetTransaction(
            string transactionId,
            string userId = null);

        Task<bool> UpdateTransaction(
            Transaction transaction,
            (string id, string name) currentUser,
            string userId);
    }
}
