using Inmobiliaria.Models;
using Microsoft.EntityFrameworkCore;

namespace Inmobiliaria.Context
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
            
        }

        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Inmueble> Inmuebles { get; set; }
    }
}
