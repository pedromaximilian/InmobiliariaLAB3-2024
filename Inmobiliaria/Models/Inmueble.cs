using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class Inmueble
    {
        [Display(Name = "Nº")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Dirección")]
        [MaxLength(40)]
        public string Direccion { get; set; } = "";
        [Required]
        [MaxLength(4)]
        public int Ambientes { get; set; }
        [Required]
        [MaxLength(15)]
        public int Tipo { get; set; }
        [Required]
        [MaxLength(15)]
        public int Uso { get; set; }
        public double Precio { get; set; }

        public bool Activo { get; set; }

        public string? Foto { get; set; }

        [Display(Name = "Dueño")]
        public int PropietarioId { get; set; }
        [ForeignKey(nameof(PropietarioId))]
        public Propietario? Propietario { get; set; }
    }
}
