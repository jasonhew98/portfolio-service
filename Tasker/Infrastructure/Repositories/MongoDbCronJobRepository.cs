using Domain.AggregatesModel.CronJobAggregate;
using Tasker.Infrastructure.Seedwork;

namespace Tasker.Infrastructure.Repositories
{
    public class MongoDbCronJobRepository : MongoDbRepository<CronJob>, ICronJobRepository
    {
        public MongoDbCronJobRepository(IMongoContext context, string collectionName)
            : base(context, collectionName)
        {
        }
    }
}