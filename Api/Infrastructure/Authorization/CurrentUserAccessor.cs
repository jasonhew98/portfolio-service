using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Infrastructure.Authorization
{
    public interface ICurrentUserAccessor
    {
        string UserId { get; set; }
        string UserName { get; set; }
    }

    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string UserId { get; set; }
        public string UserName { get; set; }

        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            UserId = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == "id")?.Value;
            UserName = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
        }
    }
}
