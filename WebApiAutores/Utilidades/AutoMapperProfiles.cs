using AutoMapper;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();

            CreateMap<Autor, AutorDTO>();

            CreateMap<Autor, AutorDTOConLibros>()
                    .ForMember(autorDTO => autorDTO.Libros, options => options.MapFrom(MapAutorDTOLibros));

            CreateMap<LibroCreacionDTO, Libro>()
                    .ForMember(libro => libro.AutoresLibros, options => options.MapFrom(MapAutoresLibros));

            CreateMap<Libro, LibroDTO>();

            CreateMap<Libro, LibroDTOConAutores>()
                    .ForMember(LibroDTO => LibroDTO.Autores, options => options.MapFrom(MapLibroDTOAutores));
            CreateMap<LibroPatchDTO, Libro>().ReverseMap();

            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
        }
        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();

            if (autor.AutoresLibros == null){return resultado;}

            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO()
                {
                    Id = autorLibro.LibroId,
                    Título = autorLibro.Libro.Título
                });
            }

            return resultado;
        }
        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();
            if (libro.AutoresLibros == null){return resultado;}

            foreach (var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }

            return resultado;
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();
            if (libroCreacionDTO.AutoresIds == null){return resultado;}

            foreach (var autorId in libroCreacionDTO.AutoresIds) 
            { 
                resultado.Add(new AutorLibro() { 
                    AutorId = autorId 
                });
            }

            return resultado;
        }
    }
}
