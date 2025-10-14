using System;
using System.Collections.Generic;
using System.Linq;
using CAASD.Core.DTOs;
using CAASD.Core.Interfaces;
using CAASD.Core.Interfaces.Services;

namespace CAASD.Integracion.Services
{
    public class FacturaService(IUnitOfWork unitOfWork) : IFacturaService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IEnumerable<FacturaDTO> ObtenerFacturasPorUsuario(int usuarioId)
        {
            try
            {
                var facturas = _unitOfWork.Facturas.GetFacturasPendientesByUsuario(usuarioId);

                return facturas.Select(f => new FacturaDTO
                {
                    Id = f.Id,
                    NumeroFactura = f.NumeroFactura,
                    NumeroContrato = f.Contrato?.NumeroContrato ?? "N/A",
                    FechaEmision = f.FechaEmision,
                    FechaVencimiento = f.FechaVencimiento,
                    MetrosConsumidos = f.MetrosConsumidos,
                    MontoTotal = f.MontoTotal,
                    EstadoPago = f.EstadoPago,
                    EstaPagada = f.EstadoPago == "Pagada"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener facturas: {ex.Message}");
            }
        }

        public FacturaDTO? ObtenerFacturaPorNumero(string numeroFactura)
        {
            try
            {
                var factura = _unitOfWork.Facturas.GetByNumeroFactura(numeroFactura);

                return factura == null ? null : new FacturaDTO
                {
                    Id = factura.Id,
                    NumeroFactura = factura.NumeroFactura,
                    NumeroContrato = factura.Contrato?.NumeroContrato ?? "N/A",
                    FechaEmision = factura.FechaEmision,
                    FechaVencimiento = factura.FechaVencimiento,
                    MetrosConsumidos = factura.MetrosConsumidos,
                    MontoTotal = factura.MontoTotal,
                    EstadoPago = factura.EstadoPago,
                    EstaPagada = factura.EstadoPago == "Pagada"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener factura: {ex.Message}");
            }
        }

        public IEnumerable<FacturaDTO> ObtenerFacturasPendientes(int usuarioId)
        {
            try
            {
                var facturas = _unitOfWork.Facturas.GetFacturasPendientesByUsuario(usuarioId)
                    .Where(f => f.EstadoPago == "Pendiente");

                return facturas.Select(f => new FacturaDTO
                {
                    Id = f.Id,
                    NumeroFactura = f.NumeroFactura,
                    NumeroContrato = f.Contrato?.NumeroContrato ?? "N/A",
                    FechaEmision = f.FechaEmision,
                    FechaVencimiento = f.FechaVencimiento,
                    MetrosConsumidos = f.MetrosConsumidos,
                    MontoTotal = f.MontoTotal,
                    EstadoPago = f.EstadoPago,
                    EstaPagada = false
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener facturas pendientes: {ex.Message}");
            }
        }

        public decimal ObtenerTotalAdeudado(int usuarioId)
        {
            try
            {
                var facturasPendientes = _unitOfWork.Facturas.GetFacturasPendientesByUsuario(usuarioId)
                    .Where(f => f.EstadoPago == "Pendiente");

                return facturasPendientes.Sum(f => f.MontoTotal);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al calcular total adeudado: {ex.Message}");
            }
        }
    }
}