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
        IList<CardListModel> SearchByOwnerId(string ownerId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        IList<CardListModel> SearchByCollectionId(Guid collectionId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Guid CreateOrUpdate(CardDetailModel cardModel, IList<string> userRoles, string? userId);
        Guid Create(CardDetailModel cardModel, IList<string> userRoles, string? userId);
        Guid? Update(CardDetailModel cardModel, IList<string> userRoles, string? userId);
        void Delete(Guid id, IList<string> userRoles, string? userId);
    }
}
