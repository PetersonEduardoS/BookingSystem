using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Data;
using BookingSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            return usuario;
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(UsuarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest("Nome, email e senha são obrigatórios.");
            }

            var emailNormalizado = request.Email.Trim().ToLower();

            // Verifica se já existe um usuário com o mesmo e-mail
            var existe = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == emailNormalizado);
            if (existe)
            {
                return Conflict("Já existe um usuário com este e-mail.");
            }

            var usuario = new Usuario
            {
                Nome = request.Nome.Trim(),
                Email = emailNormalizado,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                // Public sign-up always creates a regular user; roles are changed only by an admin
                Role = "usuario"
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.Role
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, [FromBody] UsuarioRequest request)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.Nome = request.Nome?.Trim() ?? usuario.Nome;
            usuario.Email = request.Email?.Trim().ToLower() ?? usuario.Email;
            usuario.Role = string.IsNullOrWhiteSpace(request.Role) ? usuario.Role : request.Role.ToLower();

            if (!string.IsNullOrWhiteSpace(request.Senha))
            {
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)

        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
