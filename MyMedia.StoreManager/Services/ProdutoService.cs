using Microsoft.EntityFrameworkCore;
using MyMedia.Shared.Entities;
using MyMedia.Shared.Entities;
using MyMedia.StoreManager.Data;

namespace MyMedia.StoreManager.Services
{
    public class ProdutoService
    {
        private readonly ApplicationDbContext _context;

        public ProdutoService(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- LEITURA (Listagens) ---

        // Obtém todos os produtos carregando as relações (Categoria e Fornecedor)
        // Necessário para preencher a grelha de gestão[cite: 156].
        public async Task<List<Produto>> GetProdutosAsync()
        {
            return await _context.Produtos
                .Include(p => p.Categoria)      // Carrega o nome da Categoria
                .Include(p => p.Fornecedor)
                .Include(p => p.ModoDisponibilizacao)
                .OrderByDescending(p => p.Id)   // Mais recentes primeiro
                .ToListAsync();
        }

       // Obtém um produto específico para Edição[cite: 158].
        public async Task<Produto?> GetProdutoByIdAsync(int id)
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // --- ESCRITA (Criar e Editar) ---

        // Cria um novo produto ou atualiza um existente[cite: 157, 158].
        // Nota: O Preço Final é calculado automaticamente na entidade Produto baseada no PreçoBase + Margem[cite: 164].
        public async Task SaveProdutoAsync(Produto produto)
        {
            if (produto.Id == 0)
            {
                // Novo Registo
                // Define a data de registo se não estiver definida
                if (produto.DataRegisto == DateTime.MinValue)
                    produto.DataRegisto = DateTime.Now;

                _context.Produtos.Add(produto);
            }
            else
            {
                // Atualização de Registo existente
                _context.Produtos.Update(produto);
            }

            await _context.SaveChangesAsync();
        }

        // --- ELIMINAÇÃO (Com Regra de Negócio) ---

        // "Apagar 'fisicamente' os registos... (apenas se não existirem vendas)".
        public async Task<bool> DeleteProdutoAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return false;

            // VERIFICAÇÃO CRÍTICA:
            // Vai à tabela ItensEncomenda ver se este ID de produto já aparece nalguma venda.
            bool temVendas = await _context.ItensEncomenda.AnyAsync(i => i.ProdutoId == id);

            if (temVendas)
            {
                // Retorna falso para avisar a UI que não pode apagar
                return false;
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- GESTÃO DE ESTADOS ---

        // "Ativar ou inativar o registo de produtos... colocando-os ou retirando como visíveis"[cite: 160, 162].
        public async Task ToggleVisibilidadeAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                produto.Ativo = !produto.Ativo; // Inverte (True -> False, False -> True)
                await _context.SaveChangesAsync();
            }
        }

        // Alternar entre "Para Venda" e "Apenas Listagem"[cite: 171].
        public async Task ToggleParaVendaAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                produto.ParaVenda = !produto.ParaVenda;
                await _context.SaveChangesAsync();
            }
        }

        // --- MÉTODOS AUXILIARES PARA OS DROPDOWNS ---

        // Devolve todas as categorias para preencher o <select>
        public async Task<List<Categoria>> GetCategoriasAsync()
        {
            return await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
        }

        // Devolve todos os utilizadores para escolher o Fornecedor
        // Nota: Num cenário real filtrarias apenas pelo Role "Fornecedor", 
        // mas para simplificar trazemos todos para poderes testar já.
        public async Task<List<ApplicationUser>> GetFornecedoresAsync()
        {
            return await _context.Users.OrderBy(u => u.UserName).ToListAsync();
        }
    }
}