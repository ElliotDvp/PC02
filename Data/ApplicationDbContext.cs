using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PC02.Models;

namespace PC02.Data;


public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

    public DbSet<Inmueble> Inmuebles { get; set; }
    public DbSet<Visita> Visitas { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Inmueble>()
            .HasIndex(i => i.Codigo)
            .IsUnique();

        // Validaciones de rango ya en DataAnnotations; config adicional:
        builder.Entity<Inmueble>()
            .Property(i => i.Precio)
            .HasConversion<decimal>()
            .IsRequired();

        // Reserva: no permitir más de una reserva activa por inmueble
        // SQLite no soporta filtros via EFProvider uniformly; añadiremos índice parcial con migration raw SQL.
        builder.Entity<Reserva>()
            .HasIndex(r => r.InmuebleId)
            .HasDatabaseName("IX_Reserva_Inmueble_Active"); // será reemplazado/creado en la migración con filtro.

        // Restricción simple sobre Visita: FechaInicio < FechaFin (check en aplicación y DB)
        // Agregamos check constraint para Visita si SQLite soporta: usamos raw SQL later in migration.
    }

    public override int SaveChanges()
    {
        ValidateBusinessRules();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ValidateBusinessRules();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ValidateBusinessRules()
    {
        var now = DateTime.UtcNow;

        // Validar visitas: FechaInicio < FechaFin y no solapamiento para mismo inmueble
        var addedOrModifiedVisitas = ChangeTracker.Entries<Visita>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity);

        foreach (var v in addedOrModifiedVisitas)
        {
            if (v.FechaInicio >= v.FechaFin) throw new InvalidOperationException("FechaInicio debe ser menor que FechaFin");

            bool overlap = Visitas
                .Where(x => x.InmuebleId == v.InmuebleId && x.Id != v.Id)
                .Any(x => x.FechaInicio < v.FechaFin && v.FechaInicio < x.FechaFin);

            if (overlap) throw new InvalidOperationException("Existe una visita solapada para este inmueble");
        }

        // Validar reservas activas: no permitir crear una reserva si ya existe una activa para el inmueble
        var addedReservas = ChangeTracker.Entries<Reserva>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity);

        foreach (var r in addedReservas)
        {
            bool hasActive = Reservas
                .Where(x => x.InmuebleId == r.InmuebleId)
                .Any(x => x.FechaExpiracion > now);

            if (hasActive) throw new InvalidOperationException("El inmueble ya tiene una reserva activa");
        }
    }
}