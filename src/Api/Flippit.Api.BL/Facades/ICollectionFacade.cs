using System;
using System.Collections.Generic;
using Flippit.Common.BL.Facades;
using Flippit.Common.Models.Collection;

namespace Flippit.Api.BL.Facades
{
    public interface ICollectionFacade : IAppFacade
    {
        IList<CollectionListModel> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        CollectionDetailModel? GetById(Guid id);
        IList<CollectionListModel> Search(string searchText);
        IList<CollectionListModel> SearchByCreatorId(Guid creatorId);
        Guid CreateOrUpdate(CollectionDetailModel collectionModel);
        Guid Create(CollectionDetailModel collectionModel);
        Guid? Update(CollectionDetailModel collectionModel);
        void Delete(Guid id);
    }
}
