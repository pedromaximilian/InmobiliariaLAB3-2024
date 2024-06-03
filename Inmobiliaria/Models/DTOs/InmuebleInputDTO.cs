using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Inmobiliaria.Models.DTOs
{
    public class InmuebleInputDTO
    {
        public int Id { get; set; }
        [Required]
        [JsonIgnore]
        public int TipoId { get; set; }
        [Required]
        [JsonIgnore]
        public int UsoId { get; set; }
        [Required]
        public string Direccion { get; set; }
        [Required]
        public int Ambientes { get; set; }
        public decimal Precio { get; set; }
        public bool Disponible { get; set; }
        public string? ImagenUrl { get; set; }
        [JsonIgnore]
        [NotMapped]
        public IFormFile Imagen { get; set; }

    }
}
