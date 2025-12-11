using System;
using System.Collections.Generic;
using System.Linq;
using Flippit.Api.DAL.Common.Entities.Interfaces;
using Flippit.Api.DAL.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flippit.Api.DAL.EF.Repositories
{
    public class RepositoryBase<TEntity> : IApiRepository<TEntity>, IDisposable
        where TEntity : class, IEntity
    {
        protected readonly FlippitDbContext dbContext;

        public RepositoryBase(FlippitDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual IList<TEntity> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>();

            if (page > 0 && pageSize > 0)
                query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        public virtual TEntity? GetById(Guid id)
        {
            return dbContext.Set<TEntity>().SingleOrDefault(entity => entity.Id == id);
        }

        public virtual Guid Insert(TEntity entity)
        {
            var createdEntity = dbContext.Set<TEntity>().Add(entity);
            dbContext.SaveChanges();

            return createdEntity.Entity.Id;
        }

        public virtual Guid? Update(TEntity entity)
        {
            if (Exists(entity.Id))
            {
                dbContext.Set<TEntity>().Attach(entity);
                var updatedEntity = dbContext.Set<TEntity>().Update(entity);
                dbContext.SaveChanges();

                return updatedEntity.Entity.Id;
            }
            else
            {
                return null;
            }
        }

        public virtual void Remove(Guid id)
        {
            var entity = GetById(id);
            if (entity is not null)
            {
                dbContext.Set<TEntity>().Remove(entity);
                dbContext.SaveChanges();
            }
        }

        public virtual bool Exists(Guid id)
        {
            return dbContext.Set<TEntity>().Any(entity => entity.Id == id);
        }

        public virtual IEnumerable<TEntity> Search(string searchText)
        {
            return dbContext.Set<TEntity>().ToList();
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
