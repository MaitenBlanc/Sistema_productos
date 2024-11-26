using Microsoft.EntityFrameworkCore;
using prueba_addaccion.Models;

namespace prueba_addaccion.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public required DbSet<Product> Products { get; set; }
        public required DbSet<ProductCategory> ProductCategories { get; set; }

        // Configuración de las relaciones entre las tablas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
