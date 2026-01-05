using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMedia.API.Data;
using MyMedia.Shared.Entities;
using System.Security.Claims;

namespace MyMedia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProdutosController> _logger;

        public ProdutosController(ApplicationDbContext context, ILogger<ProdutosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. GET: api/produtos (PÚBLICO)
        // ⚠️ IMPORTANTE: Aqui TEMOS de filtrar por Ativo == true.
        // Se não filtrarmos, os clientes veem produtos que ainda não foram aprovados.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos([FromQuery] bool me = false)
        {
            if (me)
            {
                // Requires authentication and role Fornecedor
                if (!User.Identity?.IsAuthenticated ?? true) return Unauthorized();

                // Resolve user id from common claim names
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("nameid")
                    ?? User.FindFirstValue("sub");

                if (userId == null) return Unauthorized();

                _logger.LogInformation("Returning products for user {UserId}", userId);

                return await _context.Produtos
                    .Where(p => p.FornecedorId == userId)
                    .Include(p => p.Categoria)
                    .Include(p => p.ModoDisponibilizacao)
                    .OrderByDescending(p => p.Id)
                    .ToListAsync();
            }

            // Public listing
            return await _context.Produtos
                .Where(p => p.Ativo == true) // <--- Filtro de segurança para o público
                .Include(p => p.Categoria)
                .Include(p => p.ModoDisponibilizacao)
                // Não precisamos de incluir o Fornecedor na lista pública para poupar dados, 
                // mas podes deixar se quiseres mostrar "Vendido por X".
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        // 2. GET: api/produtos/5 (DETALHES)
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor)
                .Include(p => p.ModoDisponibilizacao)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null) return NotFound();

            return produto;
        }

        // 3. GET: api/produtos/categoria/5 (FILTRO)
        [HttpGet("categoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosPorCategoria(int categoriaId)
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.ModoDisponibilizacao)
                .Where(p => p.Ativo == true && p.CategoriaId == categoriaId)
                .ToListAsync();
        }

        // 4. POST: CRIAR PRODUTO
        [HttpPost]
        [Authorize(Roles = "Fornecedor, Administrador")]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("nameid")
                ?? User.FindFirstValue("sub");
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");

            // Regra: O dono é quem cria
            produto.FornecedorId = userId!;

            // Regra: Fornecedores criam produtos "Pendentes" (Inativos)
            if (userRole == "Fornecedor")
            {
                produto.Ativo = false;
            }
            // Admin pode criar já Ativo se quiser (vem do frontend)

            if (produto.PrecoBase <= 0) return BadRequest("O preço deve ser maior que zero.");

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduto", new { id = produto.Id }, produto);
        }

        // 5. PUT: EDITAR PRODUTO (Faltava este!)
        [HttpPut("{id}")]
        [Authorize(Roles = "Fornecedor, Administrador")]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if (id != produto.Id) return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("nameid")
                ?? User.FindFirstValue("sub");
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");

            // REGRA DE SEGURANÇA:
            // Se for Fornecedor, temos de verificar se o produto é mesmo dele antes de deixar gravar.
            if (userRole == "Fornecedor")
            {
                // Vamos à BD ver de quem é o produto original
                var produtoOriginal = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

                if (produtoOriginal == null || produtoOriginal.FornecedorId != userId)
                {
                    return Unauthorized("Não tens permissão para editar este produto.");
                }

                // Opcional: Se editares, volta a ficar pendente? 
                // Se sim: produto.Ativo = false;
                // Se não, mantém o estado que vier do front (cuidado para ele não se auto-aprovar).
                // Por segurança, podes forçar:
                produto.Ativo = false; // Força re-aprovação após edição
            }

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Produtos.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // 6. DELETE: APAGAR PRODUTO (Faltava este!)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Fornecedor, Administrador")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("nameid")
                ?? User.FindFirstValue("sub");
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");

            // Só deixa apagar se for Admin OU se for o dono do produto
            if (userRole == "Fornecedor" && produto.FornecedorId != userId)
            {
                return Unauthorized("Não podes apagar produtos de outros fornecedores.");
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}