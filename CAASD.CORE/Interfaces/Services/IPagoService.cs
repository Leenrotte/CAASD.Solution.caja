using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAASD.Core.DTOs;

namespace CAASD.Core.Interfaces.Services
{
    public interface IPagoService
    {
        bool ProcesarPago(int facturaId, decimal monto, string metodoPago);
        bool VerificarPago(string numeroTransaccion);
        PagoDTO? ObtenerDetallePago(int pagoId);
    }
}