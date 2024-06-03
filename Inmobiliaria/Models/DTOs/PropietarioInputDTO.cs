using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models.DTOs
{
    public class PropietarioDTO
    {

        [Required]
        [MaxLength(40)]
        public string Nombre { get; set; }
        [Required]
        [MaxLength(40)]
        public string Apellido { get; set; }
        [Required]
        [MaxLength(20)]
        public string Dni { get; set; }
        [MaxLength(30)]
        public string Telefono { get; set; }
        [Required, EmailAddress]
        [MaxLength(320)]
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
