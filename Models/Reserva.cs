using System.ComponentModel.DataAnnotations;
using PC02.Models;

public class Reserva
{
    public int Id { get; set; }

    [Required]
    public int InmuebleId { get; set; }
    public Inmueble Inmueble { get; set; }

    [Required]
    public string UsuarioId { get; set; }

    [Required]
    public DateTime FechaExpiracion { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
