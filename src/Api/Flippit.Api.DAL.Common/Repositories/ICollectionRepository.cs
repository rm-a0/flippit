using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.Common.Repositories
{
    public interface ICollectionRepository : IApiRepository<CollectionEntity>
    {
        IEnumerable<CollectionEntity> SearchByCreatorId(Guid creatorId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
    }
}
