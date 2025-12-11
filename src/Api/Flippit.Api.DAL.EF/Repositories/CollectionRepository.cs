using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Mappers;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.DAL.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flippit.Api.DAL.EF.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly FlippitDbContext _dbContext;
        private readonly CollectionMapper _collectionMapper;

        public CollectionRepository(FlippitDbContext dbContext, CollectionMapper mapper)
        {
            _dbContext = dbContext;
            _collectionMapper = mapper;
        }

        public IList<CollectionEntity> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            return _dbContext.Collections
                .AsQueryable()
                .ApplyFilterSortAndPage(filter, sortBy, page, pageSize)
                .Skip((page-1)*pageSize).Take(pageSize)
                .ToList();
        }

        public CollectionEntity? GetById(Guid id)
        {
            return _dbContext.Collections.SingleOrDefault(c => c.Id == id);
        }

        public IEnumerable<CollectionEntity> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _dbContext.Collections.ToList();

            return _dbContext.Collections
                .Where(c => c.Name.Contains(searchText))
                .ToList();
        }

        public IEnumerable<CollectionEntity> SearchByCreatorId(Guid creatorId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var query = _dbContext.Collections.Where(c => c.CreatorId == creatorId)
                .ApplyFilterSortAndPage(filter, sortBy, page, pageSize)
                .Skip((page-1)*pageSize).Take(pageSize);

            return query.ToList();
        }

        public Guid Insert(CollectionEntity entity)
        {
            _dbContext.Collections.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public Guid? Update(CollectionEntity collectionUpdated)
        {
            var collectionExisting = _dbContext.Collections.SingleOrDefault(c => c.Id == collectionUpdated.Id);
            if (collectionExisting == null)
                return null;

            _collectionMapper.UpdateEntity(collectionUpdated, collectionExisting);
            _dbContext.Collections.Update(collectionExisting);
            _dbContext.SaveChanges();
            return collectionExisting.Id;
        }

        public void Remove(Guid id)
        {
            var collectionToRemove = _dbContext.Collections.SingleOrDefault(c => c.Id == id);
            if (collectionToRemove != null)
            {
                _dbContext.Collections.Remove(collectionToRemove);
                _dbContext.SaveChanges();
            }
        }

        public bool Exists(Guid id)
        {
            return _dbContext.Collections.Any(c => c.Id == id);
        }
    }
}
