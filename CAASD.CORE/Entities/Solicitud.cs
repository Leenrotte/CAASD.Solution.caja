using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.Entities
{
    public class Solicitud
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string TipoSolicitud { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaRespuesta { get; set; }
        public string Respuesta { get; set; } = string.Empty;
        public string DocumentosAdjuntos { get; set; } = string.Empty;
        public virtual Usuario? Usuario { get; set; }
    }
}