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
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private bool IsAdmin => User.IsInRole("admin");

        // Admins see every booking; regular users only see their own
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .AsQueryable();

            if (!IsAdmin)
            {
                var userId = CurrentUserId;
                query = query.Where(b => b.UserId == userId);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();
            if (!IsAdmin && booking.UserId != CurrentUserId) return Forbid();

            return booking;
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            // The booking always belongs to the logged-in user, never to an id sent by the client
            booking.UserId = CurrentUserId;
            booking.User = null;
            booking.Room = null;

            var validationError = await ValidateBookingAsync(booking.RoomId, booking.StartDate, booking.EndDate);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking updated)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();
            if (!IsAdmin && booking.UserId != CurrentUserId) return Forbid();

            var validationError = await ValidateBookingAsync(updated.RoomId, updated.StartDate, updated.EndDate, ignoreBookingId: id);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            // Only room and dates can change; the owner of the booking stays the same
            booking.RoomId = updated.RoomId;
            booking.StartDate = updated.StartDate;
            booking.EndDate = updated.EndDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();
            if (!IsAdmin && booking.UserId != CurrentUserId) return Forbid();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Shared rules for creating and updating a booking.
        // ignoreBookingId excludes the booking being edited from the conflict check.
        private async Task<string?> ValidateBookingAsync(int roomId, DateTime startDate, DateTime endDate, int? ignoreBookingId = null)
        {
            if (endDate <= startDate)
            {
                return "A data de fim deve ser posterior à data de início.";
            }

            if (!await _context.Rooms.AnyAsync(r => r.Id == roomId))
            {
                return "Sala não encontrada.";
            }

            var hasConflict = await _context.Bookings
                .AnyAsync(b =>
                    b.Id != ignoreBookingId &&
                    b.RoomId == roomId &&
                    b.StartDate < endDate &&
                    startDate < b.EndDate);

            return hasConflict ? "Já existe uma reserva para essa sala nesse horário." : null;
        }
    }
}
