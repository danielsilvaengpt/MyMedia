using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // Importante para evitar ciclos infinitos no JSON


namespace MyMedia.Shared.Entities
{
    public class ModoDisponibilizacao
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        // --- RELAÇÃO: Um modo tem "Muitos" produtos ---
        [JsonIgnore]
        public ICollection<Produto>? Produtos { get; set; }
    }
}