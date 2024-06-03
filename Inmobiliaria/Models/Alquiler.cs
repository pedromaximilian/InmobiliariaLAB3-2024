using Microsoft.CodeAnalysis.Options;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Inmobiliaria.Models
{
    public class Alquiler
    {
        [Key]
        public int Id { get; set; }
        public double Precio { get; set; }
        [Display(Name = "Fecha Inicio")]
        public DateTime FechaInicio { get; set; }
        [Display(Name = "Fecha Fin")]
        public DateTime FechaFin { get; set; }
        [JsonIgnore]
        public int InquilinoId { get; set; }
        [ForeignKey(nameof(InquilinoId))]
        public Inquilino? Inquilino { get; set; }
        [JsonIgnore]
        public int InmuebleId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(InmuebleId))]
        public Inmueble? Inmueble { get; set; }
    }
}
