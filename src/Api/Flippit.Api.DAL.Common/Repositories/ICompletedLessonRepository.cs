using Flippit.Api.DAL.Common.Entities;

namespace Flippit.Api.DAL.Common.Repositories
{
    public interface ICompletedLessonRepository : IApiRepository<CompletedLessonEntity>
    {
        IEnumerable<CompletedLessonEntity> SearchByCreatorId(Guid creatorId, string? sortBy = null, int page = 1, int pageSize = 10);
        IEnumerable<CompletedLessonEntity> SearchByCollectionId(Guid collectionId, string? sortBy = null, int page = 1, int pageSize = 10);
    }
}
