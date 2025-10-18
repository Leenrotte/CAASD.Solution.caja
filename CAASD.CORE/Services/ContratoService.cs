using System.Collections.Generic;
using System.Linq;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Interfaces.Services;

namespace CAASD.Core.Services;

public class ContratoService : IContratoService
{
    private readonly IContratoRepository _repo;
    public ContratoService(IContratoRepository repo) => _repo = repo;

    public IEnumerable<Contrato> ObtenerPorUsuario(int usuarioId, bool soloActivos = true)
        => _repo.GetAll().Where(c => c.UsuarioId == usuarioId && (!soloActivos || c.Activo));

    public void Crear(Contrato contrato) => _repo.Add(contrato);
}
