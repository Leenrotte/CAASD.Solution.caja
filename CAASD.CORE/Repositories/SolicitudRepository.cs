using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Infrastructure;

namespace CAASD.Core.Repositories
{
    public class SolicitudRepository : Repository<Solicitud>, ISolicitudRepository
    {
        public SolicitudRepository(CaasdCoreDbContext context) : base(context) { }

        public IEnumerable<Solicitud> GetByUsuario(int usuarioId)
        {
            return _dbSet
                .Where(s => s.UsuarioId == usuarioId)
                .Include(s => s.Usuario)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToList();
        }

        public IEnumerable<Solicitud> GetByEstado(string estado)
        {
            return _dbSet
                .Where(s => s.Estado == estado)
                .Include(s => s.Usuario)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToList();
        }

        public IEnumerable<Solicitud> GetPendientes()
        {
            return _dbSet
                .Where(s => s.Estado == "Pendiente")
                .Include(s => s.Usuario)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToList();
        }
    }
}
