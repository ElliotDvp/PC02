using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PC02.Models
{
    public class Inmueble
    {
        public int Id { get; set; }

        [Required]
        public string? Codigo { get; set; }

        [Required]
        public string? Titulo { get; set; }

        public string? Imagen { get; set; }

        public TipoInmueble Tipo { get; set; }

        public string? Ciudad { get; set; }

        public string? Direccion { get; set; }

        [Range(0.01,double.MaxValue)]
        public decimal Precio { get; set; }

        [Range(0.01,double.MaxValue)]
        public double MetrosCuadrados { get; set; }

        public int Dormitorios { get; set; }

        public int Banos { get; set; }

        public bool Activo { get; set; } = true;

        public List<Visita>? Visitas { get; set; }= new();
        public List<Reserva>? Reservas { get; set; } = new();
    }
}
