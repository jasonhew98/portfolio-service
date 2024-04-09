using Domain.AggregatesModel.TransactionAggregate;
using Tasker.Infrastructure.Seedwork;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Tasker.Infrastructure.Repositories
{
    public class MongoDbTransactionRepository : MongoDbRepository<Transaction>, ITransactionRepository
    {
        public MongoDbTransactionRepository(IMongoContext context, string collectionName)
            : base(context, collectionName)
        {
        }

        public async Task<Transaction> GetTransaction(
            string transactionId,
            string userId = null)
        {
            var filter = Builders<Transaction>.Filter.Empty;

            filter &= Builders<Transaction>.Filter.Eq(x => x.TransactionId, transactionId);

            if (!string.IsNullOrEmpty(userId))
                filter &= Builders<Transaction>.Filter.Eq(x => x.CreatedBy, userId);

            var property = await QueryOne(filter, null);

            return property;
        }

        public async Task<List<Transaction>> GetTransactions(
            int limit,
            int offset,
            string sortBy,
            int sortOrder,
            string userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string mainCategory = null,
            string subCategory = null,
            string paymentMethod = null,
            double? startPaymentAmount = null,
            double? endPaymentAmount = null)
        {
            var filter = Builders<Transaction>.Filter.Empty;

            if (!string.IsNullOrEmpty(userId))
                filter &= Builders<Transaction>.Filter.Eq(x => x.CreatedBy, userId);

            if (startDate.HasValue)
                filter &= Builders<Transaction>.Filter.Gte(x => x.TransactionDate, startDate);

            if (endDate.HasValue)
                filter &= Builders<Transaction>.Filter.Lte(x => x.TransactionDate, endDate);

            if (!string.IsNullOrEmpty(mainCategory))
                filter &= Builders<Transaction>.Filter.Eq(x => x.MainCategory, mainCategory);

            if (!string.IsNullOrEmpty(subCategory))
                filter &= Builders<Transaction>.Filter.Eq(x => x.SubCategory, subCategory);

            if (!string.IsNullOrEmpty(paymentMethod))
                filter &= Builders<Transaction>.Filter.Eq("paymentMethod", paymentMethod);

            if (startPaymentAmount.HasValue)
                filter &= Builders<Transaction>.Filter.Gte(x => x.PaymentAmount, startPaymentAmount);

            if (endPaymentAmount.HasValue)
                filter &= Builders<Transaction>.Filter.Lte(x => x.PaymentAmount, endPaymentAmount);

            sortBy = !string.IsNullOrEmpty(sortBy) ? sortBy : "_id";

            var sort = sortOrder == -1 ? Builders<Transaction>.Sort.Descending(sortBy) : Builders<Transaction>.Sort.Ascending(sortBy);

            var option = new FindOptions<Transaction, BsonDocument>
            {
                Limit = limit,
                Skip = offset,
                Sort = sort
            };

            var transactions = await Query<Transaction>(filter, option);

            return transactions.ToList();
        }

        public async Task<long> GetTransactionCount(
            string userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string mainCategory = null,
            string subCategory = null,
            string paymentMethod = null,
            double? startPaymentAmount = null,
            double? endPaymentAmount = null)
        {
            var filter = Builders<Transaction>.Filter.Empty;

            if (!string.IsNullOrEmpty(userId))
                filter &= Builders<Transaction>.Filter.Eq(x => x.CreatedBy, userId);

            if (startDate.HasValue)
                filter &= Builders<Transaction>.Filter.Gte(x => x.TransactionDate, startDate);

            if (endDate.HasValue)
                filter &= Builders<Transaction>.Filter.Lte(x => x.TransactionDate, endDate);

            if (!string.IsNullOrEmpty(mainCategory))
                filter &= Builders<Transaction>.Filter.Eq(x => x.MainCategory, mainCategory);

            if (!string.IsNullOrEmpty(subCategory))
                filter &= Builders<Transaction>.Filter.Eq(x => x.SubCategory, subCategory);

            if (!string.IsNullOrEmpty(paymentMethod))
                filter &= Builders<Transaction>.Filter.Eq("paymentMethod", paymentMethod);

            if (startPaymentAmount.HasValue)
                filter &= Builders<Transaction>.Filter.Gte(x => x.PaymentAmount, startPaymentAmount);

            if (endPaymentAmount.HasValue)
                filter &= Builders<Transaction>.Filter.Lte(x => x.PaymentAmount, endPaymentAmount);

            var count = await DbSet.CountDocumentsAsync(filter);

            return count;
        }

        public async Task<bool> UpdateTransaction(Transaction transaction, (string id, string name) currentUser, string userId)
        {
            transaction.SetModified(currentUser);

            var filter = Builders<Transaction>.Filter.Empty;

            if (!string.IsNullOrEmpty(userId))
                filter &= Builders<Transaction>.Filter.Eq(x => x.CreatedBy, userId);

            filter &= Builders<Transaction>.Filter.Eq(x => x.TransactionId, transaction.TransactionId);
            filter &= Builders<Transaction>.Filter.Eq(x => x.ModifiedUTCDateTime, transaction.OriginalModifiedUTCDateTime);

            var update = Builders<Transaction>.Update
                .Set(a => a.MainCategory, transaction.MainCategory)
                .Set(a => a.SubCategory, transaction.SubCategory)
                .Set(a => a.TransactionDate, transaction.TransactionDate)
                .Set(a => a.Notes, transaction.Notes)
                .Set(a => a.PaymentMethod, transaction.PaymentMethod)
                .Set(a => a.PaymentAmount, transaction.PaymentAmount)
                .Set(a => a.ModifiedBy, transaction.ModifiedBy)
                .Set(a => a.ModifiedByName, transaction.ModifiedByName)
                .Set(a => a.ModifiedUTCDateTime, transaction.ModifiedUTCDateTime);

            var options = new UpdateOptions { IsUpsert = false };

            var updateResult = await DbSet.UpdateOneAsync(filter, update, options);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}
