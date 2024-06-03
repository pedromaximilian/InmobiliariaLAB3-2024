using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Inmobiliaria.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Nro Pago")]
        public int NroPago { get; set; }
        public DateTime Fecha { get; set; }
        public double Importe { get; set; }
        public int AlquilerId { get; set; }
        [ForeignKey(nameof(AlquilerId))]
        [JsonIgnore]
        public Alquiler? Alquiler { get; set; }
        
    }
}
