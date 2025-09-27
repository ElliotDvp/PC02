using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PC02.Data;
using PC02.Models;

namespace PC02.Areas.Broker.Controllers
{
  public class InmueblesController : BrokerControllerBase
  {
    private readonly ApplicationDbContext _ctx;
    public InmueblesController(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IActionResult> Index()
      => View(await _ctx.Inmuebles.ToListAsync());

    public IActionResult Create() => View(new Inmueble());

    [HttpPost]
    public async Task<IActionResult> Create(Inmueble m)
    {
      if (!ModelState.IsValid) return View(m);
      _ctx.Inmuebles.Add(m);
      await _ctx.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
      var m = await _ctx.Inmuebles.FindAsync(id);
      if (m==null) return NotFound();
      return View(m);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Inmueble m)
    {
      if (!ModelState.IsValid) return View(m);
      _ctx.Update(m);
      await _ctx.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActive(int id)
    {
      var m = await _ctx.Inmuebles.FindAsync(id);
      if (m!=null)
      {
        m.Activo = !m.Activo;
        await _ctx.SaveChangesAsync();
      }
      return RedirectToAction(nameof(Index));
    }
  }
}
