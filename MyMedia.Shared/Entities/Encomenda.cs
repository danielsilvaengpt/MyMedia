using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyMedia.Shared.Entities
{
    public class Encomenda
    {
        [Key]
        public int Id { get; set; }

        // Data exata da compra (importante para relatórios e histórico)
        public DateTime DataEncomenda { get; set; } = DateTime.Now;

        // O valor total da encomenda (soma de todos os sub-totais dos itens)
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }

        // Estados para gestão de loja [cite: 176, 177]
        // Exemplos: "Pendente", "Pago", "Expedido", "Concluído"
        [StringLength(50)]
        public string Estado { get; set; } = "Pendente";

        // Dados do checkout
        public string MetodoPagamento { get; set; } = "MBWay";
        public string MoradaEntrega { get; set; } = string.Empty;

        // --- RELAÇÕES (Foreign Keys) ---

        // Quem fez a encomenda? (Cliente) [cite_start][cite: 10]
        [Required]
        public string ClienteId { get; set; } = string.Empty;

        [ForeignKey("ClienteId")]
        public ApplicationUser? Cliente { get; set; }

        // Detalhes / Linhas da encomenda
        public ICollection<DetalheEncomenda> Detalhes { get; set; } = new List<DetalheEncomenda>();
    }
}