using System.Text.Json.Serialization;

namespace BookingSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "user";

        // Never serialize the password hash in API responses
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
