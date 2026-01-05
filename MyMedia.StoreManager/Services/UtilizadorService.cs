using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyMedia.Shared.Entities;
using MyMedia.StoreManager.Data;

namespace MyMedia.StoreManager.Services
{
    public class UtilizadorService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UtilizadorService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            return await _context.Users.OrderBy(u => u.Email).ToListAsync();
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        // ALTERAR ESTADO (Ativar/Suspender)
        public async Task ToggleEstadoAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.ContaAtiva = !user.ContaAtiva;
                await _context.SaveChangesAsync();
            }
        }

        // ALTERAR QUALQUER ROLE (Genérico)
        // Serve para promover a Funcionário, ou dar permissão de Fornecedor/Cliente
        public async Task ToggleRoleAsync(string userId, string roleName)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            bool isInRole = await _userManager.IsInRoleAsync(user, roleName);

            if (isInRole)
            {
                // Remover Role
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
            else
            {
                // Adicionar Role
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}