using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.BL.Mappers;
using Flippit.Common.Models.Collection;
using Flippit.Api.BL.Validators;
using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.BL.Facades
{
    public class CollectionFacade : FacadeBase<ICollectionRepository, CollectionEntity>, ICollectionFacade
    {
        private readonly ICollectionRepository _repository;
        private readonly CollectionMapper _mapper;

        public CollectionFacade(ICollectionRepository repository, CollectionMapper mapper) : base(repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IList<CollectionListModel> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            PaginationValidator.Validate(page, pageSize);
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

        public IList<CollectionListModel> SearchByOwnerId(string ownerId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var entities = _repository.SearchByOwnerId(ownerId, filter, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public Guid CreateOrUpdate(CollectionDetailModel collectionModel, IList<string> userRoles, string? userId)
        {
            ValidateCollectionModel(collectionModel);
            var entity = _mapper.ModelToEntity(collectionModel);
            entity = entity with { OwnerId = userId };
            return _repository.Exists(entity.Id) ? _repository.Update(entity) ?? entity.Id : _repository.Insert(entity);
        }

        public Guid Create(CollectionDetailModel collectionModel, IList<string> userRoles, string? userId)
        {
            ValidateCollectionModel(collectionModel);
            var entity = _mapper.ModelToEntity(collectionModel);
            entity = entity with { OwnerId = userId };
            return _repository.Insert(entity);
        }

        public Guid? Update(CollectionDetailModel collectionModel, IList<string> userRoles, string? userId)
        {
            ValidateCollectionModel(collectionModel);
            ThrowIfWrongOwnerAndNotAdmin(collectionModel.Id, userRoles, userId);
            var entity = _mapper.ModelToEntity(collectionModel);
            entity = entity with { OwnerId = userId };
            return _repository.Update(entity);
        }

        public void Delete(Guid id, IList<string> userRoles, string? userId)
        {
            ThrowIfWrongOwnerAndNotAdmin(id, userRoles, userId);
            _repository.Remove(id);
        }

        private static void ValidateCollectionModel(CollectionDetailModel collectionModel)
        {
            if (collectionModel == null)
                throw new ArgumentNullException(nameof(collectionModel));

            if (string.IsNullOrWhiteSpace(collectionModel.Name))
                throw new ArgumentException("Collection name cannot be empty.", nameof(collectionModel.Name));
        }
    }
}
