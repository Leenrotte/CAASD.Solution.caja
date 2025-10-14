using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAASD.Core.Entities;


namespace CAASD.Core.Interfaces.Repositories
{
    public interface INotificacionRepository : IRepository<Notificacion>
    {
        IEnumerable<Notificacion> GetByUsuario(int usuarioId);
        IEnumerable<Notificacion> GetNoLeidasByUsuario(int usuarioId);
        int ContarNoLeidasByUsuario(int usuarioId);
    }
}
