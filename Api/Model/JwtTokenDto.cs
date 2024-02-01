using System;

namespace Api.Model
{
    public class JwtTokenDto
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}
