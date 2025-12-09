using Flippit.Common.Models.Card;
using Flippit.Web.BL.Mappers;
using Flippit.Web.DAL.Repositories;

namespace Flippit.Web.BL.Facades
{
    public class CardFacade : ICardFacade
    {
        private readonly CardRepository _repository;
        private readonly CardMapper _mapper;

        public CardFacade(CardRepository repository, CardMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IList<CardListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
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

        public async Task<IList<CardListModel>> SearchByCollectionIdAsync(Guid collectionId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
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

        public async Task<CardDetailModel?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Guid> CreateOrUpdateAsync(CardDetailModel cardModel)
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

        public async Task DeleteAsync(Guid id)
        {
            await _repository.RemoveAsync(id);
        }
    }
}
