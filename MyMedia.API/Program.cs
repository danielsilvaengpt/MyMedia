using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyMedia.API.Data;
using MyMedia.Shared.Entities;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Security.Claims;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// ---1. CORS (Para o Blazor falar com a API) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ---2. CONTROLLERS E JSON ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ---3. SWAGGER ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// ---4. BASE DE DADOS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ==============================================================================
// 🚨 MUDANÇA IMPORTANTE: O IDENTITY VEM PRIMEIRO!
// ==============================================================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 🚨 BLOQUEIO DE REDIRECTS (VEM LOGO A SEGUIR AO IDENTITY)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401; // Diz "Não Autorizado" em vez de mandar para o Login
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403; // Diz "Proibido"
        return Task.CompletedTask;
    };
});

// ==============================================================================
// 🚨 O JWT VEM EM ÚLTIMO (PARA GANHAR AOS COOKIES DO IDENTITY)
// ==============================================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),

        // 👇 CORREÇÃO: Usamos os tipos padrão da Microsoft para bater certo com o AuthController
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };

    // (Mantive os teus logs de debug, são úteis!)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            var logger = ctx.HttpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("JwtBearer");
            var hasHeader = ctx.Request.Headers.ContainsKey("Authorization");
            logger?.LogInformation("OnMessageReceived: Authorization header present={HasHeader}", hasHeader);
            return Task.CompletedTask;
        },
        OnTokenValidated = ctx =>
        {
            var logger = ctx.HttpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("JwtBearer");
            try
            {
                var claims = ctx.Principal?.Claims.Select(c => c.Type + "=" + c.Value) ?? Enumerable.Empty<string>();
                logger?.LogInformation("OnTokenValidated. Claims: {Claims}", string.Join(", ", claims));

                var identity = ctx.Principal?.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    // Map name identifier
                    if (!identity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                    {
                        var nid = ctx.Principal?.FindFirst("nameid")?.Value ?? ctx.Principal?.FindFirst("sub")?.Value;
                        if (!string.IsNullOrEmpty(nid)) identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, nid));
                    }
                    // Map name
                    if (!identity.HasClaim(c => c.Type == ClaimTypes.Name))
                    {
                        var uname = ctx.Principal?.FindFirst("unique_name")?.Value ?? ctx.Principal?.FindFirst(ClaimTypes.Name)?.Value;
                        if (!string.IsNullOrEmpty(uname)) identity.AddClaim(new Claim(ClaimTypes.Name, uname));
                    }
                    // Map roles (Arrays or Single)
                    if (!identity.HasClaim(c => c.Type == ClaimTypes.Role))
                    {
                        var roleClaim = ctx.Principal?.FindFirst("role");
                        if (roleClaim != null && !string.IsNullOrEmpty(roleClaim.Value))
                        {
                            if (roleClaim.Value.TrimStart().StartsWith("["))
                            {
                                try
                                {
                                    var roles = JsonSerializer.Deserialize<string[]>(roleClaim.Value);
                                    if (roles != null) foreach (var r in roles) identity.AddClaim(new Claim(ClaimTypes.Role, r));
                                }
                                catch { /* Ignore parsing error */ }
                            }
                            else
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error while logging or mapping claims in OnTokenValidated");
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = ctx =>
        {
            var logger = ctx.HttpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("JwtBearer");
            logger?.LogError(ctx.Exception, "Authentication failed: {Message}", ctx.Exception?.Message);
            return Task.CompletedTask;
        },
        OnChallenge = ctx =>
        {
            var logger = ctx.HttpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("JwtBearer");
            logger?.LogWarning("OnChallenge triggered. Error={Error}, Description={ErrorDescription}", ctx.Error, ctx.ErrorDescription);
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();



// ---6. PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PermitirTudo");

// Logs de debug do Request (Mantive os teus)
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("RequestDebug");
    var hasHeader = context.Request.Headers.ContainsKey("Authorization");
    if (hasHeader)
    {
        var auth = context.Request.Headers["Authorization"].ToString();
        if (auth.StartsWith("Bearer "))
        {
            var token = auth.Substring(7);
            var masked = token.Length > 12 ? token.Substring(0, 6) + "..." + token.Substring(token.Length - 6) : token;
            logger?.LogInformation("Incoming masked token: {Token}", masked);
        }
    }
    await next();
});

app.UseAuthentication(); // 1. Quem é?

// Logs pós-autenticação (Mantive os teus)
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("RequestDebug");
    var isAuth = context.User?.Identity?.IsAuthenticated ?? false;
    if (isAuth)
    {
        foreach (var c in context.User.Claims)
        {
            logger?.LogInformation("Authenticated Claim: {Type}={Value}", c.Type, c.Value);
        }
    }
    await next();
});

app.UseAuthorization(); // 2. Pode entrar?

app.MapControllers();

app.Run();