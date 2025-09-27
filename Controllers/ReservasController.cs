using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PC02.Data;

namespace PC02.Areas.Broker.Controllers
{
  public class ReservasController : BrokerControllerBase
  {
    private readonly ApplicationDbContext _ctx;
    public ReservasController(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IActionResult> Active()
      => View(await _ctx.Reservas
          .Include(r=>r.Inmueble)
          .Where(r=>r.FechaExpiracion>DateTime.UtcNow)
          .ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Release(int id)
    {
      var r = await _ctx.Reservas.FindAsync(id);
      if (r!=null)
      {
        _ctx.Reservas.Remove(r);
        await _ctx.SaveChangesAsync();
      }
      return RedirectToAction(nameof(Active));
    }
  }
}
