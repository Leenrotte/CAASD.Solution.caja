using System.Collections.Generic;
using System.Linq;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Infrastructure;

namespace CAASD.Core.Repositories
{
    public class ContenidoEducativoRepository : Repository<ContenidoEducativo>, IContenidoEducativoRepository
    {
        public ContenidoEducativoRepository(CaasdCoreDbContext context) : base(context) { }

        public IEnumerable<ContenidoEducativo> GetActivos()
        {
            return _dbSet
                .Where(c => c.Activo)
                .OrderByDescending(c => c.FechaPublicacion)
                .ToList();
        }

        public IEnumerable<ContenidoEducativo> GetByCategoria(string categoria)
        {
            return _dbSet
                .Where(c => c.Categoria == categoria && c.Activo)
                .OrderByDescending(c => c.FechaPublicacion)
                .ToList();
        }
    }
}
