using MyMedia.Shared.Entities;

namespace MyMedia.Shared.Interfaces
{
    public interface ICategoriaService
    {
        // Contrato: Apenas precisamos de buscar a lista de categorias
        Task<List<Categoria>> GetCategoriasAsync();
    }
}