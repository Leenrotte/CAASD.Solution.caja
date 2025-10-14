using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.Entities
{
    public class Factura
    {
        public int Id { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public int ContratoId { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int MetrosConsumidos { get; set; }
        public decimal MontoAgua { get; set; }
        public decimal MontoAlcantarillado { get; set; }
        public decimal MontoBasura { get; set; }
        public decimal Itbis { get; set; }
        public decimal MontoTotal { get; set; }
        public string EstadoPago { get; set; } = string.Empty;
        public DateTime? FechaPago { get; set; }
        public virtual Contrato? Contrato { get; set; }
    }
}