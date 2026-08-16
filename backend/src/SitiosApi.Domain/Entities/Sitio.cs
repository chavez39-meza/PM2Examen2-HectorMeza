namespace SitiosApi.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio "Sitio". No depende de EF Core, ni de ASP.NET,
    /// ni de ninguna otra capa: eso es lo que hace que sea "Clean Architecture".
    /// </summary>
    public class Sitio
    {
        public int Id { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        public double Latitud { get; set; }

        public double Longitud { get; set; }

        /// <summary>
        /// Foto guardada como Base64 (string). En la base de datos se guarda
        /// como texto largo (TEXT / nvarchar(max)).
        /// </summary>
        public string? FotografiaBase64 { get; set; }

        /// <summary>
        /// Nota de voz guardada como Base64 (string).
        /// </summary>
        public string? AudioBase64 { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
