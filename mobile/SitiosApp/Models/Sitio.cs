namespace SitiosApp.Models
{
    public class Sitio
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string? FotografiaBase64 { get; set; }
        public string? AudioBase64 { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class SitioCreateDto
    {
        public string Descripcion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string? FotografiaBase64 { get; set; }
        public string? AudioBase64 { get; set; }
    }
}
