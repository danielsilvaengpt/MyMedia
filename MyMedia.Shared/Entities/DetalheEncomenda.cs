using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMedia.Shared.Entities
{
 public class DetalheEncomenda
 {
 [Key]
 public int Id { get; set; }

 [Required]
 public int EncomendaId { get; set; }
 [ForeignKey("EncomendaId")]
 public Encomenda? Encomenda { get; set; }

 [Required]
 public int ProdutoId { get; set; }
 [ForeignKey("ProdutoId")]
 public Produto? Produto { get; set; }

 [Required]
 [Column(TypeName = "decimal(18,2)")]
 public decimal PrecoUnitario { get; set; }

 [Required]
 public int Quantidade { get; set; }

 [NotMapped]
 public decimal Subtotal => Quantidade * PrecoUnitario;
 }
}
