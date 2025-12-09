using Flippit.Common.BL.Facades;
using Flippit.Common.Models.Collection;

namespace Flippit.Web.BL.Facades
{
    public interface ICollectionFacade : IAppFacade
    {
        Task<IList<CollectionListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<CollectionDetailModel?> GetByIdAsync(Guid id);
        Task<Guid> CreateOrUpdateAsync(CollectionDetailModel collectionModel);
        Task DeleteAsync(Guid id);
    }
}
