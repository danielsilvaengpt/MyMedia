using System.Collections.Generic;

namespace MyMedia.Shared.DTO
{
 public class AuthInfoDto
 {
 public bool IsAuthenticated { get; set; }
 public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
 }
}
