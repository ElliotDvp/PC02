using System.Collections.Generic;

namespace PC02.Models
{
  public class PageInfo
  {
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; }
    public int TotalPages => (int)System.Math.Ceiling((double)TotalItems / PageSize);
  }

  public class CatalogoViewModel
  {
    public CatalogoFilter Filter { get; set; } = new();
    public IEnumerable<Inmueble> Inmuebles { get; set; } = new List<Inmueble>();
    public PageInfo PageInfo { get; set; } = new();
    public IEnumerable<string> Ciudades { get; set; } = new List<string>();
  }
}
