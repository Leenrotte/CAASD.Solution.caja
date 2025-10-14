using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAASD.Core.Entities;
using System.Collections.Generic;

namespace CAASD.Core.Interfaces.Repositories
{
    public interface IPagoRepository : IRepository<Pago>
    {
        IEnumerable<Pago> GetPagosByFactura(int facturaId);
        Pago? GetByNumeroTransaccion(string numeroTransaccion);
        IEnumerable<Pago> GetPagosByUsuario(int usuarioId);
    }
}