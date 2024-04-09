using System;

namespace Tasker.Model
{
    public class JwtTokenDto
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}
