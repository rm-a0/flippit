using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.Common.Repositories
{
    public interface ICardRepository : IApiRepository<CardEntity>
    {
        IEnumerable<CardEntity> SearchByCreatorId(Guid creatorId);
        IEnumerable<CardEntity> SearchByCollectionId(Guid collectionId);
    }
}
