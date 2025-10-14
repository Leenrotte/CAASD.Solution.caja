using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.DTOs
{
    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public UsuarioDTO? Usuario { get; set; }
    }
}