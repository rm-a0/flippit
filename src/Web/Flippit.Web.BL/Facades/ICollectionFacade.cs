using Flippit.Common.BL.Facades;
using Flippit.Common.Models.Collection;

namespace Flippit.Web.BL.Facades
{
    public interface ICollectionFacade : IAppFacade
    {
        Task<IList<Flippit.Common.Models.Collection.CollectionListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<Flippit.Common.Models.Collection.CollectionDetailModel?> GetByIdAsync(Guid id);
        Task<Guid> CreateOrUpdateAsync(Flippit.Common.Models.Collection.CollectionDetailModel collectionModel);
        Task DeleteAsync(Guid id);
        Task<IList<Flippit.Common.Models.Collection.CollectionListModel>> GetAllByUserId(Guid userId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
    }
}
