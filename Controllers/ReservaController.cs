using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PC02.Data;
using PC02.Models;

namespace PC02.Controllers
{
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly ApplicationDbContext _ctx;

        public ReservaController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpPost]
        public IActionResult Create(int inmuebleId)
        {
            // 1. Verifica si ya hay reserva activa
            bool existe = _ctx.Reservas
                .Any(r => r.InmuebleId == inmuebleId && r.FechaExpiracion > DateTime.UtcNow);

            if (existe)
            {
                TempData["Feedback"] = "Ya existe una reserva activa para este inmueble.";
                TempData["Success"] = false;
                return RedirectToAction("Details", "Catalogo", new { id = inmuebleId });
            }

            // 2. Crear y persistir
            var reserva = new Reserva
            {
                InmuebleId      = inmuebleId,
                UsuarioId       = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                FechaExpiracion = DateTime.UtcNow.AddHours(48)
            };

            try
            {
                _ctx.Reservas.Add(reserva);
                _ctx.SaveChanges();
                TempData["Feedback"] = "Reserva creada por 48 h.";
                TempData["Success"] = true;
            }
            catch (Exception ex)
            {
                TempData["Feedback"] = ex.Message;
                TempData["Success"] = false;
            }

            return RedirectToAction("Details", "Catalogo", new { id = inmuebleId });
        }
    }
}
