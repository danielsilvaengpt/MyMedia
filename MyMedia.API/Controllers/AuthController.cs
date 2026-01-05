using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyMedia.Shared.DTOs;
using MyMedia.Shared.Entities;
using MyMedia.Shared.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyMedia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration; // <---1. ADICIONADO CAMPO

        //2. ADICIONADO IConfiguration NO CONSTRUTOR
        public AuthController(UserManager<ApplicationUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration; // <---3. INJETADO AQUI
        }

        [HttpPost("registar")]
        public async Task<IActionResult> Registar([FromBody] RegistoDTO request)
        {
            // 1. Criar as Roles se não existirem (Segurança)
            string[] roles = { "Cliente", "Fornecedor", "Admin", "Funcionario" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Definir se a conta nasce Ativa ou Pendente
            // REGRA: Clientes entram logo (true). Fornecedores ficam pendentes (false).
            bool estadoInicial = false;

            if (request.TipoUtilizador == "Cliente")
            {
                estadoInicial = true; // Cliente pode comprar logo
            }
            else
            {
                estadoInicial = false; // Fornecedor (e outros) têm de esperar aprovação
            }

            // 3. Criar o Utilizador com os dados todos
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                NomeCompleto = request.NomeCompleto,
                TipoUtilizador = request.TipoUtilizador,

                // 👇 AQUI APLICAMOS O ESTADO CALCULADO EM CIMA
                ContaAtiva = estadoInicial,

                // Restantes dados...
                Nif = request.Nif,
                Morada = request.Morada,
                NomeEmpresa = request.NomeEmpresa,
                NifEmpresa = request.NifEmpresa,
                Iban = request.Iban
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            // 4. Atribuir a Role
            await _userManager.AddToRoleAsync(user, request.TipoUtilizador);

            // 5. Mensagem de Sucesso personalizada
            if (estadoInicial == false)
            {
                return Ok("Registo efetuado! A conta aguarda aprovação do Administrador.");
            }
            else
            {
                return Ok("Sucesso");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            //1. Procurar o utilizador
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest("Utilizador ou Password incorretos.");
            }

            // 👇 NOVA VERIFICAÇÃO DE SEGURANÇA 👇
            if (user.ContaAtiva == false)
            {
                return BadRequest("A tua conta está pendente de aprovação. Aguarda validação do Administrador.");
            }
            // ------------------------------------

            //2. Verificar Password
            var passwordValida = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValida)
            {
                return BadRequest("Utilizador ou Password incorretos.");
            }

            //3. Claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName!),
        new Claim(ClaimTypes.Email, user.Email!)
    };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //4. Gerar Chave
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JWT:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //5. Criar Token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Return strongly-typed DTO matching client expectations
            var session = new UserSession
            {
                Token = tokenString,
                UserName = user.UserName ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty
            };

            return Ok(session);
        }

        // Diagnostic endpoint to see what the API receives
        [HttpGet("me")]
        public IActionResult Me()
        {
            var dto = new AuthInfoDto
            {
                IsAuthenticated = User?.Identity?.IsAuthenticated ?? false
            };

            if (User?.Identity?.IsAuthenticated == true)
            {
                foreach (var c in User.Claims)
                {
                    dto.Claims[c.Type] = c.Value;
                }
            }

            return Ok(dto);
        }
    }
}