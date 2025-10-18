using System.Collections.Generic;
using System.Linq;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Infrastructure;

namespace CAASD.Core.Repositories
{
    public class NotificacionRepository : Repository<Notificacion>, INotificacionRepository
    {
        public NotificacionRepository(CaasdCoreDbContext context) : base(context) { }

        public IEnumerable<Notificacion> GetByUsuario(int usuarioId)
        {
            return _dbSet
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.FechaEnvio)
                .ToList();
        }

        public IEnumerable<Notificacion> GetNoLeidasByUsuario(int usuarioId)
        {
            return _dbSet
                .Where(n => n.UsuarioId == usuarioId && !n.Leida)
                .OrderByDescending(n => n.FechaEnvio)
                .ToList();
        }

        public int ContarNoLeidasByUsuario(int usuarioId)
        {
            return _dbSet.Count(n => n.UsuarioId == usuarioId && !n.Leida);
        }
    }
}
