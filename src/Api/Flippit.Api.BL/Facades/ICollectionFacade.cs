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
        IList<CollectionListModel> SearchByOwnerId(string ownerId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Guid CreateOrUpdate(CollectionDetailModel collectionModel, IList<string> userRoles, string? userId);
        Guid Create(CollectionDetailModel collectionModel, IList<string> userRoles, string? userId);
        Guid? Update(CollectionDetailModel collectionModel, IList<string> userRoles, string? userId);
        void Delete(Guid id, IList<string> userRoles, string? userId);
    }
}
