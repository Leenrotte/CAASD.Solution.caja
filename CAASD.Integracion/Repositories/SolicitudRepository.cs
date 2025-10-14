using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Integracion.Data;

namespace CAASD.Integracion.Repositories
{
    public class SolicitudRepository(CAASDContext context) : Repository<Solicitud>(context), ISolicitudRepository
    {
        public IEnumerable<Solicitud> GetByUsuario(int usuarioId)
        {
            return [.. _dbSet
                .Where(s => s.UsuarioId == usuarioId)
                .Include(s => s.Usuario)
                .OrderByDescending(s => s.FechaSolicitud)];
        }

        public IEnumerable<Solicitud> GetByEstado(string estado)
        {
            return [.. _dbSet
                .Where(s => s.Estado == estado)
                .Include(s => s.Usuario)
                .OrderByDescending(s => s.FechaSolicitud)];
        }

        public IEnumerable<Solicitud> GetPendientes()
        {
            return [.. _dbSet
                .Where(s => s.Estado == "Pendiente")
                .Include(s => s.Usuario)
                .OrderByDescending(s => s.FechaSolicitud)];
        }
    }
}