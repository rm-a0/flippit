using Flippit.Common.BL.Facades;
using Flippit.Common.Models.Card;

namespace Flippit.Web.BL.Facades
{
    public interface ICardFacade : IAppFacade
    {
        Task<IList<CardListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<IList<CardListModel>> SearchByCollectionIdAsync(Guid collectionId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<CardDetailModel?> GetByIdAsync(Guid id);
        Task<Guid> CreateOrUpdateAsync(CardDetailModel cardModel);
        Task DeleteAsync(Guid id);
    }
}
