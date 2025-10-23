using System;
using System.Collections.Generic;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.BL.Mappers;
using Flippit.Common.BL.Facades;
using Flippit.Common.Models.Collection;

namespace Flippit.Api.BL.Facades
{
    public class CollectionFacade : ICollectionFacade
    {
        private readonly ICollectionRepository _repository;
        private readonly CollectionMapper _mapper;

        public CollectionFacade(ICollectionRepository repository, CollectionMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IList<CollectionListModel> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            if (page < 1)
                throw new ArgumentException("Page number must be greater than or equal to 1.", nameof(page));
            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than or equal to 1.", nameof(pageSize));

            var entities = _repository.GetAll(filter, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public CollectionDetailModel? GetById(Guid id)
        {
            var entity = _repository.GetById(id);
            return entity != null ? _mapper.ToDetailModel(entity) : null;
        }

        public IList<CollectionListModel> Search(string searchText)
        {
            var entities = _repository.Search(searchText);
            return _mapper.ToListModels(entities);
        }

        public IList<CollectionListModel> SearchByCreatorId(Guid creatorId)
        {
            var entities = _repository.SearchByCreatorId(creatorId);
            return _mapper.ToListModels(entities);
        }

        public Guid CreateOrUpdate(CollectionDetailModel collectionModel)
        {
            if (collectionModel == null)
                throw new ArgumentNullException(nameof(collectionModel));
            if (string.IsNullOrWhiteSpace(collectionModel.Name))
                throw new ArgumentException("Collection name cannot be empty.", nameof(collectionModel.Name));

            var entity = _mapper.ModelToEntity(collectionModel);
            return _repository.Exists(entity.Id) ? _repository.Update(entity) ?? entity.Id : _repository.Insert(entity);
        }

        public Guid Create(CollectionDetailModel collectionModel)
        {
            if (collectionModel == null)
                throw new ArgumentNullException(nameof(collectionModel));
            if (string.IsNullOrWhiteSpace(collectionModel.Name))
                throw new ArgumentException("Collection name cannot be empty.", nameof(collectionModel.Name));

            var entity = _mapper.ModelToEntity(collectionModel);
            return _repository.Insert(entity);
        }

        public Guid? Update(CollectionDetailModel collectionModel)
        {
            if (collectionModel == null)
                throw new ArgumentNullException(nameof(collectionModel));
            if (string.IsNullOrWhiteSpace(collectionModel.Name))
                throw new ArgumentException("Collection name cannot be empty.", nameof(collectionModel.Name));

            var entity = _mapper.ModelToEntity(collectionModel);
            return _repository.Update(entity);
        }

        public void Delete(Guid id)
        {
            _repository.Remove(id);
        }
    }
}
