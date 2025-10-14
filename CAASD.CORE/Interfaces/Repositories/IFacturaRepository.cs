using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAASD.Core.Entities;

namespace CAASD.Core.Interfaces.Repositories
{
    public interface IFacturaRepository : IRepository<Factura>
    {
        IEnumerable<Factura> GetFacturasByContrato(int contratoId);
        IEnumerable<Factura> GetFacturasPendientesByUsuario(int usuarioId);
        Factura? GetByNumeroFactura(string numeroFactura);
    }
}