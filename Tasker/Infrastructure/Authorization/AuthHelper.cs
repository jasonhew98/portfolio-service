using Tasker.Model;
using Domain.AggregatesModel.UserAggregate;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Tasker.Infrastructure.Authorization
{
    public class AuthHelper : IAuthHelper
    {
        private readonly JwtAuthorizationConfigurationOptions _jwtAuthorizationConfigurationOptions;

        public AuthHelper(
            IOptions<JwtAuthorizationConfigurationOptions> jwtAuthorizationConfigurationOptions,
            ILogger<AuthHelper> logger)
        {
            _jwtAuthorizationConfigurationOptions = jwtAuthorizationConfigurationOptions.Value;
        }

        public JwtTokenDto GenerateJwtToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthorizationConfigurationOptions.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim("id", userInfo.UserId),
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiryDate = DateTime.Now.AddMinutes(120);

            var token = new JwtSecurityToken(_jwtAuthorizationConfigurationOptions.Issuer,
                _jwtAuthorizationConfigurationOptions.Issuer,
                claims,
                expires: expiryDate,
                signingCredentials: credentials);

            return new JwtTokenDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiry = expiryDate
            };
        }
    }
}
