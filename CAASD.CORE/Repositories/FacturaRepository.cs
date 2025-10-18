using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Infrastructure;

namespace CAASD.Core.Repositories
{
    public class FacturaRepository : Repository<Factura>, IFacturaRepository
    {
        public FacturaRepository(CaasdCoreDbContext context) : base(context) { }

        public IEnumerable<Factura> GetFacturasByContrato(int contratoId)
        {
            return _dbSet
                .Where(f => f.ContratoId == contratoId)
                .Include(f => f.Contrato)
                .OrderByDescending(f => f.FechaEmision)
                .ToList();
        }

        public IEnumerable<Factura> GetFacturasPendientesByUsuario(int usuarioId)
        {
            return _dbSet
                .Include(f => f.Contrato)
                .Where(f => f.Contrato != null &&
                           f.Contrato.UsuarioId == usuarioId &&
                           f.EstadoPago == "Pendiente")
                .OrderBy(f => f.FechaVencimiento)
                .ToList();
        }

        public Factura? GetByNumeroFactura(string numeroFactura)
        {
            return _dbSet
                .Include(f => f.Contrato)
                .FirstOrDefault(f => f.NumeroFactura == numeroFactura);
        }
    }
}
