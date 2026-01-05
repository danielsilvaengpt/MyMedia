using Microsoft.EntityFrameworkCore;
using MyMedia.Shared.Entities;
using MyMedia.StoreManager.Data;

namespace MyMedia.StoreManager.Services
{
    public class CategoriaService
    {
        private readonly ApplicationDbContext _context;

        public CategoriaService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obter todas as categorias (incluindo a Categoria Pai para mostrar o nome dela)
        public async Task<List<Categoria>> GetCategoriasAsync()
        {
            return await _context.Categorias
                .Include(c => c.CategoriaPai) // Importante para ver "Rock (Filha de Música)"
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<Categoria?> GetCategoriaByIdAsync(int id)
        {
            return await _context.Categorias.FindAsync(id);
        }

        // Criar ou Editar
        public async Task SaveCategoriaAsync(Categoria categoria)
        {
            if (categoria.Id == 0)
            {
                _context.Categorias.Add(categoria);
            }
            else
            {
                _context.Categorias.Update(categoria);
            }
            await _context.SaveChangesAsync();
        }

        // Apagar (Com verificação de segurança)
        public async Task<bool> DeleteCategoriaAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            // REGRA: Não apagar se tiver Produtos associados
            bool temProdutos = await _context.Produtos.AnyAsync(p => p.CategoriaId == id);

            // REGRA: Não apagar se tiver Subcategorias (Filhas)
            bool temFilhas = await _context.Categorias.AnyAsync(c => c.CategoriaPaiId == id);

            if (temProdutos || temFilhas)
            {
                return false; // Bloqueia a eliminação
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}