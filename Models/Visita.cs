using System;
using System.ComponentModel.DataAnnotations;

namespace PC02.Models
{
    public class Visita
    {
        public int Id { get; set; }

        [Required]
        public int InmuebleId { get; set; }
        public Inmueble Inmueble { get; set; }

        [Required]
        public string? UsuarioId { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public EstadoVisita Estado { get; set; } = EstadoVisita.Solicitada;

        public string? Notas { get; set; }
    }
}
