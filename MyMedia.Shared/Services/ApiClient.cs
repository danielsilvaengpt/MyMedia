using System.Net.Http.Headers;
using System.Net.Http.Json;
using MyMedia.Shared.DTOs;
using MyMedia.Shared.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace MyMedia.Shared.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;

        // Evento para avisar o layout quando o login/logout acontece
        // (Para o menu mudar de "Entrar" para "Olá, João")
        public event Action? OnAuthStateChanged;

        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        // ==========================================
        // 1. AUTENTICAÇÃO
        // ==========================================

        public async Task<UserSession?> LoginAsync(LoginDTO loginDto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var session = await response.Content.ReadFromJsonAsync<UserSession>();

                if (session != null)
                {
                    // Guarda o Token para os próximos pedidos
                    _http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", session.Token);

                    OnAuthStateChanged?.Invoke(); // Avisa a app que mudou
                    return session;
                }
            }
            return null;
        }

        public async Task<bool> RegisterAsync(RegistoDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/registar", dto);
            // Retorna true se criou (200 OK), false se falhou
            return response.IsSuccessStatusCode;
        }

        public void Logout()
        {
            _http.DefaultRequestHeaders.Authorization = null;
            OnAuthStateChanged?.Invoke();
        }

        // ==========================================
        // 2. CATÁLOGO PÚBLICO
        // ==========================================

        public async Task<List<Produto>> GetProdutosAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Produto>>("api/produtos")
                       ?? new List<Produto>();
            }
            catch
            {
                return new List<Produto>();
            }
        }

        public async Task<List<Categoria>> GetCategoriasAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Categoria>>("api/categorias")
                       ?? new List<Categoria>();
            }
            catch
            {
                return new List<Categoria>();
            }
        }

        public async Task<Produto?> GetProdutoDetalheAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Produto>($"api/produtos/{id}");
            }
            catch
            {
                return null;
            }
        }

        // ==========================================
        // 3. PRODUTOS (Requer Login para algumas)
        // ==========================================

        public async Task<List<Produto>> GetMeusProdutosAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Produto>>("api/produtos?me=true") ?? new List<Produto>();
            }
            catch
            {
                return new List<Produto>();
            }
        }

        public async Task DeleteProdutoAsync(int id)
        {
            await _http.DeleteAsync($"api/produtos/{id}");
        }

        public async Task<Produto> CreateProdutoAsync(Produto produto)
        {
            var result = await _http.PostAsJsonAsync("api/produtos", produto);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadFromJsonAsync<Produto>();
            }

            var text = await result.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to create produto. Status: {result.StatusCode}. {text}");
        }

        public async Task<Produto?> UpdateProdutoAsync(Produto produto)
        {
            var result = await _http.PutAsJsonAsync($"api/produtos/{produto.Id}", produto);

            if (result.IsSuccessStatusCode)
            {
                // 👇 CORREÇÃO AQUI:
                // Se o servidor devolver 204 (No Content), não tentamos ler JSON.
                // Assumimos que correu tudo bem e devolvemos o produto que enviámos.
                if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return produto;
                }

                // Se devolver 200 (OK) com dados, aí sim lemos o JSON.
                return await result.Content.ReadFromJsonAsync<Produto>();
            }

            return null;
        }

        public async Task<Produto?> GetProdutoAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Produto>($"api/produtos/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Produto>> GetProdutosPorCategoriaAsync(int categoriaId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Produto>>($"api/produtos/categoria/{categoriaId}") ?? new List<Produto>();
            }
            catch
            {
                return new List<Produto>();
            }
        }

        // ==========================================
        // 4. ENCOMENDAS (Requer Login)
        // ==========================================

        public async Task<bool> EnviarEncomendaAsync(EncomendaDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/encomendas", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Encomenda>> GetMinhasEncomendasAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Encomenda>>("api/encomendas/minhas")
                       ?? new List<Encomenda>();
            }
            catch
            {
                return new List<Encomenda>();
            }
        }

        public async Task<List<VendaDto>> GetVendasFornecedorAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<VendaDto>>("api/encomendas/vendas")
                       ?? new List<VendaDto>();
            }
            catch
            {
                return new List<VendaDto>();
            }
        }
    }
}