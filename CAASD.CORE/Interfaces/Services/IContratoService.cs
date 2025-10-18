// IContratoService.cs
using System.Collections.Generic;
using CAASD.Core.Entities;

namespace CAASD.Core.Interfaces.Services
{
    public interface IContratoService
    {
        IEnumerable<Contrato> ObtenerPorUsuario(int usuarioId, bool soloActivos = true);
        void Crear(Contrato contrato);
    }
}
