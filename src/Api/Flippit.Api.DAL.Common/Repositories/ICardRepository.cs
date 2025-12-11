using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.Common.Repositories
{
    public interface ICardRepository : IApiRepository<CardEntity>
    {
        IEnumerable<CardEntity> SearchByOwnerId(string ownerId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        IEnumerable<CardEntity> SearchByCollectionId(Guid collectionId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
    }
}
