using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.DTOs
{
    public class FacturaDTO
    {
        public int Id { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public string NumeroContrato { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int MetrosConsumidos { get; set; }
        public decimal MontoTotal { get; set; }
        public string EstadoPago { get; set; } = string.Empty;
        public bool EstaPagada { get; set; }
    }
}