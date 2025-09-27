using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PC02.Models
{
  public class CatalogoFilter : IValidatableObject
  {
    public string? Ciudad { get; set; }

    public TipoInmueble? Tipo { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Precio mínimo no negativo")]
    public decimal? PrecioMin { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Precio máximo no negativo")]
    public decimal? PrecioMax { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Dormitorios no negativos")]
    public int? DormitoriosMin { get; set; }

    public int Page { get; set; } = 1;

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
      if (PrecioMin.HasValue && PrecioMax.HasValue && PrecioMin > PrecioMax)
      {
        yield return new ValidationResult(
          "Precio mínimo debe ser menor o igual al máximo",
          new[] { nameof(PrecioMin), nameof(PrecioMax) }
        );
      }
    }
  }
}
