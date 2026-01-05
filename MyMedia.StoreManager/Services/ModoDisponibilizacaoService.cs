using Microsoft.EntityFrameworkCore;
using MyMedia.StoreManager.Data; // Ajusta para o namespace onde está o teu ApplicationDbContext
using MyMedia.Shared.Entities;

namespace MyMedia.StoreManager.Services
{
    public class ModoDisponibilizacaoService
    {
        private readonly ApplicationDbContext _context;

        // Injetamos o Contexto da Base de Dados em vez do HttpClient
        public ModoDisponibilizacaoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ModoDisponibilizacao>> GetAll()
        {
            return await _context.ModosDisponibilizacao
                                 .AsNoTracking() // Melhora performance em consultas de leitura
                                 .ToListAsync();
        }

        public async Task<ModoDisponibilizacao?> Get(int id)
        {
            return await _context.ModosDisponibilizacao
                                 .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task Add(ModoDisponibilizacao modo)
        {
            _context.ModosDisponibilizacao.Add(modo);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ModoDisponibilizacao modo)
        {
            _context.ModosDisponibilizacao.Update(modo);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var modo = await _context.ModosDisponibilizacao.FindAsync(id);
            if (modo != null)
            {
                _context.ModosDisponibilizacao.Remove(modo);
                await _context.SaveChangesAsync();
            }
        }
    }
}