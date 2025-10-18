using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAASD.Core.DTOs;

namespace CAASD.Core.Interfaces.Services
{
    public interface IAuthService
    {
        LoginResponseDTO Login(LoginRequestDTO request);
        bool RegistrarUsuario(UsuarioDTO usuario, string password);
        bool CambiarPassword(int usuarioId, string passwordActual, string passwordNuevo);
        bool RecuperarPassword(string email);
        bool RegistrarClienteDesdeWeb(UsuarioDTO dto, string password);
    }
}