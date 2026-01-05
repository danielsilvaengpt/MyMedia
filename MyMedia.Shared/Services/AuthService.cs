using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MyMedia.Shared.DTOs;
using MyMedia.Shared.Interfaces;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MyMedia.Shared.Services
{
    public class AuthService: IAuthService
    {
        private readonly ApiClient _apiClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(ApiClient apiClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _apiClient = apiClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<string> Registar(RegistoDTO request)
        {
            var result = await _apiClient.RegisterAsync(request);
            return result ? "Sucesso" : "Erro";
        }

        public async Task<string> Login(LoginDTO request)
        {
            var session = await _apiClient.LoginAsync(request);
            if (session == null) return null;

            // Save token to local storage
            await _localStorage.SetItemAsStringAsync("authToken", session.Token);

            // Notify provider
            var customProvider = (CustomAuthStateProvider)_authStateProvider;
            customProvider.NotificarUserLogin(session.Token);

            return session.Token;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _apiClient.Logout();
            var authStateProvider = (CustomAuthStateProvider)_authStateProvider;
            authStateProvider.NotificarUserLogout();
        }
    }
}