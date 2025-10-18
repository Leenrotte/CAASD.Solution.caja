using System;
using System.Collections.Generic;
using System.Linq;
using CAASD.Core.DTOs;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Interfaces.Services;
using CAASD.Core.Constants;

namespace CAASD.Core.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly IFacturaRepository _repo;
        public FacturaService(IFacturaRepository repo) => _repo = repo;

        // ===== QUERIES =====
        public IEnumerable<FacturaDTO> ObtenerFacturasPendientes(int usuarioId)
            => _repo.GetFacturasPendientesByUsuario(usuarioId).Select(Map);

        public IEnumerable<FacturaDTO> ObtenerFacturasPorUsuario(int usuarioId)
            => _repo.GetAll()
                    .Where(f => f.Contrato != null && f.Contrato.UsuarioId == usuarioId)
                    .Select(Map);

        public IEnumerable<FacturaDTO> ObtenerFacturasPorContrato(int contratoId)
            => _repo.GetAll()
                    .Where(f => f.ContratoId == contratoId)
                    .OrderByDescending(f => f.FechaEmision)
                    .Select(Map);

        public FacturaDTO? ObtenerFacturaPorNumero(string numeroFactura)
            => _repo.GetByNumeroFactura(numeroFactura) is { } f ? Map(f) : null;

        public decimal ObtenerTotalAdeudado(int usuarioId)
            => _repo.GetFacturasPendientesByUsuario(usuarioId).Sum(f => f.MontoTotal);

        // ===== COMMANDS =====
        public int CrearFactura(Factura nueva)
        {
            if (string.IsNullOrWhiteSpace(nueva.NumeroFactura))
            {
                var sufijo = DateTime.UtcNow.ToString("yyyyMM");
                var aleatorio = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
                nueva.NumeroFactura = $"FAC-{sufijo}-{aleatorio}";
            }

            if (nueva.FechaEmision == default)
                nueva.FechaEmision = DateTime.Now;
            if (nueva.FechaVencimiento == default)
                nueva.FechaVencimiento = nueva.FechaEmision.AddDays(AppConstants.DIAS_VENCIMIENTO_FACTURA);

            var subtotal = (nueva.MontoAgua) + (nueva.MontoAlcantarillado) + (nueva.MontoBasura);
            if (nueva.Itbis <= 0)
                nueva.Itbis = Math.Round(subtotal * AppConstants.ITBIS_PORCENTAJE, 2, MidpointRounding.AwayFromZero);

            nueva.MontoTotal = Math.Round(subtotal + nueva.Itbis, 2, MidpointRounding.AwayFromZero);

            if (string.IsNullOrWhiteSpace(nueva.EstadoPago))
                nueva.EstadoPago = "Pendiente";

            _repo.Add(nueva);
            return nueva.Id;
        }

        public bool ActualizarEstado(int facturaId, string nuevoEstado, DateTime? fechaPago = null)
        {
            var f = _repo.GetById(facturaId);
            if (f == null) return false;

            f.EstadoPago = nuevoEstado;
            if (fechaPago.HasValue) f.FechaPago = fechaPago.Value;
            _repo.Update(f);
            return true;
        }

        public bool ActualizarMontos(int facturaId, int metrosConsumidos, decimal montoAgua, decimal montoAlcantarillado, decimal montoBasura, bool recalcularItbis = true)
        {
            var f = _repo.GetById(facturaId);
            if (f == null) return false;

            f.MetrosConsumidos = metrosConsumidos;
            f.MontoAgua = montoAgua;
            f.MontoAlcantarillado = montoAlcantarillado;
            f.MontoBasura = montoBasura;

            var subtotal = montoAgua + montoAlcantarillado + montoBasura;
            if (recalcularItbis) f.Itbis = Math.Round(subtotal * AppConstants.ITBIS_PORCENTAJE, 2, MidpointRounding.AwayFromZero);
            f.MontoTotal = Math.Round(subtotal + f.Itbis, 2, MidpointRounding.AwayFromZero);

            _repo.Update(f);
            return true;
        }

        public bool AnularFactura(int facturaId)
        {
            var f = _repo.GetById(facturaId);
            if (f == null) return false;
            f.EstadoPago = "Anulada";
            _repo.Update(f);
            return true;
        }

        private static FacturaDTO Map(Factura f) => new()
        {
            Id = f.Id,
            NumeroFactura = f.NumeroFactura,
            NumeroContrato = f.Contrato?.NumeroContrato ?? "",
            FechaEmision = f.FechaEmision,
            FechaVencimiento = f.FechaVencimiento,
            MetrosConsumidos = f.MetrosConsumidos,
            MontoTotal = f.MontoTotal,
            EstadoPago = f.EstadoPago,
            EstaPagada = string.Equals(f.EstadoPago, "Pagada", StringComparison.OrdinalIgnoreCase)
        };
    }
}
