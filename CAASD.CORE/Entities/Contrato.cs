using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.Entities
{
    public class Contrato
    {
        public int Id { get; set; }
        public string NumeroContrato { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activo { get; set; }
        public virtual Usuario? Usuario { get; set; }
    }
}