using MyMedia.Shared.DTOs;
using MyMedia.Shared.Entities;

namespace MyMedia.Shared.Interfaces
{
    public interface ICarrinhoService
    {
        event Action OnChange;

        // Agora retornam Task porque vamos esperar pelo LocalStorage
        Task<bool> AdicionarItem(Produto produto);
        Task<List<CarrinhoItem>> ObterItens();
        Task<int> ContarItens();
        Task<decimal> TotalPagar();

        Task<bool> FinalizarEncomenda(EncomendaDto encomenda);
        Task<List<ItemCarrinhoDto>> ObterItensDto();

        Task RemoverItem(int produtoId);
        Task DiminuirQuantidade(int produtoId);
    }
}