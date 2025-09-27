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
    public class VisitaController : Controller
    {
        private readonly ApplicationDbContext _ctx;

        public VisitaController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpPost]
        public IActionResult Create(VisitaInputModel model)
        {
            // Verificar horario laboral
            var hi = model.FechaInicio.TimeOfDay;
            var hf = model.FechaFin.TimeOfDay;
            if (model.FechaInicio >= model.FechaFin
                || hi < TimeSpan.FromHours(8)
                || hf > TimeSpan.FromHours(19))
            {
                TempData["Feedback"] = "Horario inválido o FechaInicio ≥ FechaFin";
                TempData["Success"] = false;
                return RedirectToAction("Details", "Catalogo", new { id = model.InmuebleId });
            }

            // Crear entidad Visita
            var visita = new Visita
            {
                InmuebleId  = model.InmuebleId,
                FechaInicio = model.FechaInicio,
                FechaFin    = model.FechaFin,
                Notas       = model.Notas,
                UsuarioId   = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            try
            {
                _ctx.Visitas.Add(visita);
                _ctx.SaveChanges();
                TempData["Feedback"] = "Visita agendada con éxito";
                TempData["Success"] = true;
            }
            catch (Exception ex)
            {
                // Captura solapamientos u otras validaciones de DbContext
                TempData["Feedback"] = ex.Message;
                TempData["Success"] = false;
            }

            return RedirectToAction("Details", "Catalogo", new { id = model.InmuebleId });
        }
    }
}
