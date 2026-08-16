namespace SitiosApi.Application.DTOs
{
    /// <summary>DTO que se devuelve al cliente (GET).</summary>
    public class SitioDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string? FotografiaBase64 { get; set; }
        public string? AudioBase64 { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    /// <summary>DTO para crear un sitio (CREATE / POST inicial).</summary>
    public class SitioCreateDto
    {
        public string Descripcion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string? FotografiaBase64 { get; set; }
        public string? AudioBase64 { get; set; }
    }

    /// <summary>DTO para actualizar un sitio existente (UPDATE).</summary>
    public class SitioUpdateDto
    {
        public string Descripcion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string? FotografiaBase64 { get; set; }
        public string? AudioBase64 { get; set; }
    }
}
