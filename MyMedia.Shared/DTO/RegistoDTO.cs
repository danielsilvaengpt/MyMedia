using System.ComponentModel.DataAnnotations;

namespace MyMedia.Shared.DTOs
{
    public class RegistoDTO
    {
        // --- DADOS BÁSICOS (Obrigatórios para todos) ---
        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Nome é obrigatório")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Password é obrigatória")]
        [MinLength(6, ErrorMessage = "A password deve ter pelo menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "As passwords não coincidem")]
        public string ConfirmarPassword { get; set; } = string.Empty;

        // "Cliente" ou "Fornecedor"
        public string TipoUtilizador { get; set; } = "Cliente";

        // --- CAMPOS DE CLIENTE (Opcionais pois podem ser nulos se for Fornecedor) ---
        public string? Nif { get; set; }    // <--- O erro pedia este
        public string? Morada { get; set; }

        // --- CAMPOS DE FORNECEDOR (Opcionais pois podem ser nulos se for Cliente) ---
        public string? NomeEmpresa { get; set; } // <--- O erro pedia este
        public string? NifEmpresa { get; set; }  // <--- O erro pedia este
        public string? Iban { get; set; }        // <--- O erro pedia este
    }
}