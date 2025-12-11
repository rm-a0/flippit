using Flippit.Common.BL.Facades;
using Flippit.Common.Models.CompletedLesson;

namespace Flippit.Web.BL.Facades
{
    public interface ICompletedLessonFacade : IAppFacade
    {
        Task<IList<Flippit.Common.Models.CompletedLesson.CompletedLessonListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<IList<Flippit.Common.Models.CompletedLesson.CompletedLessonListModel>> SearchByCollectionIdAsync(Guid collectionId);
        Task<Flippit.Common.Models.CompletedLesson.CompletedLessonDetailModel?> GetByIdAsync(Guid id);
        Task<Guid> CreateOrUpdateAsync(Flippit.Common.Models.CompletedLesson.CompletedLessonDetailModel lessonModel);
        Task DeleteAsync(Guid id);
        Task<IList<Flippit.Common.Models.CompletedLesson.CompletedLessonListModel>> GetAllByUserid(Guid userId, int page = 1, int pageSize = 10, string? sortBy = null);
    }
}
