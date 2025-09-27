using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PC02.Data;
using PC02.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // Agrega este using
using System.Text.Json; // Para serializar filtros
using Microsoft.Extensions.Caching.Distributed; // Agrega este using
using StackExchange.Redis;
using System.Text.Json;

namespace PC02.Controllers
{
    public class CatalogoController : Controller
    {
            private readonly ApplicationDbContext _ctx;
            private readonly IDatabase _db;
            private const int PageSize = 10;

            public CatalogoController(ApplicationDbContext ctx, IConnectionMultiplexer muxer)
        {
            _ctx = ctx;
            _db = muxer.GetDatabase();
        }

        public async Task<IActionResult> Index(CatalogoFilter filter)
    {
        HttpContext.Session.SetString("CatalogoFilter", JsonSerializer.Serialize(filter));
        if (!ModelState.IsValid) filter.Page = 1;

        var cacheKey = $"inmuebles:{filter.Ciudad}:{filter.Tipo}:{filter.PrecioMin}:{filter.PrecioMax}:{filter.DormitoriosMin}:{filter.Page}";
        CatalogoCache? cacheEntry = null;

        // 1) Intentar leer String
        var packed = await _db.StringGetAsync(cacheKey);
        if (packed.HasValue)
        {
            cacheEntry = JsonSerializer.Deserialize<CatalogoCache>(packed!);
        }

        List<Inmueble> inmuebles;
        int total;

        if (cacheEntry != null)
        {
            inmuebles = cacheEntry.Inmuebles;
            total = cacheEntry.Total;
        }
        else
        {
            // 2) Traer de BD
            var query = _ctx.Inmuebles.Where(i => i.Activo);
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

            total = await query.CountAsync();
            inmuebles = await query
                .Skip((filter.Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // 3) Serializar y guardar con TTL 60s
            cacheEntry = new CatalogoCache { Inmuebles = inmuebles, Total = total };
            var payload = JsonSerializer.Serialize(cacheEntry);
            await _db.StringSetAsync(cacheKey, payload, TimeSpan.FromSeconds(60));
        }

        var vm = new CatalogoViewModel
        {
            Filter = filter,
            Inmuebles = inmuebles,
            PageInfo = new PageInfo { TotalItems = total, PageSize = PageSize, Page = filter.Page },
            Ciudades = await _ctx.Inmuebles.Select(i => i.Ciudad!).Distinct().ToListAsync()
        };

        return View(vm);
    }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            // Guarda el último inmueble visitado en sesión
            HttpContext.Session.SetInt32("LastInmuebleId", id);

            var inmueble = await _ctx.Inmuebles
              .Include(i => i.Reservas)
              .FirstOrDefaultAsync(i => i.Id == id);
            if (inmueble == null) return NotFound();

            var vm = new InmuebleDetailViewModel
            {
                Inmueble = inmueble,
                HasActiveReservation = inmueble.Reservas.Any(r => r.FechaExpiracion > DateTime.UtcNow),
                Feedback = TempData["Feedback"] as string,
                Success = (TempData["Success"] as bool?) == true
            };
            vm.VisitaInput.InmuebleId = id;
            return View(vm);
        }
    }
}
