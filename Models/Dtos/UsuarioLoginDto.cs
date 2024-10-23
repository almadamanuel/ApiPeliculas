using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos
{
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage ="Campo obligatorio")]
        public string NombreUsuario { get; set; }
       
        [Required(ErrorMessage = "Campo obligatorio")]
        public string Password { get; set; }
        
    }
}
