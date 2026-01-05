using MyMedia.Shared.Entities;

namespace MyMedia.Shared.Interfaces
{
    public interface IProdutoService
    {
        // Apenas métodos GET (Mostrar)
        Task<List<Produto>> GetProdutosAsync();
        Task<Produto?> GetProdutoAsync(int id);
        Task<List<Produto>> GetProdutosPorCategoriaAsync(int categoriaId);

        Task<List<Produto>> GetMeusProdutos();
        Task DeleteProduto(int id);

        Task<Produto> GetProduto(int id);
        Task<Produto> CreateProduto(Produto produto);
        Task<Produto> UpdateProduto(Produto produto);
    }
}