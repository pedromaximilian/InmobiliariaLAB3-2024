using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inmobiliaria.Models
{
    public class Propietario
    {
        [Key]
        [JsonIgnore]
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Required]
        [MaxLength(40)]
        public string Nombre { get; set; }
        [Required]
        [MaxLength(40)]
        public string Apellido { get; set; }
        [Required]
        [MaxLength(20)]
        public string Dni { get; set; }
        [Display(Name = "Teléfono")]
        [MaxLength(30)]
        public string Telefono { get; set; }
        [Required, EmailAddress]
        [MaxLength(320)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public override string ToString()
        {
            //return $"{Apellido}, {Nombre}";
            return $"{Nombre} {Apellido}";
        }
    }
}
