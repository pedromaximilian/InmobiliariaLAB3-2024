using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inmobiliaria.Models
{
    public class Inmueble
    {
        public int Id { get; set; }

        [Required]
        [JsonIgnore]
        public int PropietarioId { get; set; }

        [Required]
        [JsonIgnore]
        public int TipoId { get; set; }

        [Required]
        [JsonIgnore]
        public int UsoId { get; set; }

        [Required]
        public string Direccion { get; set; } = "";

        [Required]
        public int Ambientes { get; set; }

        public decimal Precio { get; set; }

        public bool Disponible { get; set; }
        public string? ImagenUrl { get; set; }
        [JsonIgnore]
        [NotMapped]
        public IFormFile Imagen { get; set; }

        [ForeignKey(nameof(PropietarioId))]
        [JsonIgnore]
        public Propietario? Propietario { get; set; }

        [ForeignKey(nameof(TipoId))]
        public Tipo? Tipo { get; set; }

        [ForeignKey(nameof(UsoId))]
        public Uso? Uso { get; set; }
    }
}
