// Flippit.Api.DAL.Memory.Repositories/CollectionRepository.cs
using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Mappers;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.DAL.Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Flippit.Api.DAL.Memory.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly IList<CollectionEntity> _collections;
        private readonly CollectionMapper _collectionMapper;

        public CollectionRepository(Storage storage, CollectionMapper collectionMapper)
        {
            _collections = storage.Collections;
            _collectionMapper = collectionMapper;
        }

        public IList<CollectionEntity> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            return _collections.AsQueryable()
                               .ApplyFilterSortAndPage(filter, sortBy, page, pageSize)
                               .ToList();
        }

        public CollectionEntity? GetById(Guid id)
        {
            return _collections.SingleOrDefault(c => c.Id == id);
        }

        public IEnumerable<CollectionEntity> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _collections;

            return _collections.Where(c => c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<CollectionEntity> SearchByCreatorId(Guid creatorId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            return _collections
                .Where(c => c.CreatorId == creatorId)
                .AsQueryable()
                .ApplyFilterSortAndPage(filter, sortBy, page, pageSize)
                .ToList();
        }

        public Guid Insert(CollectionEntity entity)
        {
            _collections.Add(entity);
            return entity.Id;
        }

        public Guid? Update(CollectionEntity collectionUpdated)
        {
            var collectionExisting = _collections.SingleOrDefault(c => c.Id == collectionUpdated.Id);
            if (collectionExisting == null)
                return null;

            _collectionMapper.UpdateEntity(collectionUpdated, collectionExisting);
            return collectionExisting.Id;
        }

        public void Remove(Guid id)
        {
            var collectionToRemove = _collections.SingleOrDefault(c => c.Id == id);
            if (collectionToRemove != null)
            {
                _collections.Remove(collectionToRemove);
            }
        }

        public bool Exists(Guid id)
        {
            return _collections.Any(c => c.Id == id);
        }
    }
}
