using System;
using CAASD.Core.Constants;
using CAASD.Core.DTOs;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Interfaces.Services;

namespace CAASD.Core.Services;

public class PagoService : IPagoService
{
    private readonly IPagoRepository _repo;
    private readonly IFacturaRepository _facturas;
    public PagoService(IPagoRepository repo, IFacturaRepository facturas) { _repo = repo; _facturas = facturas; }

    public PagoDTO? ObtenerDetallePago(int pagoId)
        => _repo.GetById(pagoId) is { } p ? Map(p) : null;

    public bool VerificarPago(string numeroTransaccion)
        => _repo.GetByNumeroTransaccion(numeroTransaccion) != null;

    public bool ProcesarPago(int facturaId, decimal monto, string metodoPago)
    {
        var factura = _facturas.GetById(facturaId);
        if (factura == null) return false;

        var pago = new Pago
        {
            FacturaId = facturaId,
            Monto = monto,
            MetodoPago = metodoPago,
            NumeroTransaccion = $"TX-{Guid.NewGuid():N}".Substring(0, 12),
            FechaPago = DateTime.Now,
            Estado = AppConstants.PAGO_COMPLETADO
        };
        _repo.Add(pago);

        if (monto >= factura.MontoTotal)
        {
            factura.EstadoPago = "Pagada";
            _facturas.Update(factura);
        }
        return true;
    }

    private static PagoDTO Map(Pago p) => new()
    {
        Id = p.Id,
        NumeroFactura = p.Factura?.NumeroFactura ?? "",
        MetodoPago = p.MetodoPago,
        Monto = p.Monto,
        NumeroTransaccion = p.NumeroTransaccion,
        FechaPago = p.FechaPago,
        Estado = p.Estado
    };
}
