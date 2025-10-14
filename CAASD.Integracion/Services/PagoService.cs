using System;
using CAASD.Core.DTOs;
using CAASD.Core.Entities;
using CAASD.Core.Exceptions;
using CAASD.Core.Interfaces;
using CAASD.Core.Interfaces.Services;

namespace CAASD.Integracion.Services
{
    public class PagoService(IUnitOfWork unitOfWork) : IPagoService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public bool ProcesarPago(int facturaId, decimal monto, string metodoPago)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var factura = _unitOfWork.Facturas.GetById(facturaId);

                if (factura == null)
                {
                    throw new PagoException("Factura no encontrada");
                }

                if (factura.EstadoPago == "Pagada")
                {
                    throw new PagoException("La factura ya está pagada");
                }

                if (monto != factura.MontoTotal)
                {
                    throw new PagoException($"El monto debe ser exactamente {factura.MontoTotal:C}");
                }

                var pago = new Pago
                {
                    FacturaId = facturaId,
                    Monto = monto,
                    FechaPago = DateTime.Now,
                    MetodoPago = metodoPago,
                    NumeroTransaccion = $"TRX-{DateTime.Now:yyyyMMddHHmmss}-{facturaId}",
                    Estado = "Completado",
                    ReferenciaExterna = Guid.NewGuid().ToString()
                };

                _unitOfWork.Pagos.Add(pago);

                factura.EstadoPago = "Pagada";
                factura.FechaPago = DateTime.Now;
                _unitOfWork.Facturas.Update(factura);

                _unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw new PagoException($"Error al procesar pago: {ex.Message}");
            }
        }

        public bool VerificarPago(string numeroTransaccion)
        {
            try
            {
                var pago = _unitOfWork.Pagos.GetByNumeroTransaccion(numeroTransaccion);
                return pago != null && pago.Estado == "Completado";
            }
            catch
            {
                return false;
            }
        }

        public PagoDTO? ObtenerDetallePago(int pagoId)
        {
            try
            {
                var pago = _unitOfWork.Pagos.GetById(pagoId);

                if (pago == null)
                    return null;

                return new PagoDTO
                {
                    Id = pago.Id,
                    NumeroFactura = pago.Factura?.NumeroFactura ?? "N/A",
                    Monto = pago.Monto,
                    FechaPago = pago.FechaPago,
                    MetodoPago = pago.MetodoPago,
                    NumeroTransaccion = pago.NumeroTransaccion,
                    Estado = pago.Estado
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener detalle de pago: {ex.Message}");
            }
        }
    }
}