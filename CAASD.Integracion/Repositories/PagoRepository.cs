using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Integracion.Data;

namespace CAASD.Integracion.Repositories
{
    public class PagoRepository(CAASDContext context) : Repository<Pago>(context), IPagoRepository
    {
        public IEnumerable<Pago> GetPagosByFactura(int facturaId)
        {
            return [.. _dbSet
                .Where(p => p.FacturaId == facturaId)
                .Include(p => p.Factura)
                .OrderByDescending(p => p.FechaPago)];
        }

        public Pago? GetByNumeroTransaccion(string numeroTransaccion)
        {
            return _dbSet
                .Include(p => p.Factura)
                .FirstOrDefault(p => p.NumeroTransaccion == numeroTransaccion);
        }

        public IEnumerable<Pago> GetPagosByUsuario(int usuarioId)
        {
            return [.. _dbSet
                .Include(p => p.Factura)
                    .ThenInclude(f => f!.Contrato)
                .Where(p => p.Factura != null && p.Factura.Contrato != null && p.Factura.Contrato.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaPago)];
        }
    }
}