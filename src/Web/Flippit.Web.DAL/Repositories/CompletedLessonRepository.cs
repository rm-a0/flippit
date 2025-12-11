using Flippit.Common.Models.CompletedLesson;

namespace Flippit.Web.DAL.Repositories
{
    public class CompletedLessonRepository : RepositoryBase<CompletedLessonDetailModel>
    {
        public override string TableName { get; } = "completedLessons";

        public CompletedLessonRepository(LocalDb localDb)
            : base(localDb)
        {
        }
    }
}
