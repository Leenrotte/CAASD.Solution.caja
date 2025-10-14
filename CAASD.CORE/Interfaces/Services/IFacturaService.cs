using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAASD.Core.DTOs;


namespace CAASD.Core.Interfaces.Services
{
    public interface IFacturaService
    {
        IEnumerable<FacturaDTO> ObtenerFacturasPorUsuario(int usuarioId);
        FacturaDTO? ObtenerFacturaPorNumero(string numeroFactura);
        IEnumerable<FacturaDTO> ObtenerFacturasPendientes(int usuarioId);
        decimal ObtenerTotalAdeudado(int usuarioId);
    }
}