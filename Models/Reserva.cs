using System;

namespace BookingSystem.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int SalaId { get; set; }
        public Sala? Sala { get; set; }

        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
