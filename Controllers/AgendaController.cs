using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PC02.Data;
using PC02.Models; // <-- Agrega este using si tu enum está en Models

namespace PC02.Areas.Broker.Controllers
{
  public class AgendaController : BrokerControllerBase
  {
    private readonly ApplicationDbContext _ctx;
    public AgendaController(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IActionResult> Today()
    {
      var hoy = DateTime.Today;
      var visitas = await _ctx.Visitas
        .Include(v=>v.Inmueble)
        .Where(v => v.FechaInicio.Date == hoy)
        .OrderBy(v=>v.InmuebleId)
        .ToListAsync();
      return View(visitas);
    }

    [HttpPost]
    public async Task<IActionResult> Confirm(int id)
    {
      var v = await _ctx.Visitas.FindAsync(id);
      if (v!=null) { v.Estado = EstadoVisita.Confirmada; await _ctx.SaveChangesAsync(); }
      return RedirectToAction(nameof(Today));
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
      var v = await _ctx.Visitas.FindAsync(id);
      if (v!=null) { v.Estado = EstadoVisita.Cancelada; await _ctx.SaveChangesAsync(); }
      return RedirectToAction(nameof(Today));
    }
  }
}
