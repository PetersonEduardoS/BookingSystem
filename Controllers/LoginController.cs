using BookingSystem.Data;
using BookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AppDbContext context, IConfiguration config, ILogger<LoginController> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email e senha são obrigatórios.");
            }

            var email = request.Email.Trim().ToLower();
            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower().Trim() == email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for {Email}", email);
                return Unauthorized("Email ou senha inválidos");
            }

            // Verificação com BCrypt
            bool passwordOk = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!passwordOk)
            {
                _logger.LogWarning("Failed login attempt for {Email}", email);
                return Unauthorized("Email ou senha inválidos");
            }

            var token = GenerateToken(user);
            return Ok(new { token });
        }

        private string GenerateToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
            var cred = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
