using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Integracion.Data;

namespace CAASD.Integracion.Repositories
{
    public class ContratoRepository(CAASDContext context) : Repository<Contrato>(context), IContratoRepository
    {
    }
}   