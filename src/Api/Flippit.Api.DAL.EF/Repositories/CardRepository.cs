using Flippit.Api.DAL.Common.Entities;
using Flippit.Api.DAL.Common.Mappers;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.DAL.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flippit.Api.DAL.EF.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly FlippitDbContext _dbContext;
        private readonly CardMapper _cardMapper;

        public CardRepository(FlippitDbContext dbContext, CardMapper cardMapper)
        {
            _dbContext = dbContext;
            _cardMapper = cardMapper;
        }

        public IList<CardEntity> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            return _dbContext.Cards
                .AsQueryable()
                .ApplyFilterSortAndPage(filter, sortBy, page, pageSize)
                .ToList();
        }

        public CardEntity? GetById(Guid id)
        {
            return _dbContext.Cards.SingleOrDefault(card => card.Id == id);
        }

        public IEnumerable<CardEntity> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _dbContext.Cards.ToList();

            return _dbContext.Cards
                .Where(c => c.Question.Contains(searchText) || 
                            (c.Description != null && c.Description.Contains(searchText)))
                .ToList();
        }

        public IEnumerable<CardEntity> SearchByCreatorId(Guid creatorId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var query = _dbContext.Cards.Where(c => c.CreatorId == creatorId)
                .ApplyFilterSortAndPage(filter, sortBy, page, pageSize);

            return query.ToList();
        }

        public IEnumerable<CardEntity> SearchByCollectionId(Guid collectionId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var query = _dbContext.Cards.Where(c => c.CollectionId == collectionId)
                .ApplyFilterSortAndPage(filter, sortBy, page, pageSize);

            return query.ToList();
        }

        public Guid Insert(CardEntity entity)
        {
            _dbContext.Cards.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public Guid? Update(CardEntity cardUpdated)
        {
            var cardExisting = _dbContext.Cards.SingleOrDefault(card => card.Id == cardUpdated.Id);
            if (cardExisting == null)
                return null;

            _cardMapper.UpdateEntity(cardUpdated, cardExisting);
            _dbContext.Cards.Update(cardExisting);
            _dbContext.SaveChanges();
            return cardExisting.Id;
        }

        public void Remove(Guid id)
        {
            var cardToRemove = _dbContext.Cards.SingleOrDefault(card => card.Id == id);
            if (cardToRemove != null)
            {
                _dbContext.Cards.Remove(cardToRemove);
                _dbContext.SaveChanges();
            }
        }

        public bool Exists(Guid id)
        {
            return _dbContext.Cards.Any(card => card.Id == id);
        }
    }
}
