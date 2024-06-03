using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class Inquilino
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required]
        [MaxLength(40)]
        public string Nombre { get; set; }
        [Required]
        [MaxLength(40)]
        public string Apellido { get; set; }
        [Required]
        [MaxLength(8)]
        public string Dni { get; set; }
        [MaxLength(30)]
        public string Telefono { get; set; }
        [Required]
        [MaxLength(50)]
        public string Direccion { get; set; }
    }
}
