using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.Entities
{
    public class ReporteAveria
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string TipoAveria { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public DateTime FechaReporte { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public DateTime? FechaResolucion { get; set; }
        public string FotoUrl { get; set; } = string.Empty;
        public int? TecnicoAsignadoId { get; set; }
        public string ComentarioResolucion { get; set; } = string.Empty;
        public virtual Usuario? Usuario { get; set; }
    }
}