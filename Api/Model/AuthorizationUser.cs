using Newtonsoft.Json;

namespace Api.Model
{
    public class AuthorizationUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        // Prevents password from being serialized and returned in API responses
        [JsonIgnore]
        public string Password { get; set; }
    }
}
