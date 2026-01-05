using Blazored.LocalStorage; // <--- Não te esqueças deste using
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyMedia.Shared.Interfaces;
using MyMedia.Shared.Services;
using MyMedia.Web.Client;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// API base
string apiUrl = "https://hfvrglcv-7022.uks1.devtunnels.ms/"; // Confirma a tua porta!

// Register Blazored.LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Register AuthTokenHandler
builder.Services.AddTransient<AuthTokenHandler>();

// Named HttpClient with handler
builder.Services.AddHttpClient("Api", client => client.BaseAddress = new Uri(apiUrl))
 .AddHttpMessageHandler<AuthTokenHandler>();

// Register default HttpClient to resolve to the named client
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

// Register ApiClient
builder.Services.AddScoped<ApiClient>();

// Update service registrations to use ApiClient where appropriate
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();
// Register modo disponiblizacao service
builder.Services.AddScoped<IModoDispService, ModoDispService>();

// Use the shared CustomAuthStateProvider
builder.Services.AddScoped<AuthenticationStateProvider, MyMedia.Shared.Services.CustomAuthStateProvider>();

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();