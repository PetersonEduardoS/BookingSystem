using System.Text.Json.Serialization;

namespace BookingSystem.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "usuario";

        // Never serialize the password hash in API responses
        [JsonIgnore]
        public string SenhaHash { get; set; } = string.Empty;
    }
}
