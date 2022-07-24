 using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Libro>>> GetAll()
        {
            return await _context.Libros.ToListAsync();
        }

        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await _context.Libros
                .Include(libroBD => libroBD.AutoresLibros)
                .ThenInclude(autorLibroBD => autorLibroBD.Autor)
                //.Include(libroBD => libroBD.Comentarios)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return _mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost(Name = "agregarLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresIds = await _context.Autores
                                .Where(autorBD => libroCreacionDTO.AutoresIds.Contains(autorBD.Id))
                                .Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = _mapper.Map<Libro>(libroCreacionDTO);

            AsignarOrdenAutores(libro);

            _context.Add(libro);
            await _context.SaveChangesAsync();

            var libroDTO = _mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroBD = await _context.Libros
                                .Include(x => x.AutoresLibros)
                                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroBD == null)
            {
                return NotFound();
            }

            libroBD = _mapper.Map(libroCreacionDTO, libroBD);
            AsignarOrdenAutores(libroBD);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "actualizarConPatch")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var libroBD = await _context.Libros.FirstOrDefaultAsync(x=> x.Id == id);
            if (libroBD== null)
            {
                return NotFound();
            }

            var libroDTO = _mapper.Map<LibroPatchDTO>(libroBD);
            patchDocument.ApplyTo(libroDTO, ModelState);

            var isValid = TryValidateModel(libroDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(libroDTO, libroBD);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "eliminarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Libros.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            _context.Remove(new Libro { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }
    }
}
