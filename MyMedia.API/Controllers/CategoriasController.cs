using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMedia.API.Data;
using MyMedia.Shared.Entities;

namespace MyMedia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/categorias
        // Este endpoint devolve a hierarquia para os menus do Frontend
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            return await _context.Categorias
                .Include(c => c.SubCategorias) // Carrega as filhas (para os frisos)
                .ToListAsync();
        }

        // GET: api/categorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.SubCategorias)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            return categoria;
        }
    }
}