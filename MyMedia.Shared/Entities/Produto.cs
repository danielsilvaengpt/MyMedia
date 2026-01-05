using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMedia.Shared.Entities
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, ErrorMessage = "O título não pode exceder 100 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; } = string.Empty;

        // O enunciado refere "filmes, disco, CD" e "suportes media",
        // logo precisamos de uma imagem ou capa.
        // public string? ImageUrl { get; set; } // REMOVIDO: Agora usamos byte[] para armazenar a imagem diretamente
        public byte[]? Imagem { get; set; }
        public string? ImagemContentType { get; set; } // Para saber se é image/png, image/jpeg, etc.

        // Propriedade auxiliar para exibir a imagem em <img> tags (Base64)
        [NotMapped]
        public string ImageUrl
        {
            get
            {
                if (Imagem != null && Imagem.Length > 0 && !string.IsNullOrEmpty(ImagemContentType))
                {
                    var base64 = Convert.ToBase64String(Imagem);
                    return $"data:{ImagemContentType};base64,{base64}";
                }
                return string.Empty; // Ou retorna uma imagem placeholder padrão se quiseres
            }
        }


        // --- REGRAS DE PREÇO [cite: 13, 14] ---

        // "O preço base, definido pelo fornecedor" [cite: 14]
        [Required]
        [Range(0.01, 100000, ErrorMessage = "O preço base deve ser maior que zero.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoBase { get; set; }

        // "Percentagem definido pelo administrador" [cite: 14]
        // Exemplo: 0.20 para 20% de margem.
        [Column(TypeName = "decimal(18,2)")]
        public decimal PercentagemMargem { get; set; } = 0.0m;

        // "O soma do preço base com o valor correspondente a esta percentagem constitui o preço final" 
        // NotMapped significa que não cria uma coluna na BD, é calculado sempre que pedes o produto.
        [NotMapped]
        public decimal PrecoFinal => PrecoBase + (PrecoBase * PercentagemMargem);


        // --- ESTADOS E DISPONIBILIDADE ---

        // "Listagem ou venda consoante o caso" 
        // True = Produto à venda (pode ser comprado).
        // False = Apenas para listagem/coleção (não tem botão de comprar).
        public bool ParaVenda { get; set; } = true;

        // "Passará do estado inactivo para activo" [cite: 14]
        // "Os produtos inseridos... têm de ficar... no estado pendente" [cite: 139]
        // False = Pendente (Ninguém vê exceto Fornecedor e Admin).
        // True = Ativo (Visível na Loja Pública).
        public bool Ativo { get; set; } = false;

        // Necessário para gerir se ainda há itens para venda[cite: 154].
        public int Stock { get; set; } = 1;

        // Data de registo para ordenação (útil para "Novidades")
        public DateTime DataRegisto { get; set; } = DateTime.Now;


        // --- RELAÇÕES (Foreign Keys) ---
        // Categoria Obrigatória [cite: 99]
        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        // O dono do produto (Fornecedor) 
        // A empresa é apenas intermediária, o produto pertence a um Fornecedor.
        [Required]
        public string FornecedorId { get; set; } = string.Empty;

        [ForeignKey("FornecedorId")]
        public ApplicationUser? Fornecedor { get; set; }

        // 1. A Chave Estrangeira (Guarda o ID na tabela Produtos)
        public int? ModoDisponibilizacaoId { get; set; }

        // 2. A Propriedade de Navegação (Permite fazer produto.ModoDisponibilizacao.Nome)
        public ModoDisponibilizacao? ModoDisponibilizacao { get; set; }
    }
}