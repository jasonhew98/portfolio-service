using Api.Model;
using Domain.AggregatesModel.UserAggregate;

namespace Api.Infrastructure.Authorization
{
    public interface IAuthHelper
    {
        JwtTokenDto GenerateJwtToken(User userInfo);
    }
}
