using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAASD.Core.Entities;

namespace CAASD.Core.Interfaces.Repositories
{
    public interface IReporteAveriaRepository : IRepository<ReporteAveria>
    {
        IEnumerable<ReporteAveria> GetAveriasByUsuario(int usuarioId);
        IEnumerable<ReporteAveria> GetAveriasByEstado(string estado);
        IEnumerable<ReporteAveria> GetAveriasByZona(decimal latitud, decimal longitud, double radioKm);
    }
}