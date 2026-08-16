using System.Net.Http.Json;
using SitiosApp.Models;

namespace SitiosApp.Services
{
    /// <summary>
    /// Encapsula todas las llamadas HTTP al backend (CRUD).
    /// </summary>
    public class ApiService
    {
        private readonly HttpClient _http;

        // IMPORTANTE: cambia esta URL según dónde corras el backend:
        //  - Emulador Android           -> http://10.0.2.2:5100/api/
        //  - Dispositivo físico Android -> http://TU_IP_DE_RED_LOCAL:5100/api/
        //  - Windows / iOS Simulator    -> http://localhost:5100/api/
        public const string BaseUrl = "http://localhost:5100/api/";

        public ApiService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<List<Sitio>> GetSitiosAsync()
        {
            var resultado = await _http.GetFromJsonAsync<List<Sitio>>("sitios");
            return resultado ?? new List<Sitio>();
        }

        public async Task<Sitio?> CreateSitioAsync(SitioCreateDto dto)
        {
            var respuesta = await _http.PostAsJsonAsync("sitios", dto);
            respuesta.EnsureSuccessStatusCode();
            return await respuesta.Content.ReadFromJsonAsync<Sitio>();
        }

        public async Task<bool> UpdateSitioAsync(int id, SitioCreateDto dto)
        {
            var respuesta = await _http.PutAsJsonAsync($"sitios/{id}", dto);
            return respuesta.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteSitioAsync(int id)
        {
            var respuesta = await _http.DeleteAsync($"sitios/{id}");
            return respuesta.IsSuccessStatusCode;
        }
    }
}
