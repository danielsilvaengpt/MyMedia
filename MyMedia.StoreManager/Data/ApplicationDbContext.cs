using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyMedia.Shared.Entities;

namespace MyMedia.StoreManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Encomenda> Encomendas { get; set; }
        // Se tiveres a classe ItemEncomenda definida, adiciona o DbSet:
        public DbSet<ItemEncomenda> ItensEncomenda { get; set; }
        public DbSet<DetalheEncomenda> DetalhesEncomenda { get; set; }

        public DbSet<ModoDisponibilizacao> ModosDisponibilizacao { get; set; }

        // --- A CORREÇÃO É ESTE BLOCO ABAIXO ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // IMPORTANTE: Mantém o Identity a funcionar

            // Configuração para resolver o erro de Ciclo
            modelBuilder.Entity<ItemEncomenda>()
                .HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict); // <--- AQUI! Muda de Cascade para Restrict
                                                    // Restrict significa: "Não podes apagar um Produto se ele já foi vendido numa encomenda"

            // Também configurar DetalheEncomenda para evitar exclusão em cascata
            modelBuilder.Entity<DetalheEncomenda>()
                .HasOne(d => d.Produto)
                .WithMany()
                .HasForeignKey(d => d.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}