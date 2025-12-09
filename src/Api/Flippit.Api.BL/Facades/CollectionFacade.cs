using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.BL.Mappers;
using Flippit.Api.BL.Services;
using Flippit.Common.Models.Collection;
using Flippit.Api.BL.Validators;

namespace Flippit.Api.BL.Facades
{
    public class CollectionFacade : ICollectionFacade
    {
        private readonly ICollectionRepository _repository;
        private readonly CollectionMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public CollectionFacade(ICollectionRepository repository, CollectionMapper mapper, ICurrentUserService currentUserService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
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

        public IList<CollectionListModel> SearchByCreatorId(Guid creatorId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var entities = _repository.SearchByCreatorId(creatorId, filter, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public Guid CreateOrUpdate(CollectionDetailModel collectionModel)
        {
            ValidateCollectionModel(collectionModel);
            var entity = _mapper.ModelToEntity(collectionModel);
            entity = entity with { CreatorId = EnsureAuthenticatedUserId() };
            return _repository.Exists(entity.Id) ? _repository.Update(entity) ?? entity.Id : _repository.Insert(entity);
        }

        public Guid Create(CollectionDetailModel collectionModel)
        {
            ValidateCollectionModel(collectionModel);
            var entity = _mapper.ModelToEntity(collectionModel);
            entity = entity with { CreatorId = EnsureAuthenticatedUserId() };
            return _repository.Insert(entity);
        }

        public Guid? Update(CollectionDetailModel collectionModel)
        {
            ValidateCollectionModel(collectionModel);
            var entity = _mapper.ModelToEntity(collectionModel);
            entity = entity with { CreatorId = EnsureAuthenticatedUserId() };
            return _repository.Update(entity);
        }

        public void Delete(Guid id)
        {
            _repository.Remove(id);
        }

        private static void ValidateCollectionModel(CollectionDetailModel collectionModel)
        {
            if (collectionModel == null)
                throw new ArgumentNullException(nameof(collectionModel));

            if (string.IsNullOrWhiteSpace(collectionModel.Name))
                throw new ArgumentException("Collection name cannot be empty.", nameof(collectionModel.Name));
        }

        private Guid EnsureAuthenticatedUserId()
        {
            var userId = _currentUserService.CurrentUserId;
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User must be authenticated to perform this operation.");
            return userId.Value;
        }
    }
}
