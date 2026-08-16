using Microsoft.EntityFrameworkCore;
using SitiosApi.Application.Interfaces;
using SitiosApi.Domain.Entities;

namespace SitiosApi.Infrastructure.Persistence
{
    /// <summary>
    /// Implementación concreta del repositorio usando EF Core + SQLite.
    /// Implementa la interfaz definida en Application (inversión de dependencias).
    /// </summary>
    public class SitioRepository : ISitioRepository
    {
        private readonly AppDbContext _context;

        public SitioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sitio>> GetAllAsync()
        {
            return await _context.Sitios
                .AsNoTracking()
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Sitio?> GetByIdAsync(int id)
        {
            return await _context.Sitios.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sitio> AddAsync(Sitio sitio)
        {
            _context.Sitios.Add(sitio);
            await _context.SaveChangesAsync();
            return sitio;
        }

        public async Task<bool> UpdateAsync(Sitio sitio)
        {
            var existente = await _context.Sitios.FirstOrDefaultAsync(s => s.Id == sitio.Id);
            if (existente is null) return false;

            existente.Descripcion = sitio.Descripcion;
            existente.Latitud = sitio.Latitud;
            existente.Longitud = sitio.Longitud;
            existente.FotografiaBase64 = sitio.FotografiaBase64;
            existente.AudioBase64 = sitio.AudioBase64;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existente = await _context.Sitios.FirstOrDefaultAsync(s => s.Id == id);
            if (existente is null) return false;

            _context.Sitios.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
