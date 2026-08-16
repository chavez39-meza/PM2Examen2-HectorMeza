using SitiosApi.Application.DTOs;
using SitiosApi.Application.Interfaces;
using SitiosApi.Domain.Entities;

namespace SitiosApi.Application.Services
{
    /// <summary>
    /// Aquí vive la lógica de negocio (validaciones, mapeo Entity <-> DTO).
    /// No sabe nada de EF Core ni de HTTP: solo depende de la interfaz del repositorio.
    /// </summary>
    public class SitioService : ISitioService
    {
        private readonly ISitioRepository _repository;

        public SitioService(ISitioRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SitioDto>> GetAllAsync()
        {
            var sitios = await _repository.GetAllAsync();
            return sitios.Select(MapToDto).ToList();
        }

        public async Task<SitioDto?> GetByIdAsync(int id)
        {
            var sitio = await _repository.GetByIdAsync(id);
            return sitio is null ? null : MapToDto(sitio);
        }

        public async Task<SitioDto> CreateAsync(SitioCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Descripcion))
                throw new ArgumentException("La descripción es obligatoria.");

            var sitio = new Sitio
            {
                Descripcion = dto.Descripcion,
                Latitud = dto.Latitud,
                Longitud = dto.Longitud,
                FotografiaBase64 = dto.FotografiaBase64,
                AudioBase64 = dto.AudioBase64,
                FechaCreacion = DateTime.UtcNow
            };

            var creado = await _repository.AddAsync(sitio);
            return MapToDto(creado);
        }

        public async Task<bool> UpdateAsync(int id, SitioUpdateDto dto)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente is null) return false;

            existente.Descripcion = dto.Descripcion;
            existente.Latitud = dto.Latitud;
            existente.Longitud = dto.Longitud;

            // Si no mandan foto/audio nuevo, conservamos el que ya había.
            if (!string.IsNullOrEmpty(dto.FotografiaBase64))
                existente.FotografiaBase64 = dto.FotografiaBase64;

            if (!string.IsNullOrEmpty(dto.AudioBase64))
                existente.AudioBase64 = dto.AudioBase64;

            return await _repository.UpdateAsync(existente);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static SitioDto MapToDto(Sitio sitio) => new()
        {
            Id = sitio.Id,
            Descripcion = sitio.Descripcion,
            Latitud = sitio.Latitud,
            Longitud = sitio.Longitud,
            FotografiaBase64 = sitio.FotografiaBase64,
            AudioBase64 = sitio.AudioBase64,
            FechaCreacion = sitio.FechaCreacion
        };
    }
}
