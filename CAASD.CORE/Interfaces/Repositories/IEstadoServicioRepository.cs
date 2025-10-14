using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAASD.Core.Entities;


namespace CAASD.Core.Interfaces.Repositories
{
    public interface IEstadoServicioRepository : IRepository<EstadoServicio>
    {
        IEnumerable<EstadoServicio> GetActivos();
        IEnumerable<EstadoServicio> GetByZona(string zona);
        IEnumerable<EstadoServicio> GetBySector(string sector);
    }
}