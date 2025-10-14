using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAASD.Core.Entities;


namespace CAASD.Core.Interfaces.Repositories
{
    public interface ISolicitudRepository : IRepository<Solicitud>
    {
        IEnumerable<Solicitud> GetByUsuario(int usuarioId);
        IEnumerable<Solicitud> GetByEstado(string estado);
        IEnumerable<Solicitud> GetPendientes();
    }
}