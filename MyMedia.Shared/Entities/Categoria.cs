using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMedia.Shared.Entities
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome não pode exceder 50 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "A descrição não pode exceder 250 caracteres.")]
        public string? Descricao { get; set; } // Útil para tooltips na interface

        // --- HIERARQUIA (Requisito dos Frisos Deslizantes) ---
        // O enunciado [cite: 99] pede subcategorias dinâmicas.
        // Usamos um auto-relacionamento: Uma categoria pode ser "filha" de outra.

        public int? CategoriaPaiId { get; set; } // Null = Categoria Principal (Raiz)

        [ForeignKey("CategoriaPaiId")]
        public Categoria? CategoriaPai { get; set; }

        // Coleção de subcategorias (Ex: Se isto for "Rock", aqui estarão as "Editoras")
        public ICollection<Categoria> SubCategorias { get; set; } = new List<Categoria>();

        // --- RELAÇÃO COM PRODUTOS ---
        // Uma categoria tem vários produtos associados.
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}