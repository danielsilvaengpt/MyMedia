using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMedia.API.Data;
using MyMedia.Shared.DTOs;
using MyMedia.Shared.Entities;
using System.Security.Claims;

namespace MyMedia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncomendasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EncomendasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")] // 🔒 Só Clientes podem encomendar!
        public async Task<IActionResult> CriarEncomenda([FromBody] EncomendaDto dto)
        {
            // 1. Descobrir quem é o utilizador através do Token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue("nameid")
                         ?? User.FindFirstValue("sub");

            if (userId == null) return Unauthorized();

            // 2. Criar a Encomenda (Cabeçalho)
            var novaEncomenda = new Encomenda
            {
                ClienteId = userId,
                DataEncomenda = DateTime.Now,
                MetodoPagamento = dto.MetodoPagamento,
                MoradaEntrega = dto.MoradaEntrega,
                ValorTotal = dto.ValorTotal,
                Estado = "Pendente" // Começa sempre pendente
            };

            // 3. Adicionar os produtos (Detalhes)
            // Nota: Num sistema real, deverias ir buscar o preço à BD para evitar fraudes,
            // mas para este exemplo vamos usar o que vem do DTO.
            foreach (var item in dto.Itens)
            {
                novaEncomenda.Detalhes.Add(new DetalheEncomenda
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                });

                // Opcional: Abater stock aqui se quiseres
            }

            _context.Encomendas.Add(novaEncomenda);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Encomenda criada com sucesso!", Id = novaEncomenda.Id });
        }


        [HttpGet("minhas")]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<List<Encomenda>>> GetMinhasEncomendas()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue("nameid")
                         ?? User.FindFirstValue("sub");

            if (userId == null) return Unauthorized();

            var encomendas = await _context.Encomendas
                .Include(e => e.Detalhes) // Inclui os itens da encomenda
                .ThenInclude(d => d.Produto) // Inclui os dados do produto (título, imagem)
                .Where(e => e.ClienteId == userId)
                .OrderByDescending(e => e.DataEncomenda)
                .ToListAsync();

            return encomendas;
        }


        [HttpGet("vendas")]
        [Authorize(Roles = "Fornecedor")]
        public async Task<ActionResult<List<VendaDto>>> GetHistoricoVendas()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue("nameid")
                         ?? User.FindFirstValue("sub");

            if (userId == null) return Unauthorized();

            // A Lógica: Vamos buscar todas as encomendas que tenham pelo menos um produto meu.
            // Depois, "achatamos" a lista para pegar apenas nas linhas que me interessam.

            var vendas = await _context.Encomendas
                .Where(e => e.Detalhes.Any(d => d.Produto.FornecedorId == userId)) // Filtra encomendas relevantes
                .SelectMany(e => e.Detalhes.Where(d => d.Produto.FornecedorId == userId) // Seleciona só os meus itens
                    .Select(d => new VendaDto
                    {
                        EncomendaId = e.Id,
                        Data = e.DataEncomenda,
                        Produto = d.Produto.Titulo,
                        Quantidade = d.Quantidade,
                        PrecoUnitario = d.PrecoUnitario,
                        // Tenta apanhar o nome, se for null usa o email
                        NomeCliente = e.Cliente.NomeCompleto ?? e.Cliente.Email ?? "Desconhecido",
                        EstadoEntrega = e.Estado
                    }))
                .OrderByDescending(v => v.Data)
                .ToListAsync();

            return vendas;
        }
    }


}