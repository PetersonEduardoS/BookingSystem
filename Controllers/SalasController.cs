using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Data;
using BookingSystem.Models;

namespace BookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sala>>> GetSalas()
        {
            return await _context.Salas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Sala>> GetSala(int id)
        {
            var sala = await _context.Salas.FindAsync(id);
            if (sala == null) return NotFound();
            return sala;
        }

        [HttpPost]
        public async Task<ActionResult<Sala>> PostSala(Sala sala)
        {
            _context.Salas.Add(sala);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSala), new { id = sala.Id }, sala);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSala(int id, Sala updatedSala)
        {
            var sala = await _context.Salas.FindAsync(id);
            if (sala == null) return NotFound();

            sala.Nome = updatedSala.Nome;
            sala.Capacidade = updatedSala.Capacidade;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSala(int id)
        {
            var sala = await _context.Salas.FindAsync(id);
            if (sala == null) return NotFound();
            _context.Salas.Remove(sala);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
