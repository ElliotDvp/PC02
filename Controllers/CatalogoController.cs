using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PC02.Data;
using PC02.Models;

namespace PC02.Controllers
{
  public class CatalogoController : Controller
  {
    private readonly ApplicationDbContext _ctx;
    private const int PageSize = 10;

    public CatalogoController(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IActionResult> Index(CatalogoFilter filter)
    {
      if (!ModelState.IsValid)
        filter.Page = 1;

      var query = _ctx.Inmuebles
        .Where(i => i.Activo);

      if (!string.IsNullOrWhiteSpace(filter.Ciudad))
        query = query.Where(i => i.Ciudad == filter.Ciudad);

      if (filter.Tipo.HasValue)
        query = query.Where(i => i.Tipo == filter.Tipo);

      if (filter.PrecioMin.HasValue)
        query = query.Where(i => i.Precio >= filter.PrecioMin);

      if (filter.PrecioMax.HasValue)
        query = query.Where(i => i.Precio <= filter.PrecioMax);

      if (filter.DormitoriosMin.HasValue)
        query = query.Where(i => i.Dormitorios >= filter.DormitoriosMin);

      var total = await query.CountAsync();

      var inmuebles = await query
        .Skip((filter.Page - 1) * PageSize)
        .Take(PageSize)
        .ToListAsync();

      var vm = new CatalogoViewModel
      {
        Filter     = filter,
        Inmuebles  = inmuebles,
        PageInfo   = new PageInfo { TotalItems = total, PageSize = PageSize, Page = filter.Page },
        Ciudades   = await _ctx.Inmuebles.Select(i => i.Ciudad!).Distinct().ToListAsync()
      };

      return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
      var inmueble = await _ctx.Inmuebles.FindAsync(id);
      if (inmueble == null) return NotFound();
      return View(inmueble);
    }
  }
}
