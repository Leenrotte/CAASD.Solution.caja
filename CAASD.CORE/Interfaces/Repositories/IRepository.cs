using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CAASD.Core.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
        void Delete(T entity);
        bool Exists(int id);
    }
}