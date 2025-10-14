using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CAASD.Core.Entities;

namespace CAASD.Core.Interfaces.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Usuario? GetByEmail(string email);
        Usuario? GetByCedula(string cedula);
        bool ValidarCredenciales(string email, string passwordHash);
    }
}