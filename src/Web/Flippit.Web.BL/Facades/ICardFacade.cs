using Flippit.Common.BL.Facades;
using Flippit.Common.Models.Card;

namespace Flippit.Web.BL.Facades
{
    public interface ICardFacade : IAppFacade
    {
        Task<IList<Flippit.Common.Models.Card.CardListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<IList<Flippit.Common.Models.Card.CardListModel>> SearchByCollectionIdAsync(Guid collectionId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<Flippit.Common.Models.Card.CardDetailModel?> GetByIdAsync(Guid id);
        Task<Guid> CreateOrUpdateAsync(Flippit.Common.Models.Card.CardDetailModel cardModel);
        Task DeleteAsync(Guid id);
    }
}
