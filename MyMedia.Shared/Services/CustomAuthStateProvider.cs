using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace MyMedia.Shared.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;
        private const string TokenKey = "authToken";

        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Limpa header por defeito
            _http.DefaultRequestHeaders.Authorization = null;

            // Lê o token do browser
            var token = await _localStorage.GetItemAsStringAsync(TokenKey);
            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Normaliza (remove aspas se existir)
            token = token.Trim('"');

            try
            {
                var claims = ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwt");

                // Define header para requisições HTTP
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                // Token inválido -> remove e devolve anónimo
                await _localStorage.RemoveItemAsync(TokenKey);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        // Método auxiliar para extrair claims de um JWT usando JsonDocument para melhor performance
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
                return Array.Empty<Claim>();

            var parts = jwt.Split('.');
            if (parts.Length < 2)
                return Array.Empty<Claim>();

            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);

            using var doc = JsonDocument.Parse(jsonBytes);
            var root = doc.RootElement;

            var claims = new List<Claim>();

            foreach (var prop in root.EnumerateObject())
            {
                // Trata roles de forma especial
                if (prop.Name == "role" || prop.Name == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                {
                    if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var role in prop.Value.EnumerateArray())
                        {
                            var roleValue = role.ValueKind == JsonValueKind.String ? role.GetString() : role.GetRawText();
                            if (!string.IsNullOrEmpty(roleValue))
                                claims.Add(new Claim(ClaimTypes.Role, roleValue));
                        }
                    }
                    else
                    {
                        var roleValue = prop.Value.ValueKind == JsonValueKind.String ? prop.Value.GetString() : prop.Value.GetRawText();
                        if (!string.IsNullOrEmpty(roleValue))
                            claims.Add(new Claim(ClaimTypes.Role, roleValue));
                    }
                }
                else
                {
                    // Map standard JWT claim names to .NET claim types so APIs can find NameIdentifier etc.
                    string claimType = prop.Name switch
                    {
                        "sub" => ClaimTypes.NameIdentifier,
                        "nameid" => ClaimTypes.NameIdentifier,
                        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" => ClaimTypes.NameIdentifier,
                        "email" => ClaimTypes.Email,
                        "unique_name" => ClaimTypes.Name,
                        _ => prop.Name
                    };

                    string? value = prop.Value.ValueKind switch
                    {
                        JsonValueKind.String => prop.Value.GetString(),
                        JsonValueKind.Number => prop.Value.GetRawText(),
                        JsonValueKind.True => "true",
                        JsonValueKind.False => "false",
                        _ => prop.Value.GetRawText()
                    };

                    if (value is not null)
                        claims.Add(new Claim(claimType, value));
                }
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            // Converte base64url para base64 standard
            base64 = base64.Replace('-', '+').Replace('_', '/');

            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
                case 0: break;
                default: throw new FormatException("Invalid base64 string");
            }

            return Convert.FromBase64String(base64);
        }

        public void NotificarUserLogin(string token)
        {
            token = token?.Trim('"');
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);

            // Define header para chamadas HTTP após login
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotificarUserLogout()
        {
            // Remove token do storage (fire-and-forget) e limpa header
            _ = _localStorage.RemoveItemAsync(TokenKey);
            _http.DefaultRequestHeaders.Authorization = null;

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}