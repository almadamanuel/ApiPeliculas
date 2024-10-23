using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos
{
    public class ObtenerCategoriaDto
    {

        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "campo obligatorio")]
        [MaxLength(60,ErrorMessage ="Longitud maxima de 60 caracteres")]
        public string Nombre { get; set; }

        
    }
}
