using Flippit.Common.Models.Card;
using Flippit.Web.BL.Mappers;
using Flippit.Web.DAL.Repositories;

namespace Flippit.Web.BL.Facades
{
    public class CardFacade : ICardFacade
    {
        private readonly CardRepository _repository;
        private readonly CardMapper _mapper;
        private readonly LocalDbOptions _localDbOptions;
        private readonly IApiApiClient _apiClient;
        private readonly ICardsApiClient _cardsApiClient;
        private readonly ApiModelMapper _apiMapper;

        public CardFacade(
            CardRepository repository, 
            CardMapper mapper,
            LocalDbOptions localDbOptions,
            IApiApiClient apiClient,
            ICardsApiClient cardsApiClient,
            ApiModelMapper apiMapper)
        {
            _repository = repository;
            _mapper = mapper;
            _localDbOptions = localDbOptions;
            _apiClient = apiClient;
            _cardsApiClient = cardsApiClient;
            _apiMapper = apiMapper;
        }

        public async Task<IList<Flippit.Common.Models.Card.CardListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                var entities = await _repository.GetAllAsync();
                var query = entities.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query = query.Where(c => c.Question.Contains(filter, StringComparison.OrdinalIgnoreCase) 
                                          || c.Answer.Contains(filter, StringComparison.OrdinalIgnoreCase));
                }

                var paged = query.Skip((page - 1) * pageSize).Take(pageSize);
                return _mapper.DetailToListModels(paged);
            }
            else
            {
                var apiCards = await _apiClient.CardsGetAsync(filter, page, pageSize, sortBy, CancellationToken.None);
                return _apiMapper.ToCommonCardLists(apiCards);
            }
        }

        public async Task<IList<Flippit.Common.Models.Card.CardListModel>> SearchByCollectionIdAsync(Guid collectionId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                var entities = await _repository.GetAllAsync();
                
                var query = entities.Where(c => c.CollectionId == collectionId);

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query = query.Where(c => c.Question.Contains(filter, StringComparison.OrdinalIgnoreCase));
                }

                var paged = query.Skip((page - 1) * pageSize).Take(pageSize);
                return _mapper.DetailToListModels(paged);
            }
            else
            {
                // API doesn't have a specific endpoint for searching by collection ID
                // Fetch with large page size and filter client-side
                // This is not ideal but matches the API capabilities
                var apiCards = await _apiClient.CardsGetAsync(filter, 1, 1000, sortBy, CancellationToken.None);
                var filtered = apiCards.Where(c => c.CollectionId == collectionId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);
                return _apiMapper.ToCommonCardLists(filtered);
            }
        }

        public async Task<Flippit.Common.Models.Card.CardDetailModel?> GetByIdAsync(Guid id)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                return await _repository.GetByIdAsync(id);
            }
            else
            {
                var apiCard = await _apiClient.CardsGetAsync(id, CancellationToken.None);
                return apiCard != null ? _apiMapper.ToCommonCardDetail(apiCard) : null;
            }
        }

        public async Task<Guid> CreateOrUpdateAsync(Flippit.Common.Models.Card.CardDetailModel cardModel)
        {
            if (_localDbOptions.IsLocalDbEnabled)
            {
                var modelToSave = cardModel;

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
                var apiModel = _apiMapper.ToApiCardDetail(cardModel);
                return await _cardsApiClient.UpsertAsync(apiModel, CancellationToken.None);
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
                await _apiClient.CardsDeleteAsync(id, CancellationToken.None);
            }
        }
    }
}
