using System;
using System.Collections.Generic;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Memory.Repositories;
using Flippit.Api.BL.Mappers;
using Flippit.Common.Models.Card;
using Flippit.Api.DAL.Common.Repositories;

namespace Flippit.Api.BL.Facades
{
    public class CardFacade : ICardFacade
    {
        private readonly ICardRepository _repository;
        private readonly CardMapper _mapper;

        public CardFacade(ICardRepository repository, CardMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IList<CardListModel> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
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

        public IList<CardListModel> SearchByCreatorId(Guid creatorId)
        {
            var entities = _repository.SearchByCreatorId(creatorId);
            return _mapper.ToListModels(entities);
        }

        public IList<CardListModel> SearchByCollectionId(Guid collectionId)
        {
            var entities = _repository.SearchByCollectionId(collectionId);
            return _mapper.ToListModels(entities);
        }

        public Guid CreateOrUpdate(CardDetailModel cardModel)
        {
            if (cardModel == null)
                throw new ArgumentNullException(nameof(cardModel));

            var entity = _mapper.ModelToEntity(cardModel);
            return _repository.Exists(entity.Id) ? _repository.Update(entity) ?? entity.Id : _repository.Insert(entity);
        }

        public Guid Create(CardDetailModel cardModel)
        {
            if (cardModel == null)
                throw new ArgumentNullException(nameof(cardModel));
            if (string.IsNullOrWhiteSpace(cardModel.Question))
                throw new ArgumentException("Card question cannot be empty.", nameof(cardModel.Question));

            var entity = _mapper.ModelToEntity(cardModel);
            return _repository.Insert(entity);
        }

        public Guid? Update(CardDetailModel cardModel)
        {
            if (cardModel == null)
                throw new ArgumentNullException(nameof(cardModel));
            if (string.IsNullOrWhiteSpace(cardModel.Question))
                throw new ArgumentException("Card question cannot be empty.", nameof(cardModel.Question));

            var entity = _mapper.ModelToEntity(cardModel);
            return _repository.Update(entity);
        }

        public void Delete(Guid id)
        {
            _repository.Remove(id);
        }
    }
}
