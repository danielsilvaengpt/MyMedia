// No ficheiro ApplicationUser.cs (ou o nome que deste à tua classe de utilizador)
using Microsoft.AspNetCore.Identity;

namespace MyMedia.Shared.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string NomeCompleto { get; set; } = string.Empty;

        // 👇 ADICIONA ESTES CAMPOS NOVOS
        public string? Nif { get; set; }
        public string? Morada { get; set; }
        public string? NomeEmpresa { get; set; }
        public string? NifEmpresa { get; set; }
        public string? Iban { get; set; }

        // Define se é Cliente ou Fornecedor
        public string TipoUtilizador { get; set; } = "Cliente";
        public bool ContaAtiva { get; set; } = false;
    }
}