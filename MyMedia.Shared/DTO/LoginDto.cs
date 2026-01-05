using System.ComponentModel.DataAnnotations;

namespace MyMedia.Shared.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Password é obrigatória")]
        public string Password { get; set; } = string.Empty;
    }
}