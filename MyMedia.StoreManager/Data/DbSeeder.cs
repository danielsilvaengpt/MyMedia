using Microsoft.AspNetCore.Identity;
using MyMedia.Shared.Entities;
using MyMedia.Shared.Entities;

namespace MyMedia.StoreManager.Data
{
    public static class DbSeeder
    {
        public static async Task EnsurePopulated(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 0. Reset da Base de Dados (Apagar e Recriar)
            // ATENÇÃO: Isto apaga TUDO. Use apenas em desenvolvimento/testes.
           // await context.Database.EnsureDeletedAsync();
            // await context.Database.EnsureCreatedAsync();

            // 1. Criar Roles (Perfis) Obrigatórios 
            string[] roles = { "Administrador", "Funcionario", "Cliente", "Fornecedor" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Criar Utilizador Administrador (se não existir)
            var adminEmail = "admin@mymedia.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NomeCompleto = "Administrador Principal",
                    EmailConfirmed = true,
                    ContaAtiva = true // Admin já nasce ativo [cite: 14]
                };

                // Password forte é obrigatória
                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }

            // 3. Criar Categorias e Subcategorias (Hierarquia)
            if (!context.Categorias.Any())
            {
                // --- 1. Música ---
                var catMusica = new Categoria { Nome = "Música", Descricao = "Suportes Físicos e Digitais" };
                context.Categorias.Add(catMusica);
                await context.SaveChangesAsync(); // Save to get ID

                // 1.1 Música - Rock
                var catRock = new Categoria { Nome = "Rock", CategoriaPaiId = catMusica.Id };
                context.Categorias.Add(catRock);
                await context.SaveChangesAsync();

                context.Categorias.AddRange(
                    new Categoria { Nome = "Sony Music Entertainment", CategoriaPaiId = catRock.Id },
                    new Categoria { Nome = "Universal Music Group", CategoriaPaiId = catRock.Id },
                    new Categoria { Nome = "Warner Music", CategoriaPaiId = catRock.Id }
                );

                // 1.2 Música - Pop & Comercial
                var catPop = new Categoria { Nome = "Pop & Comercial", CategoriaPaiId = catMusica.Id };
                context.Categorias.Add(catPop);
                await context.SaveChangesAsync();

                context.Categorias.AddRange(
                    new Categoria { Nome = "Sony Music", CategoriaPaiId = catPop.Id },
                    new Categoria { Nome = "Universal Music", CategoriaPaiId = catPop.Id }
                );

                // 1.3 Música - Jazz & Blues
                var catJazz = new Categoria { Nome = "Jazz & Blues", CategoriaPaiId = catMusica.Id };
                context.Categorias.Add(catJazz);
                await context.SaveChangesAsync();

                context.Categorias.Add(new Categoria { Nome = "Blue Note Records", CategoriaPaiId = catJazz.Id });


                // --- 2. Cinema e Audiovisual ---
                var catCinema = new Categoria { Nome = "Cinema e Audiovisual", Descricao = "DVD / Blu-Ray / 4K" };
                context.Categorias.Add(catCinema);
                await context.SaveChangesAsync();

                // 2.1 Cinema - Ação & Aventura
                var catAcao = new Categoria { Nome = "Ação & Aventura", CategoriaPaiId = catCinema.Id };
                context.Categorias.Add(catAcao);
                await context.SaveChangesAsync();

                context.Categorias.AddRange(
                    new Categoria { Nome = "Warner Bros. Pictures", CategoriaPaiId = catAcao.Id },
                    new Categoria { Nome = "Universal Pictures", CategoriaPaiId = catAcao.Id }
                );

                // 2.2 Cinema - Sci-Fi & Fantasia
                var catSciFi = new Categoria { Nome = "Sci-Fi & Fantasia", CategoriaPaiId = catCinema.Id };
                context.Categorias.Add(catSciFi);
                await context.SaveChangesAsync();

                context.Categorias.AddRange(
                    new Categoria { Nome = "Paramount Pictures", CategoriaPaiId = catSciFi.Id },
                    new Categoria { Nome = "20th Century Studios", CategoriaPaiId = catSciFi.Id }
                );

                // 2.3 Cinema - Animação
                var catAnimacao = new Categoria { Nome = "Animação", CategoriaPaiId = catCinema.Id };
                context.Categorias.Add(catAnimacao);
                await context.SaveChangesAsync();

                context.Categorias.Add(new Categoria { Nome = "Disney / Pixar", CategoriaPaiId = catAnimacao.Id });


                // --- 3. Complementos e Acessórios ---
                var catAcessorios = new Categoria { Nome = "Complementos e Acessórios", Descricao = "Equipamento de Áudio e Vídeo" };
                context.Categorias.Add(catAcessorios);
                await context.SaveChangesAsync();

                // 3.1 Acessórios de Áudio
                var catAudio = new Categoria { Nome = "Acessórios de Áudio", CategoriaPaiId = catAcessorios.Id };
                context.Categorias.Add(catAudio);
                await context.SaveChangesAsync();

                context.Categorias.AddRange(
                    new Categoria { Nome = "Sony Interactive", CategoriaPaiId = catAudio.Id },
                    new Categoria { Nome = "Marshall", CategoriaPaiId = catAudio.Id }
                );

                // 3.2 Acessórios de Vídeo
                var catVideo = new Categoria { Nome = "Acessórios de Vídeo", CategoriaPaiId = catAcessorios.Id };
                context.Categorias.Add(catVideo);
                await context.SaveChangesAsync();

                context.Categorias.Add(new Categoria { Nome = "Logitech", CategoriaPaiId = catVideo.Id });


                // --- 4. Merchandising e Colecionáveis ---
                var catMerch = new Categoria { Nome = "Merchandising e Colecionáveis", Descricao = "Vestuário e Figuras" };
                context.Categorias.Add(catMerch);
                await context.SaveChangesAsync();

                // 4.1 Vestuário
                var catVestuario = new Categoria { Nome = "Vestuário (Clothing)", CategoriaPaiId = catMerch.Id };
                context.Categorias.Add(catVestuario);
                await context.SaveChangesAsync();

                context.Categorias.Add(new Categoria { Nome = "Rock Off Retail", CategoriaPaiId = catVestuario.Id });

                // 4.2 Figuras de Coleção
                var catFiguras = new Categoria { Nome = "Figuras de Coleção", CategoriaPaiId = catMerch.Id };
                context.Categorias.Add(catFiguras);
                await context.SaveChangesAsync();

                context.Categorias.Add(new Categoria { Nome = "Funko", CategoriaPaiId = catFiguras.Id });

                await context.SaveChangesAsync();
            }
            
        }
    }
}