using Api.Infrastructure.Seedwork;
using MediatR;

namespace Api.Infrastructure.Repositories
{
    public class PortfolioMongoDbContext : MongoDbContext
    {
        public PortfolioMongoDbContext(string mongoDbUrl, string database, IMediator mediator)
            : base(mongoDbUrl, database, mediator)
        {
        }
    }
}
