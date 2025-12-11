using Flippit.Api.BL.Mappers;
using Flippit.Api.BL.Services;
using Flippit.Common.Models.Card;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.BL.Validators;
using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.BL.Facades
{
    public class CardFacade : FacadeBase<ICardRepository, CardEntity>, ICardFacade
    {
        private readonly ICardRepository _repository;
        private readonly CardMapper _mapper;

        public CardFacade(ICardRepository repository, CardMapper mapper) : base(repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IList<CardListModel> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            PaginationValidator.Validate(page, pageSize);
            var entities = _repository.GetAll(filter, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public CardDetailModel? GetById(Guid id)
        {
            var entity = _repository.GetById(id);
            return entity != null ? _mapper.ToDetailModel(entity) : null;
        }

        public IList<CardListModel> Search(string searchText)
        {
            var entities = _repository.Search(searchText);
            return _mapper.ToListModels(entities);
        }

        public IList<CardListModel> SearchByOwnerId(string ownerId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var entities = _repository.SearchByOwnerId(ownerId, filter, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public IList<CardListModel> SearchByCollectionId(Guid collectionId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var entities = _repository.SearchByCollectionId(collectionId, filter, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public Guid CreateOrUpdate(CardDetailModel cardModel, IList<string> userRoles, string? userId)
        {
            ValidateCardModel(cardModel);
            var entity = _mapper.ModelToEntity(cardModel);
            entity = entity with { OwnerId = userId };
            return _repository.Exists(entity.Id) ? _repository.Update(entity) ?? entity.Id : _repository.Insert(entity);
        }

        public Guid Create(CardDetailModel cardModel, IList<string> userRoles, string? userId)
        {
            ValidateCardModel(cardModel);
            var entity = _mapper.ModelToEntity(cardModel);
            entity = entity with { OwnerId = userId };
            return _repository.Insert(entity);
        }

        public Guid? Update(CardDetailModel cardModel, IList<string> userRoles, string? userId)
        {
            ValidateCardModel(cardModel);
            ThrowIfWrongOwnerAndNotAdmin(cardModel.Id, userRoles, userId);
            var entity = _mapper.ModelToEntity(cardModel);
            entity = entity with { OwnerId = userId };
            return _repository.Update(entity);
        }

        public void Delete(Guid id, IList<string> userRoles, string? userId)
        {
            ThrowIfWrongOwnerAndNotAdmin(id, userRoles, userId);
            _repository.Remove(id);
        }

        private static void ValidateCardModel(CardDetailModel cardModel)
        {
            if (cardModel == null)
                throw new ArgumentNullException(nameof(cardModel));

            if (string.IsNullOrWhiteSpace(cardModel.Question))
                throw new ArgumentException("Card question cannot be empty.", nameof(cardModel.Question));
        }
    }
}
