using System.Collections.Generic;
using System.Linq;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Integracion.Data;

namespace CAASD.Integracion.Repositories
{
    public class EstadoServicioRepository(CAASDContext context) : Repository<EstadoServicio>(context), IEstadoServicioRepository
    {
        public IEnumerable<EstadoServicio> GetActivos()
        {
            return [.. _dbSet
                .Where(e => e.Activo)
                .OrderByDescending(e => e.FechaInicio)];
        }

        public IEnumerable<EstadoServicio> GetByZona(string zona)
        {
            return [.. _dbSet
                .Where(e => e.Zona == zona && e.Activo)
                .OrderByDescending(e => e.FechaInicio)];
        }

        public IEnumerable<EstadoServicio> GetBySector(string sector)
        {
            return [.. _dbSet
                .Where(e => e.Sector == sector && e.Activo)
                .OrderByDescending(e => e.FechaInicio)];
        }
    }
}