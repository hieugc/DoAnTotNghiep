using DoAnTotNghiep.Data;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Service
{
    public class BaseService<T> where T : class
    {
        protected readonly DoAnTotNghiepContext context;
        DbSet<T> dbset;
        public BaseService(DoAnTotNghiepContext context, DbSet<T> dbset)
        {
            this.context = context;
            this.dbset = dbset;
        }

        public virtual void Add(T entity)
        {
            dbset.Add(entity);
            context.SaveChanges();
        }

        public virtual void Update(T entity)
        {
            dbset.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        public virtual IEnumerable<T> All()
        {
            return dbset;
        }

        public virtual T? GetById(params object[] keyValues)
        {
            return dbset.Find(keyValues);
        }

        public virtual void Delete(T entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbset.Attach(entity);
            }
            dbset.Remove(entity);
            context.SaveChanges();
        }
    }

    public interface IBaseService<T> where T : class
    {
        void Add(T entity);

        void Update(T entity);

        IEnumerable<T> GetAll();

        T? GetById(params object[] keyValues);

        void Delete(T entity);
    }
}
