using System.Collections.Generic;
using CAASD.Core.DTOs;
using CAASD.Core.Entities;
using System;

namespace CAASD.Core.Interfaces.Services
{
    public interface IFacturaService
    {
        // Consultas
        IEnumerable<FacturaDTO> ObtenerFacturasPorUsuario(int usuarioId);
        IEnumerable<FacturaDTO> ObtenerFacturasPendientes(int usuarioId);
        FacturaDTO? ObtenerFacturaPorNumero(string numeroFactura);
        decimal ObtenerTotalAdeudado(int usuarioId);

        // NUEVO: consultas por contrato
        IEnumerable<FacturaDTO> ObtenerFacturasPorContrato(int contratoId);

        // Comandos
        int CrearFactura(Factura nueva);
        bool ActualizarEstado(int facturaId, string nuevoEstado, DateTime? fechaPago = null);

        // NUEVO: edición de montos (recalcula ITBIS y total)
        bool ActualizarMontos(int facturaId, int metrosConsumidos, decimal montoAgua, decimal montoAlcantarillado, decimal montoBasura, bool recalcularItbis = true);

        // NUEVO: anulación lógica
        bool AnularFactura(int facturaId);
    }
}
