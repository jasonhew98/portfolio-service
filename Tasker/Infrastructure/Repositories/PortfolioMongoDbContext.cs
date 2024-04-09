using Tasker.Infrastructure.Seedwork;
using MediatR;

namespace Tasker.Infrastructure.Repositories
{
    public class PortfolioMongoDbContext : MongoDbContext
    {
        public PortfolioMongoDbContext(string mongoDbUrl, string database, IMediator mediator)
            : base(mongoDbUrl, database, mediator)
        {
        }
    }
}
