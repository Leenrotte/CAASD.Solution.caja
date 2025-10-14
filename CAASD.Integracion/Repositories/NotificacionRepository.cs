using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Integracion.Data;

namespace CAASD.Integracion.Repositories
{
    public class NotificacionRepository(CAASDContext context) : Repository<Notificacion>(context), INotificacionRepository
    {
        public IEnumerable<Notificacion> GetByUsuario(int usuarioId)
        {
            return [.. _dbSet
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.FechaEnvio)];
        }

        public IEnumerable<Notificacion> GetNoLeidasByUsuario(int usuarioId)
        {
            return [.. _dbSet
                .Where(n => n.UsuarioId == usuarioId && !n.Leida)
                .OrderByDescending(n => n.FechaEnvio)];
        }

        public int ContarNoLeidasByUsuario(int usuarioId)
        {
            return _dbSet.Count(n => n.UsuarioId == usuarioId && !n.Leida);
        }
    }
}