using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.Entities
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public bool Leida { get; set; }
        public string Url { get; set; } = string.Empty;
        public virtual Usuario? Usuario { get; set; }
    }
}