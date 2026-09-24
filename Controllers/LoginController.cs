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
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest("Email e senha são obrigatórios.");
            }

            var email = request.Email.Trim().ToLower();
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email.ToLower().Trim() == email);

            if (usuario == null || string.IsNullOrEmpty(usuario.SenhaHash))
            {
                _logger.LogWarning("Failed login attempt for {Email}", email);
                return Unauthorized("Email ou senha inválidos");
            }

            // Verificação com BCrypt
            bool senhaOk = BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash);

            if (!senhaOk)
            {
                _logger.LogWarning("Failed login attempt for {Email}", email);
                return Unauthorized("Email ou senha inválidos");
            }

            var token = GerarToken(usuario);
            return Ok(new { token });
        }

        private string GerarToken(Usuario usuario)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
            var cred = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Role)
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

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
