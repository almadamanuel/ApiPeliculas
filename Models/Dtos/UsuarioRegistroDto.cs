using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos
{
    public class UsuarioRegistroDto
    {
        [Required(ErrorMessage ="Campo obligatorio")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "Campo obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Campo obligatorio")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
