using System.Net.Http.Json;
using System.Text.Json;
using MyMedia.Shared.Entities;
using MyMedia.Shared.Interfaces;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;

namespace MyMedia.Shared.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly ApiClient _apiClient;
        private readonly JsonSerializerOptions _options;

        public ProdutoService(ApiClient apiClient)
        {
            _apiClient = apiClient;
            // Configuração essencial para não falhar a ler o JSON
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<Produto>> GetProdutosAsync()
        {
            return await _apiClient.GetProdutosAsync();
        }

        public async Task<Produto?> GetProdutoAsync(int id)
        {
            return await _apiClient.GetProdutoAsync(id);
        }

        public async Task<List<Produto>> GetProdutosPorCategoriaAsync(int categoriaId)
        {
            return await _apiClient.GetProdutosPorCategoriaAsync(categoriaId);
        }

        public async Task<List<Produto>> GetMeusProdutos()
        {
            return await _apiClient.GetMeusProdutosAsync();
        }

        public async Task DeleteProduto(int id)
        {
            await _apiClient.DeleteProdutoAsync(id);
        }

        public async Task<Produto> CreateProduto(Produto produto)
        {
            return await _apiClient.CreateProdutoAsync(produto);
        }

        public async Task<Produto> GetProduto(int id)
        {
            return await _apiClient.GetProdutoAsync(id);
        }

        public async Task<Produto> UpdateProduto(Produto produto)
        {
            return await _apiClient.UpdateProdutoAsync(produto);
        }
    }
}