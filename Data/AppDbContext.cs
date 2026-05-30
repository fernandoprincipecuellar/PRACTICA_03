using Microsoft.EntityFrameworkCore;
using PRACTICA_03.Models;

namespace PRACTICA_03.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Tarea> Tareas { get; set; } = null!;
    }
}
