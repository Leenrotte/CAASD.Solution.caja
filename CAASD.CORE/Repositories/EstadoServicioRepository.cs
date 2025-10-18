using System.Collections.Generic;
using System.Linq;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Infrastructure;

namespace CAASD.Core.Repositories
{
    public class EstadoServicioRepository : Repository<EstadoServicio>, IEstadoServicioRepository
    {
        public EstadoServicioRepository(CaasdCoreDbContext context) : base(context) { }

        public IEnumerable<EstadoServicio> GetActivos()
        {
            return _dbSet
                .Where(e => e.Activo)
                .OrderByDescending(e => e.FechaInicio)
                .ToList();
        }

        public IEnumerable<EstadoServicio> GetByZona(string zona)
        {
            return _dbSet
                .Where(e => e.Zona == zona && e.Activo)
                .OrderByDescending(e => e.FechaInicio)
                .ToList();
        }

        public IEnumerable<EstadoServicio> GetBySector(string sector)
        {
            return _dbSet
                .Where(e => e.Sector == sector && e.Activo)
                .OrderByDescending(e => e.FechaInicio)
                .ToList();
        }
    }
}
