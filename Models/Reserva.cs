using System;

namespace BookingSystem.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int RoomId { get; set; }
        public Room? Room { get; set; }

        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
