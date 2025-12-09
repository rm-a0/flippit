using System.Collections.Generic;
using Flippit.Common.Models.CompletedLesson;
using Riok.Mapperly.Abstractions;

namespace Flippit.Web.BL.Mappers
{
    [Mapper]
    public partial class CompletedLessonMapper
    {
        [MapperIgnoreSource(nameof(CompletedLessonDetailModel.UserId))]
        public partial CompletedLessonListModel DetailToListModel(CompletedLessonDetailModel detail);

        public partial IList<CompletedLessonListModel> DetailToListModels(IEnumerable<CompletedLessonDetailModel> details);
    }
}
