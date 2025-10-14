using System.Linq;
using Microsoft.EntityFrameworkCore;
using CAASD.Core.Entities;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Integracion.Data;

namespace CAASD.Integracion.Repositories
{
    public class UsuarioRepository(CAASDContext context) : Repository<Usuario>(context), IUsuarioRepository
    {
        public Usuario? GetByEmail(string email)
        {
            return _dbSet
                .Include(u => u.Rol) 
                .FirstOrDefault(u => u.Email == email);
        }

        public Usuario? GetByCedula(string cedula)
        {
            return _dbSet
                .Include(u => u.Rol) 
                .FirstOrDefault(u => u.Cedula == cedula);
        }

        public bool ValidarCredenciales(string email, string passwordHash)
        {
            return _dbSet.Any(u => u.Email == email &&
                                   u.PasswordHash == passwordHash &&
                                   u.Activo);
        }
    }
}