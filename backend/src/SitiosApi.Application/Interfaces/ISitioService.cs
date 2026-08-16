using SitiosApi.Application.DTOs;

namespace SitiosApi.Application.Interfaces
{
    public interface ISitioService
    {
        Task<List<SitioDto>> GetAllAsync();
        Task<SitioDto?> GetByIdAsync(int id);
        Task<SitioDto> CreateAsync(SitioCreateDto dto);
        Task<bool> UpdateAsync(int id, SitioUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
