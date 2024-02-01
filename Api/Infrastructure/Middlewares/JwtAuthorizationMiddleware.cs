﻿using Domain.AggregatesModel.UserAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api.Infrastructure.Middlewares
{
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtAuthorizationConfigurationOptions _jwtConfig;

        public JwtAuthorizationMiddleware(
            RequestDelegate next,
            IOptions<JwtAuthorizationConfigurationOptions> jwtConfig)
        {
            _next = next;
            _jwtConfig = jwtConfig.Value;
        }

        public async Task Invoke(HttpContext context, IUserRepository userRepository)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await attachUserToContext(context, userRepository, token);

            await _next(context);
        }

        private async Task attachUserToContext(HttpContext context, IUserRepository userRepository, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                var identity = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims));

                // attach user to context on successful jwt validation
                context.User = identity;
                context.Items["User"] = await userRepository.GetUser(userId: userId);
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
