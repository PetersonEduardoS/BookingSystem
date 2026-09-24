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
                .Include(r => r.Room)
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
                .Include(r => r.Room)
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
            reserva.Room = null;

            var validationError = await ValidateReservaAsync(reserva.RoomId, reserva.DataInicio, reserva.DataFim);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReserva), new { id = reserva.Id }, reserva);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReserva(int id, Reserva updated)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();
            if (!IsAdmin && reserva.UsuarioId != CurrentUserId) return Forbid();

            var validationError = await ValidateReservaAsync(updated.RoomId, updated.DataInicio, updated.DataFim, ignoreReservaId: id);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            // Only room and dates can change; the owner of the reservation stays the same
            reserva.RoomId = updated.RoomId;
            reserva.DataInicio = updated.DataInicio;
            reserva.DataFim = updated.DataFim;

            await _context.SaveChangesAsync();
            return NoContent();
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

        // Shared rules for creating and updating a reservation.
        // ignoreReservaId excludes the reservation being edited from the conflict check.
        private async Task<string?> ValidateReservaAsync(int roomId, DateTime dataInicio, DateTime dataFim, int? ignoreReservaId = null)
        {
            if (dataFim <= dataInicio)
            {
                return "A data de fim deve ser posterior à data de início.";
            }

            if (!await _context.Rooms.AnyAsync(s => s.Id == roomId))
            {
                return "Sala não encontrada.";
            }

            var conflito = await _context.Reservas
                .AnyAsync(r =>
                    r.Id != ignoreReservaId &&
                    r.RoomId == roomId &&
                    r.DataInicio < dataFim &&
                    dataInicio < r.DataFim);

            return conflito ? "Já existe uma reserva para essa sala nesse horário." : null;
        }
    }
}
