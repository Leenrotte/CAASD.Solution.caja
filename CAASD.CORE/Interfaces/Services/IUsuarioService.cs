using System.Collections.Generic;
using CAASD.Core.Entities;

namespace CAASD.Core.Interfaces.Services
{
    public interface IUsuarioService
    {
        IEnumerable<Usuario> ObtenerTodos();
        Usuario? ObtenerPorId(int id);
        Usuario? ObtenerPorEmail(string email);
        void Crear(Usuario usuario, string passwordPlano);
        void Actualizar(Usuario usuario);
      
    }
}
