// CAASD.CORE/Repositories/ContratoRepository.cs
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Infrastructure;

namespace CAASD.Core.Repositories
{
    public class ContratoRepository : Repository<Contrato>, IContratoRepository
    {
        public ContratoRepository(CaasdCoreDbContext context) : base(context) { }
    }
}
