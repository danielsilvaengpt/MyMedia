using Microsoft.EntityFrameworkCore;
using MyMedia.Shared.Entities;
using MyMedia.StoreManager.Components.Pages;
using MyMedia.StoreManager.Data;

namespace MyMedia.StoreManager.Services
{
    public class EncomendaService
    {
        private readonly ApplicationDbContext _context;

        public EncomendaService(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- LEITURA (Listagens) ---

        // Listar todas as vendas para a tabela de gestão
        // Inclui dados do Cliente para saber quem comprou
        public async Task<List<Encomenda>> GetEncomendasAsync()
        {
            return await _context.Encomendas
                .Include(e => e.Cliente) // Carrega o nome do Cliente
                .OrderByDescending(e => e.DataEncomenda) // Mais recentes primeiro
                .ToListAsync();
        }

        // Obter detalhes de uma encomenda específica
        // CRÍTICO: Carrega os Detalhes e os Produtos dentro dos detalhes para ver o que foi comprado
        public async Task<Encomenda?> GetEncomendaByIdAsync(int id)
        {
            return await _context.Encomendas
                .Include(e => e.Cliente)
                .Include(e => e.Detalhes)
                    .ThenInclude(i => i.Produto) // Carrega o Produto dentro do Detalhe
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // --- AÇÕES DE GESTÃO (Estados e Stocks) ---

        // "Confirmar uma venda" 
        // Passa de "Pendente" para "Confirmada" (ou "Em Processamento")
        public async Task ConfirmarEncomendaAsync(int id)
        {
            var encomenda = await _context.Encomendas.FindAsync(id);
            if (encomenda != null && encomenda.Estado == "Pendente")
            {
                encomenda.Estado = "Confirmada";
                await _context.SaveChangesAsync();
            }
        }

        // "Rejeitar uma venda" 
        public async Task RejeitarEncomendaAsync(int id)
        {
            var encomenda = await _context.Encomendas.FindAsync(id);
            if (encomenda != null)
            {
                encomenda.Estado = "Anulada";
                await _context.SaveChangesAsync();
            }
        }

       // "Gerir os pagamentos (simular pagamento)" 
        public async Task SimularPagamentoAsync(int id)
        {
            var encomenda = await _context.Encomendas.FindAsync(id);
            if (encomenda != null && encomenda.Estado != "Anulada")
            {
                encomenda.Estado = "Pago";
                await _context.SaveChangesAsync();
            }
        }

        // "Expedir os produtos ao cliente (simular expedição)" 
        // "Actualizar os stocks" 
        public async Task ExpedirEncomendaAsync(int id)
        {
            // Precisamos dos detalhes para baixar o stock dos produtos correspondentes
            var encomenda = await _context.Encomendas
                .Include(e => e.Detalhes)
                .ThenInclude(d => d.Produto)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (encomenda != null && encomenda.Estado == "Pago")
            {
                // 1. Atualizar Stocks
                foreach (var item in encomenda.Detalhes)
                {
                    if (item.Produto != null)
                    {
                        item.Produto.Stock -= item.Quantidade;

                        // Segurança básica: não deixar stock negativo
                        if (item.Produto.Stock < 0)
                        {
                            item.Produto.Stock = 0;
                            item.Produto.ParaVenda = false;
                        }
                    }
                }

                // 2. Mudar estado para Expedido
                encomenda.Estado = "Expedido";

                await _context.SaveChangesAsync();
            }
        }
    }
}