using Inmobiliaria.Models;
using Microsoft.EntityFrameworkCore;

namespace Inmobiliaria.Context
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la restricción única para el campo Email en la tabla Propietario
            modelBuilder.Entity<Propietario>()
                .HasIndex(p => p.Email)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Inmueble> Inmuebles { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Alquiler> Alquilers { get; set; }
        public DbSet<Inquilino> Inquilinos { get; set; }
        public DbSet<Tipo> Tipo { get; set; }
        public DbSet<Uso> Uso { get; set; }

    }
}
