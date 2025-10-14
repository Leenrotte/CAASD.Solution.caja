using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.Entities
{
    public class EstadoServicio
    {
        public int Id { get; set; }
        public string Zona { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFinEstimada { get; set; }
        public string TipoInterrupcion { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}