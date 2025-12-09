using Flippit.Common.BL.Facades;
using Flippit.Common.Models.CompletedLesson;

namespace Flippit.Web.BL.Facades
{
    public interface ICompletedLessonFacade : IAppFacade
    {
        Task<IList<CompletedLessonListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<IList<CompletedLessonListModel>> SearchByCollectionIdAsync(Guid collectionId);
        Task<CompletedLessonDetailModel?> GetByIdAsync(Guid id);
        Task<Guid> CreateOrUpdateAsync(CompletedLessonDetailModel lessonModel);
        Task DeleteAsync(Guid id);
    }
}
