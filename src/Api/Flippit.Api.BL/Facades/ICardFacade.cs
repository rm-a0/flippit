using System;
using System.Collections.Generic;
using Flippit.Common.BL.Facades;
using Flippit.Common.Models.Card;

namespace Flippit.Api.BL.Facades
{
    public interface ICardFacade : IAppFacade
    {
        IList<CardListModel> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        CardDetailModel? GetById(Guid id);
        IList<CardListModel> Search(string searchText);
        IList<CardListModel> SearchByCreatorId(Guid creatorId);
        IList<CardListModel> SearchByCollectionId(Guid collectionId);
        Guid CreateOrUpdate(CardDetailModel cardModel);
        Guid Create(CardDetailModel cardModel);
        Guid? Update(CardDetailModel cardModel);
        void Delete(Guid id);
    }
}
