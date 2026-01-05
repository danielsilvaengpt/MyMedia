using System.Net.Http.Json;
using System.Text.Json;
using MyMedia.Shared.Entities;
using MyMedia.Shared.Interfaces;

namespace MyMedia.Shared.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        public CategoriaService(HttpClient http)
        {
            _http = http;
            // Essencial para ler o JSON corretamente (ignora maiúsculas/minúsculas)
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<Categoria>> GetCategoriasAsync()
        {
            try
            {
                // Tenta ir buscar as categorias à API
                // O endpoint deve ser "api/categorias" (confirma se o teu Controller se chama CategoriasController)
                var response = await _http.GetAsync("api/categorias");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Categoria>>(_options) ?? new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar categorias: {ex.Message}");
            }

            // Se falhar, devolve lista vazia para não crashar o site
            return new List<Categoria>();
        }
    }
}