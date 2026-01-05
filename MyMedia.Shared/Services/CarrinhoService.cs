using Blazored.LocalStorage;
using MyMedia.Shared.DTOs; // Necessário para EncomendaDto e ItemCarrinhoDto
using MyMedia.Shared.Entities;
using MyMedia.Shared.Interfaces;
using System.Linq;

namespace MyMedia.Shared.Services
{
    public class CarrinhoService : ICarrinhoService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ApiClient _apiClient; // 👇1. Nova Injeção

        private List<CarrinhoItem> _carrinho = new();

        public event Action OnChange;

        // 👇 Adicionamos o ApiClient ao construtor
        public CarrinhoService(ILocalStorageService localStorage, ApiClient apiClient)
        {
            _localStorage = localStorage;
            _apiClient = apiClient;
        }

        public async Task<bool> AdicionarItem(Produto produto)
        {
            var carrinho = await _localStorage.GetItemAsync<List<CarrinhoItem>>("carrinho");
            if (carrinho == null) carrinho = new List<CarrinhoItem>();

            var item = carrinho.Find(x => x.Produto.Id == produto.Id);
            bool sucesso = true;

            if (item != null)
            {
                if (item.Quantidade < produto.Stock)
                {
                    item.Quantidade++;
                    item.Produto = produto; // Atualiza info do produto
                }
                else
                {
                    Console.WriteLine("Stock máximo atingido.");
                    sucesso = false;
                }
            }
            else
            {
                if (produto.Stock > 0)
                {
                    carrinho.Add(new CarrinhoItem { Produto = produto, Quantidade = 1 });
                }
                else
                {
                    sucesso = false;
                }
            }

            if (sucesso)
            {
                await _localStorage.SetItemAsync("carrinho", carrinho);
                OnChange?.Invoke();
            }

            return sucesso;
        }

        // Expose method named GetItens to match other pages
        public async Task<List<ItemCarrinhoDto>> GetItens()
        {
            var itens = await ObterItens();
            return itens.Select(i => new ItemCarrinhoDto
            {
                ProdutoId = i.Produto.Id,
                Titulo = i.Produto.Titulo,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.Produto.PrecoBase // Ou Preco com desconto se tiveres
            }).ToList();
        }

        public async Task<List<CarrinhoItem>> ObterItens()
        {
            try
            {
                var c = await _localStorage.GetItemAsync<List<CarrinhoItem>>("carrinho");
                _carrinho = c ?? new List<CarrinhoItem>();
            }
            catch
            {
                Console.WriteLine("Carrinho corrompido. A limpar...");
                await _localStorage.RemoveItemAsync("carrinho");
                _carrinho = new List<CarrinhoItem>();
            }
            return _carrinho;
        }

        // 👇 Novo método auxiliar para converter os itens para DTO (O Checkout precisa disto)
        public async Task<List<ItemCarrinhoDto>> ObterItensDto()
        {
            var itens = await ObterItens();
            return itens.Select(i => new ItemCarrinhoDto
            {
                ProdutoId = i.Produto.Id,
                Titulo = i.Produto.Titulo,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.Produto.PrecoBase // Ou Preco com desconto se tiveres
            }).ToList();
        }

        // 👇 2. Correção: Agora conta mesmo os itens
        public async Task<int> ContarItens()
        {
            var itens = await ObterItens();
            // Retorna a soma das quantidades (ex: 2 produtos X + 1 produto Y = 3 itens)
            return itens.Sum(x => x.Quantidade);
        }

        public async Task<decimal> TotalPagar()
        {
            var itens = await ObterItens();
            if (itens == null) return 0;
            return itens.Sum(x => x.Subtotal); // Assume que tens a propriedade Subtotal no CarrinhoItem
        }

        // 👇 3. Novo Método: Finalizar Encomenda
        public async Task<bool> FinalizarEncomenda(EncomendaDto encomenda)
        {
            // Envia para a API usando o método que criámos no ApiClient
            var sucesso = await _apiClient.EnviarEncomendaAsync(encomenda);

            if (sucesso)
            {
                // Se a API aceitou, limpamos o carrinho local
                await _localStorage.RemoveItemAsync("carrinho");
                _carrinho = new List<CarrinhoItem>();

                // Avisamos o layout que o carrinho está vazio (o badge passa a 0)
                OnChange?.Invoke();
            }

            return sucesso;
        }

        public async Task RemoverItem(int produtoId)
        {
            var carrinho = await _localStorage.GetItemAsync<List<CarrinhoItem>>("carrinho");
            if (carrinho == null) return;

            var item = carrinho.Find(x => x.Produto.Id == produtoId);
            if (item != null)
            {
                carrinho.Remove(item);
                await _localStorage.SetItemAsync("carrinho", carrinho);
                OnChange?.Invoke(); // Notifica a página para atualizar
            }
        }

        public async Task DiminuirQuantidade(int produtoId)
        {
            var carrinho = await _localStorage.GetItemAsync<List<CarrinhoItem>>("carrinho");
            if (carrinho == null) return;

            var item = carrinho.Find(x => x.Produto.Id == produtoId);
            if (item != null)
            {
                if (item.Quantidade > 1)
                {
                    item.Quantidade--;
                    await _localStorage.SetItemAsync("carrinho", carrinho);
                }
                else
                {
                    // Se a quantidade for 1 e diminuir, removemos o item
                    carrinho.Remove(item);
                    await _localStorage.SetItemAsync("carrinho", carrinho);
                }
                OnChange?.Invoke();
            }
        }


    }
}