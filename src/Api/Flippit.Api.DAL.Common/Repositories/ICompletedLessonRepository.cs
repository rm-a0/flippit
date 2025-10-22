using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.Common.Repositories
{
    public interface ICompletedLessonRepository : IApiRepository<CompletedLessonEntity>
    {
        IEnumerable<CompletedLessonEntity> SearchByCreatorId(Guid creatorId);
        IEnumerable<CompletedLessonEntity> SearchByCollectionId(Guid collectionId);
    }
}
