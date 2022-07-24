using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.DTO
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 20, ErrorMessage = "{El campo {0} no puede tener más de {1} caracteres]")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
    }
}
