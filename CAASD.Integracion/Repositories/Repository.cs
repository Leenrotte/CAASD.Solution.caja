using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CAASD.Core.Interfaces.Repositories;
using CAASD.Integracion.Data;

namespace CAASD.Integracion.Repositories
{
    public class Repository<T>(CAASDContext context) : IRepository<T> where T : class
    {
        protected readonly CAASDContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

       
        public virtual T? GetById(int id)
        {
            return _dbSet.FirstOrDefault(e => EF.Property<int>(e, "Id") == id);
        }

       
        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate).ToList();
        }

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

       
        public virtual void Delete(int id)
        {
            var entity = _dbSet.FirstOrDefault(e => EF.Property<int>(e, "Id") == id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        public virtual void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

      
        public virtual bool Exists(int id)
        {
            return _dbSet.Any(e => EF.Property<int>(e, "Id") == id);
        }
    }
}
