using Microsoft.EntityFrameworkCore;
using SitiosApi.Domain.Entities;

namespace SitiosApi.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Sitio> Sitios => Set<Sitio>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sitio>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Descripcion).IsRequired().HasMaxLength(500);
                // TEXT sin límite de tamaño para poder guardar el Base64 de foto y audio
                entity.Property(s => s.FotografiaBase64).HasColumnType("TEXT");
                entity.Property(s => s.AudioBase64).HasColumnType("TEXT");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
