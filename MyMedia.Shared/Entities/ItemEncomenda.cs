using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMedia.Shared.Entities
{
    public class ItemEncomenda
    {
        [Key]
        public int Id { get; set; }

        // "Pode ser adquirido mais do que um item de cada produto" [cite: 98]
        [Required]
        [Range(1, 1000, ErrorMessage = "A quantidade deve ser pelo menos 1.")]
        public int Quantidade { get; set; }

        // --- PREÇO CONGELADO ---
        // Guardamos o preço FINAL (Base + Margem) no momento da compra.
        // Se o fornecedor mudar o preço base amanhã, esta encomenda antiga mantém o valor correto.
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }

        // --- RELAÇÕES (Foreign Keys) ---

        // Ligação à Encomenda principal
        [Required]
        public int EncomendaId { get; set; }

        [ForeignKey("EncomendaId")]
        public Encomenda? Encomenda { get; set; }

        // Ligação ao Produto comprado
        [Required]
        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public Produto? Produto { get; set; }

        // --- PROPRIEDADE AUXILIAR (Calculada) ---
        // Útil para mostrar o total desta linha no Carrinho ou Fatura (Qtd * Preço)
        // O [NotMapped] garante que não cria uma coluna extra desnecessária na BD.
        [NotMapped]
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }
}