using System.ComponentModel.DataAnnotations;

namespace MyMedia.Shared.DTOs
{
    public class RegisterDto
    {
        [Required] public string Nome { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Compare("Password")] public string ConfirmPassword { get; set; } = string.Empty;
        [Required] public string NIF { get; set; } = string.Empty;
        [Required] public string Morada { get; set; } = string.Empty;
        [Required] public string TipoUtilizador { get; set; } = "Cliente"; // "Cliente" ou "Fornecedor"
    }
}