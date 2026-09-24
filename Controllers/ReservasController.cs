using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookingSystem.Data;
using BookingSystem.Models;

namespace BookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private bool IsAdmin => User.IsInRole("admin");

        // Admins see every reservation; regular users only see their own
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            var query = _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Sala)
                .AsQueryable();

            if (!IsAdmin)
            {
                var userId = CurrentUserId;
                query = query.Where(r => r.UsuarioId == userId);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> GetReserva(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Sala)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null) return NotFound();
            if (!IsAdmin && reserva.UsuarioId != CurrentUserId) return Forbid();

            return reserva;
        }

        [HttpPost]
        public async Task<ActionResult<Reserva>> PostReserva(Reserva reserva)
        {
            // The reservation always belongs to the logged-in user, never to an id sent by the client
            reserva.UsuarioId = CurrentUserId;
            reserva.Usuario = null;
            reserva.Sala = null;

            if (reserva.DataFim <= reserva.DataInicio)
            {
                return BadRequest("A data de fim deve ser posterior à data de início.");
            }

            if (!await _context.Salas.AnyAsync(s => s.Id == reserva.SalaId))
            {
                return BadRequest("Sala não encontrada.");
            }

            // Verifica conflito de horário na mesma sala
            var conflito = await _context.Reservas
                .AnyAsync(r =>
                    r.SalaId == reserva.SalaId &&
                    r.DataInicio < reserva.DataFim &&
                    reserva.DataInicio < r.DataFim);

            if (conflito)
            {
                return BadRequest("Já existe uma reserva para essa sala nesse horário.");
            }

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReserva), new { id = reserva.Id }, reserva);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();
            if (!IsAdmin && reserva.UsuarioId != CurrentUserId) return Forbid();

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
