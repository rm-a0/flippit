using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.Common.Repositories
{
    public interface ICollectionRepository : IApiRepository<CollectionEntity>
    {
        IEnumerable<CollectionEntity> SearchByOwnerId(string ownerId, string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
    }
}
