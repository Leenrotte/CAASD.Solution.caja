using System.Collections.Generic;
using System.Linq;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Integracion.Data;

namespace CAASD.Integracion.Repositories
{
    public class ContenidoEducativoRepository(CAASDContext context) : Repository<ContenidoEducativo>(context), IContenidoEducativoRepository
    {
        public IEnumerable<ContenidoEducativo> GetActivos()
        {
            return [.. _dbSet
                .Where(c => c.Activo)
                .OrderByDescending(c => c.FechaPublicacion)];
        }

        public IEnumerable<ContenidoEducativo> GetByCategoria(string categoria)
        {
            return [.. _dbSet
                .Where(c => c.Categoria == categoria && c.Activo)
                .OrderByDescending(c => c.FechaPublicacion)];
        }
    }
}