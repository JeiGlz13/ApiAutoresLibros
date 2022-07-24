using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTO
{
    public class LibroDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 100)]
        public string Título { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        // public List<ComentarioDTO> Comentarios { get; set; }

    }
}
