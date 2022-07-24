using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTO
{
    public class LibroPatchDTO
    {
        [Required]
        [StringLength(maximumLength: 100)]
        public string Título { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
