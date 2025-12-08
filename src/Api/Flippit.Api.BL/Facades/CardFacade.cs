using Flippit.Api.BL.Mappers;
using Flippit.Api.BL.Services;
using Flippit.Common.Models.Card;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.BL.Validators;

namespace Flippit.Api.BL.Facades
{
    public class CardFacade : ICardFacade
    {
        private readonly ICardRepository _repository;
        private readonly CardMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public CardFacade(ICardRepository repository, CardMapper mapper, ICurrentUserService currentUserService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
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

        public IList<CardListModel> SearchByCreatorId(Guid creatorId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var entities = _repository.SearchByCreatorId(creatorId, filter, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public IList<CardListModel> SearchByCollectionId(Guid collectionId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var entities = _repository.SearchByCollectionId(collectionId, filter, sortBy, page, pageSize);
            return _mapper.ToListModels(entities);
        }

        public Guid CreateOrUpdate(CardDetailModel cardModel)
        {
            ValidateCardModel(cardModel);
            var entity = _mapper.ModelToEntity(cardModel);
            entity = entity with { CreatorId = EnsureAuthenticatedUserId() };
            return _repository.Exists(entity.Id) ? _repository.Update(entity) ?? entity.Id : _repository.Insert(entity);
        }

        public Guid Create(CardDetailModel cardModel)
        {
            ValidateCardModel(cardModel);
            var entity = _mapper.ModelToEntity(cardModel);
            entity = entity with { CreatorId = EnsureAuthenticatedUserId() };
            return _repository.Insert(entity);
        }

        public Guid? Update(CardDetailModel cardModel)
        {
            ValidateCardModel(cardModel);
            var entity = _mapper.ModelToEntity(cardModel);
            entity = entity with { CreatorId = EnsureAuthenticatedUserId() };
            return _repository.Update(entity);
        }

        public void Delete(Guid id)
        {
            _repository.Remove(id);
        }

        private static void ValidateCardModel(CardDetailModel cardModel)
        {
            if (cardModel == null)
                throw new ArgumentNullException(nameof(cardModel));

            if (string.IsNullOrWhiteSpace(cardModel.Question))
                throw new ArgumentException("Card question cannot be empty.", nameof(cardModel.Question));
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
