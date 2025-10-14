using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string DireccionCompleta { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public bool Activo { get; set; }
        public int RolId { get; set; }
        public virtual Rol? Rol { get; set; }
    }
}