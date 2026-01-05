using MyMedia.Shared.Interfaces;
using MyMedia.Shared.Services;
using MyMedia.Web.Client;
using Blazored.LocalStorage;
using MyMedia.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVIÇOS ---
// Aqui apenas ativamos os componentes, NÃO adicionamos assemblies ainda.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();

// Add authentication services
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Configuração do HttpClient (Isto está correto e é muito importante)
string apiUrl = "https://hfvrglcv-7022.uks1.devtunnels.ms/"; // Confirma se a porta da API é esta!
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(apiUrl);
});
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });
var app = builder.Build();

// --- 2. PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// --- 3. ROTAS (AQUI É QUE ESTAVA O PROBLEMA) ---
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        // Carrega as rotas do projeto Cliente
        typeof(MyMedia.Web.Client.Routes).Assembly,

        // CORREÇÃO: Carrega as páginas do projeto Shared (O Catálogo!)
        typeof(MyMedia.Shared.Services.ApiClient).Assembly
    );

app.Run();