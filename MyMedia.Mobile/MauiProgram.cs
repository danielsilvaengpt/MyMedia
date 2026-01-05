using Microsoft.Extensions.Logging;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MyMedia.Shared.Interfaces;
using MyMedia.Shared.Services;

namespace MyMedia.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            // --- CONFIGURAÇÃO IGUAL AO WEB CLIENT ---

            // 1. URL da API (Atenção: Em Android Emulator usa 10.0.2.2 em vez de localhost)
            // Para dispositivo físico, usa o IP da tua máquina na rede (ex: 192.168.1.x)
            string apiUrl = "https://hfvrglcv-7022.uks1.devtunnels.ms/";
               

            // 2. LocalStorage
            builder.Services.AddBlazoredLocalStorage();

            // 3. Auth Handler
            builder.Services.AddTransient<AuthTokenHandler>();

            // 4. HttpClient com Handler
            // Nota: Em MAUI, HTTPS com certificado self-signed pode dar erro. 
            // Para dev, podes precisar de um handler que ignore SSL errors (HttpsClientHandlerService)
            // Mas para simplificar, vamos tentar usar o padrão.
            builder.Services.AddHttpClient("Api", client => client.BaseAddress = new Uri(apiUrl))
                .AddHttpMessageHandler<AuthTokenHandler>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

            // 5. Serviços Partilhados
            builder.Services.AddScoped<ApiClient>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IProdutoService, ProdutoService>();
            builder.Services.AddScoped<ICategoriaService, CategoriaService>();
            builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();
            builder.Services.AddScoped<IModoDispService, ModoDispService>();

            // 6. Auth State Provider
            builder.Services.AddScoped<AuthenticationStateProvider, MyMedia.Shared.Services.CustomAuthStateProvider>();
            builder.Services.AddAuthorizationCore();

            return builder.Build();
        }
    }
}
