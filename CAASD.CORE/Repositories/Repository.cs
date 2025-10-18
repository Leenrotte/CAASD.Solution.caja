// CAASD.CORE/Repositories/Repository.cs
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Core.Infrastructure;

namespace CAASD.Core.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly CaasdCoreDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(CaasdCoreDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // ======= Lectura =======
        public virtual IEnumerable<T> GetAll()
            => _dbSet.AsNoTracking().ToList();

        public virtual T? GetById(int id)
            => _dbSet.Find(id);

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
            => _dbSet.AsNoTracking().Where(predicate).ToList();

        public virtual bool Exists(int id)
        {
            // Nota: no conocemos el nombre de la PK genéricamente sin metadatos,
            // usar Find es lo más simple y eficiente si la entidad está configurada con PK.
            var entity = _dbSet.Find(id);
            if (entity == null) return false;

            // Desacoplar para no dejar la entidad trackeada por accidente
            _context.Entry(entity).State = EntityState.Detached;
            return true;
        }

        // ======= Escritura (persisten de inmediato) =======
        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) return;
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}
