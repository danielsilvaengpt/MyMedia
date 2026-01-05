using MyMedia.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMedia.Shared.Interfaces
{
    public interface IAuthService
    {
        Task<string> Registar(RegistoDTO request);
        Task<string> Login(LoginDTO request);
        Task Logout(); // <--- ADICIONA ISTO
    }
}
