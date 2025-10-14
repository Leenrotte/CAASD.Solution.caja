using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.DTOs
{
    public class PagoDTO
    {
        public int Id { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string NumeroTransaccion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}