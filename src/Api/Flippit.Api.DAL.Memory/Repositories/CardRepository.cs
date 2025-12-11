using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Mappers;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.DAL.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flippit.Api.DAL.Memory.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly IList<CardEntity> _cards;
        private readonly CardMapper _cardMapper;

        public CardRepository(Storage storage, CardMapper cardMapper)
        {
            _cards = storage.Cards;
            _cardMapper = cardMapper;
        }

        public IList<CardEntity> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            return _cards.AsQueryable()
                         .ApplyFilterSortAndPage(filter, sortBy, page, pageSize)
                         .ToList();
        }

        public CardEntity? GetById(Guid id)
        {
            return _cards.SingleOrDefault(card => card.Id == id);
        }

        public IEnumerable<CardEntity> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _cards;

            return _cards.Where(c => c.Question.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                    (c.Description != null && c.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
        }

        public IEnumerable<CardEntity> SearchByOwnerId(string ownerId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            return _cards
                .Where(c => c.OwnerId == ownerId)
                .AsQueryable()
                .ApplyFilterSortAndPage(filter, sortBy, page, pageSize)
                .ToList();
        }

        public IEnumerable<CardEntity> SearchByCollectionId(Guid collectionId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            return _cards
                .Where(c => c.CollectionId == collectionId)
                .AsQueryable()
                .ApplyFilterSortAndPage(filter, sortBy, page, pageSize)
                .ToList();
        }

        public Guid Insert(CardEntity entity)
        {
            _cards.Add(entity);
            return entity.Id;
        }

        public Guid? Update(CardEntity cardUpdated)
        {
            var cardExisting = _cards.SingleOrDefault(card => card.Id == cardUpdated.Id);
            if (cardExisting == null)
                return null;

            _cardMapper.UpdateEntity(cardUpdated, cardExisting);
            return cardExisting.Id;
        }

        public void Remove(Guid id)
        {
            var cardToRemove = _cards.SingleOrDefault(card => card.Id == id);
            if (cardToRemove != null)
            {
                _cards.Remove(cardToRemove);
            }
        }

        public bool Exists(Guid id)
        {
            return _cards.Any(card => card.Id == id);
        }
    }
}
