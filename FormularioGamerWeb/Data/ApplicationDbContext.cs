using Microsoft.EntityFrameworkCore;
using FormularioGamerWeb.Models;

namespace FormularioGamerWeb.Data
{
    /// <summary>
    /// El DbContext es la clase central de Entity Framework Core.
    /// Representa una sesión con la base de datos y permite consultar
    /// y guardar instancias de nuestros modelos (RegistroJugador).
    ///
    /// IMPORTANTE: Esta aplicación usa Database First / Code First "ligero":
    /// la tabla se crea MANUALMENTE en SSMS (ver script-crear-bd.sql),
    /// y este DbContext simplemente se MAPEA a esa tabla ya existente.
    /// No usamos Migrations para mantenerlo simple y que tengas control
    /// total desde SSMS, tal como pediste.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Esta propiedad representa la tabla "RegistrosJugadores" en SQL Server.
        // EF Core traduce LINQ (C#) a sentencias T-SQL automáticamente.
        public DbSet<RegistroJugador> RegistrosJugadores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo explícito al nombre exacto de la tabla creada en SSMS
            modelBuilder.Entity<RegistroJugador>(entity =>
            {
                entity.ToTable("RegistrosJugadores");

                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Apellido).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Genero).HasMaxLength(20);
                entity.Property(e => e.Pais).HasMaxLength(60);
                entity.Property(e => e.PlataformaFavorita).HasMaxLength(60);
                entity.Property(e => e.ColorFavorito).HasMaxLength(7);
                entity.Property(e => e.AvatarRuta).HasMaxLength(255);
                entity.Property(e => e.Biografia).HasMaxLength(500);

                // Email único (igual que la restricción UNIQUE en SQL)
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}