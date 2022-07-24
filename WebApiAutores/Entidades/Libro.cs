using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 100)]
        public string Título { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public List<Comentario>Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
    }
}
