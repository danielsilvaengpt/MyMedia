using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMedia.API.Data;
using MyMedia.Shared.Entities;

namespace MyMedia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // 🔒 Importante: Só Admin e Funcionários podem mexer nisto
    //[Authorize(Roles = "Admin,Funcionario")]
    public class ModosDisponibilizacaoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ModosDisponibilizacaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ModosDisponibilizacao
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModoDisponibilizacao>>> GetModos()
        {
            return await _context.ModosDisponibilizacao.ToListAsync();
        }

        // GET: api/ModosDisponibilizacao/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModoDisponibilizacao>> GetModo(int id)
        {
            var modo = await _context.ModosDisponibilizacao.FindAsync(id);

            if (modo == null)
            {
                return NotFound();
            }

            return modo;
        }

        // POST: api/ModosDisponibilizacao
        [HttpPost]
        public async Task<ActionResult<ModoDisponibilizacao>> PostModo(ModoDisponibilizacao modo)
        {
            _context.ModosDisponibilizacao.Add(modo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModo", new { id = modo.Id }, modo);
        }

        // PUT: api/ModosDisponibilizacao/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModo(int id, ModoDisponibilizacao modo)
        {
            if (id != modo.Id)
            {
                return BadRequest();
            }

            _context.Entry(modo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModoExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/ModosDisponibilizacao/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModo(int id)
        {
            var modo = await _context.ModosDisponibilizacao.FindAsync(id);
            if (modo == null)
            {
                return NotFound();
            }

            _context.ModosDisponibilizacao.Remove(modo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ModoExists(int id)
        {
            return _context.ModosDisponibilizacao.Any(e => e.Id == id);
        }
    }
}