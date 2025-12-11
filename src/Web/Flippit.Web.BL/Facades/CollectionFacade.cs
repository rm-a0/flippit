using Flippit.Common.Models.Collection;
using Flippit.Web.BL.Mappers;
using Flippit.Web.DAL.Repositories;

namespace Flippit.Web.BL.Facades
{
    public class CollectionFacade : ICollectionFacade
    {
        private readonly CollectionRepository _repository;
        private readonly CollectionMapper _mapper;
        private readonly LocalDbOptions _localDbOptions;
        private readonly IApiApiClient _apiClient;
        private readonly ICollectionsApiClient _collectionsApiClient;
        private readonly ApiModelMapper _apiMapper;
        private readonly IUsersApiClient _userClient;

        public CollectionFacade(
            CollectionRepository repository, 
            CollectionMapper mapper,
            LocalDbOptions localDbOptions,
            IApiApiClient apiClient,
            ICollectionsApiClient collectionsApiClient,
            IUsersApiClient userClient,
            ApiModelMapper apiMapper)
        {
            _repository = repository;
            _mapper = mapper;
            _localDbOptions = localDbOptions;
            _apiClient = apiClient;
            _collectionsApiClient = collectionsApiClient;
            _apiMapper = apiMapper;
            _userClient = userClient;
        }

        public async Task<IList<Flippit.Common.Models.Collection.CollectionListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                var entities = await _repository.GetAllAsync();
                var query = entities.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query = query.Where(c => c.Name.Contains(filter, StringComparison.OrdinalIgnoreCase));
                }

                var paged = query.Skip((page - 1) * pageSize).Take(pageSize);
                return _mapper.DetailToListModels(paged);
            }
            else
            {
                var apiCollections = await _apiClient.CollectionsGetAsync(filter, page, pageSize, sortBy, CancellationToken.None);
                return _apiMapper.ToCommonCollectionLists(apiCollections);
            }
        }

        public async Task<Flippit.Common.Models.Collection.CollectionDetailModel?> GetByIdAsync(Guid id)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                return await _repository.GetByIdAsync(id);
            }
            else
            {
                var apiCollection = await _apiClient.CollectionsGetAsync(id, CancellationToken.None);
                return apiCollection != null ? _apiMapper.ToCommonCollectionDetail(apiCollection) : null;
            }
        }

        public async Task<Guid> CreateOrUpdateAsync(Flippit.Common.Models.Collection.CollectionDetailModel collectionModel)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                if (collectionModel.StartTime == default) collectionModel.StartTime = DateTime.Now;
                if (collectionModel.EndTime == default) collectionModel.EndTime = DateTime.Now;

                var modelToSave = collectionModel;

                if (modelToSave.Id == Guid.Empty)
                {
                    modelToSave = modelToSave with { Id = Guid.NewGuid() };
                }

                await _repository.RemoveAsync(modelToSave.Id);
                await _repository.InsertAsync(modelToSave);
                return modelToSave.Id;
            }
            else
            {
                var apiModel = _apiMapper.ToApiCollectionDetail(collectionModel);
                return await _collectionsApiClient.UpsertAsync(apiModel, CancellationToken.None);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                await _repository.RemoveAsync(id);
            }
            else
            {
                await _apiClient.CollectionsDeleteAsync(id, CancellationToken.None);
            }
        }

        public async Task<IList<Common.Models.Collection.CollectionListModel>> GetAllByUserId(Guid userId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                var entities = await _repository.GetAllAsync();
                var query = entities.AsEnumerable();

                query = query.Where(c => c.CreatorId == userId);

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query = query.Where(c => c.Name.Contains(filter, StringComparison.OrdinalIgnoreCase));
                }

                var paged = query.Skip((page - 1) * pageSize).Take(pageSize);
                return _mapper.DetailToListModels(paged);
            }
            else
            {
                var apiCollections = await _userClient.CollectionsAsync(userId, filter, page, pageSize, sortBy, CancellationToken.None);
                return _apiMapper.ToCommonCollectionLists(apiCollections);
            }
        }
    }
}
