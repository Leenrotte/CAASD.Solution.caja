using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAASD.Core.Entities;


namespace CAASD.Core.Interfaces.Repositories
{
    public interface IContenidoEducativoRepository : IRepository<ContenidoEducativo>
    {
        IEnumerable<ContenidoEducativo> GetActivos();
        IEnumerable<ContenidoEducativo> GetByCategoria(string categoria);
    }
}