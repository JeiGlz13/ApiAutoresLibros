using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V2
{
    [ApiController]
    [Route("api/v2/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    //Filtro a nivel del controlador [Authorize]
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public AutoresController(ApplicationDbContext context, 
            IMapper mapper, 
            IAuthorizationService authorizationService)
        {
            _context = context;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerAutoresV2")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            var autores = await _context.Autores.ToListAsync();
            autores.ForEach(autor => autor.Nombre = autor.Nombre.ToUpper());
            return _mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id:int}", Name = "obtenerAutorV2")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> GetAutor(int id)
        {
            var autor = await _context.Autores
                        .Include(autorBD => autorBD.AutoresLibros)
                        .ThenInclude(autorLibroBD => autorLibroBD.Libro)
                        .FirstOrDefaultAsync(autorBD => autorBD.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<AutorDTOConLibros>(autor);
            return dto;
        }

        

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombreV2")]
        public async Task<ActionResult<List<AutorDTO>>> GetAutorByNombre([FromRoute] string nombre)
        {
            var autores = await _context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return _mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost(Name = "crearAutorV2")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacion)
        {
            var existeAutorConElMismoNombre = await _context.Autores.AnyAsync(x => x.Nombre == autorCreacion.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacion.Nombre}");
            }

            var autor = _mapper.Map<Autor>(autorCreacion);
             
            _context.Add(autor);
            await _context.SaveChangesAsync();

            var autorDTO = _mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutor", new {id = autor.Id}, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutorV2")]
        public async Task<ActionResult> Put([FromBody] AutorCreacionDTO autorCreacionDTO, [FromRoute] int id)
        {
            var existe = await _context.Autores.AnyAsync(autorBD => autorBD.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = _mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;


            _context.Update(autor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "borrarAutorV2")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            _context.Remove(new Autor { Id = id });
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
