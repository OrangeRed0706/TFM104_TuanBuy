using System;
using System.Linq;
using System.Linq.Expressions;
using Data;
using Microsoft.EntityFrameworkCore;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;

namespace TuanBuy.Models
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DbContext _context { get; set; }

        public GenericRepository(TuanBuyContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("Context");
            }
            _context = context;
        }

        public void Create(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                _context.Set<TEntity>().Add(instance);
                SaveChanges();
            }
        }

        public void Update(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                _context.Entry(instance).State = EntityState.Modified;
                SaveChanges();
            }
        }

        public void Delete(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                this._context.Entry(instance).State = EntityState.Deleted;
                this.SaveChanges();
            }
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return this._context.Set<TEntity>().FirstOrDefault(predicate);
        }
        
        public IQueryable<TEntity> GetAll()
        {
            return this._context.Set<TEntity>().AsQueryable();
        }

        public void SaveChanges()
        {
            this._context.SaveChanges();
        }

        public void Dispose()
        
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._context != null)
                {
                    this._context.Dispose();
                    this._context = null;
                }
            }
        }
    }
}