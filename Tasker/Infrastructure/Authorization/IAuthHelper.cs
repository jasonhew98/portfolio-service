using Tasker.Model;
using Domain.AggregatesModel.UserAggregate;

namespace Tasker.Infrastructure.Authorization
{
    public interface IAuthHelper
    {
        JwtTokenDto GenerateJwtToken(User userInfo);
    }
}
