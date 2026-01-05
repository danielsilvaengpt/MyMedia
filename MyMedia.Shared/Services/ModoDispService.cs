using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using MyMedia.Shared.Entities;
using MyMedia.Shared.Interfaces;

namespace MyMedia.Shared.Services
{
    public class ModoDispService: IModoDispService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        public ModoDispService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<ModoDisponibilizacao>> GetModosAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/modosdisponibilizacao");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ModoDisponibilizacao>>(_options) ?? new List<ModoDisponibilizacao>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar modos: {ex.Message}");
            }

            return new List<ModoDisponibilizacao>();
        }
    }
}
