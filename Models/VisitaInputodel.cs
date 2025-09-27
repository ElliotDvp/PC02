using System;
using System.ComponentModel.DataAnnotations;

namespace PC02.Models
{
  public class VisitaInputModel
  {
    [Required] public int InmuebleId { get; set; }
    [Required] public DateTime FechaInicio { get; set; }
    [Required] public DateTime FechaFin { get; set; }
    public string? Notas { get; set; }
  }
}
