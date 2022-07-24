using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ComentariosController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public ComentariosController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }
        [HttpGet(Name = "obtenerComentarios")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId,
            [FromQuery] PaginacionDTO paginacionDTO)
        {
            var existeLibro = await _context.Libros.AnyAsync(libroBD => libroBD.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var queryable = _context.Comentarios.Where(comentarioBD => comentarioBD.LibroId == libroId).AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            var comentarios = await queryable.OrderBy(comentario => comentario.Id).Paginar(paginacionDTO).ToListAsync();

            return _mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name = "obtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetById(int id)
        {
            var comentario = await _context.Comentarios
                                    .FirstOrDefaultAsync(comentarioBD => comentarioBD.Id == id);

            if (comentario == null)
            {
                return NotFound();
            }

            return _mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost(Name = "agregarComentario")]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await _userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var existeLibro = await _context.Libros.AnyAsync(libroBD => libroBD.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = _mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;
            _context.Add(comentario);
            await _context.SaveChangesAsync();

            var comentarioDTO = _mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("obtenerComentario", new {id = comentario.Id, libroId }, comentarioDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarComentario")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioDTO comentarioDTO)
        {
            var existeLibro = await _context.Libros.AnyAsync(libroBD => libroBD.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var existeComentario = await _context.Comentarios.AnyAsync(comentarioBD => comentarioBD.Id == id);

            if (!existeComentario)
            {
                return NotFound();
            }

            var comentario = _mapper.Map<Comentario>(comentarioDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;
            _context.Update(comentario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
