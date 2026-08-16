using SitiosApi.Domain.Entities;

namespace SitiosApi.Application.Interfaces
{
    /// <summary>
    /// Contrato del repositorio. La capa Application define QUÉ se necesita,
    /// la capa Infrastructure decide CÓMO se hace (EF Core, SQLite, etc).
    /// Esto es el principio de inversión de dependencias de Clean Architecture.
    /// </summary>
    public interface ISitioRepository
    {
        Task<List<Sitio>> GetAllAsync();
        Task<Sitio?> GetByIdAsync(int id);
        Task<Sitio> AddAsync(Sitio sitio);
        Task<bool> UpdateAsync(Sitio sitio);
        Task<bool> DeleteAsync(int id);
    }
}
