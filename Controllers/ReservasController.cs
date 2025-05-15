using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Data;
using BookingSystem.Models;

namespace BookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Sala)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> GetReserva(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Sala)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null) return NotFound();
            return reserva;
        }

        [HttpPost]
        public async Task<ActionResult<Reserva>> PostReserva(Reserva reserva)
        {
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

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
