using System;
using System.Collections.Generic;
using Flippit.Api.DAL.Common.Entities.Interfaces;

namespace Flippit.Api.DAL.Common.Repositories
{
    public interface IApiRepository<TEntity>
        where TEntity : IEntity
    {
        IList<TEntity> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        TEntity? GetById(Guid id);
        Guid Insert(TEntity entity);
        Guid? Update(TEntity entity);
        void Remove(Guid id);
        bool Exists(Guid id);
        IEnumerable<TEntity> Search(string searchText);
    }
}
