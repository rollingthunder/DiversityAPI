using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DiversityService.API.WebHost
{
    public interface IContext : IDisposable
    {
        int SaveChanges();
    }

    public interface IUnitOfWork : IDisposable
    {
        int Save();
        IContext Context { get; }
    }

    public interface IRepository<T> where T : class, IIdentifiable
    {
        IQueryable<T> All { get; }    
        IQueryable<T> AllEager(params Expression<Func<T, object>>[] includes); 
        T Find(int id); 
        void Insert(T entity); 
        void Update(T entity); 
        void Delete(int id);
    }

    public interface IRepositoryFactory
    {
        IRepository<T> Get<T>() where T: class, IIdentifiable;
    }
}
