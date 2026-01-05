using MyMedia.Shared.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMedia.Shared.Interfaces
{
    public interface IModoDispService
    {
        Task<List<ModoDisponibilizacao>> GetModosAsync();
    }
}
