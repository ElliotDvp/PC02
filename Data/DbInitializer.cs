using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PC02.Models;     // <-- para Inmueble y TipoInmueble

namespace PC02.Data      // <-- asegúrate de estar en el mismo namespace que ApplicationDbContext
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext ctx)
        {
            // Aplicar cualquier migración pendiente
            ctx.Database.Migrate();

            // Si ya hay inmuebles, salimos
            if (ctx.Inmuebles.Any()) return;

            // Lista de inmuebles para semilla
            var inmuebles = new[]
            {
                new Inmueble {
                    Codigo       = "A100",
                    Titulo       = "Departamento céntrico",
                    Tipo         = TipoInmueble.Departamento,
                    Ciudad       = "Lima",
                    Direccion    = "Av. Ejemplo 123",
                    Dormitorios  = 2,
                    Banos        = 1,
                    MetrosCuadrados = 65,
                    Precio       = 85000,
                    Activo       = true
                },
                new Inmueble {
                    Codigo       = "B200",
                    Titulo       = "Casa familiar",
                    Tipo         = TipoInmueble.Casa,
                    Ciudad       = "Lima",
                    Direccion    = "Calle Falsa 456",
                    Dormitorios  = 4,
                    Banos        = 3,
                    MetrosCuadrados = 180,
                    Precio       = 250000,
                    Activo       = true
                },
                new Inmueble {
                    Codigo       = "C300",
                    Titulo       = "Oficina moderna",
                    Tipo         = TipoInmueble.Oficina,
                    Ciudad       = "Lima",
                    Direccion    = "Paseo 12",
                    Dormitorios  = 0,
                    Banos        = 1,
                    MetrosCuadrados = 45,
                    Precio       = 60000,
                    Activo       = true
                }
            };

            // Aquí sí recibe un IEnumerable<Inmueble>
            ctx.Inmuebles.AddRange(inmuebles);
            ctx.SaveChanges();
        }
    }
}
