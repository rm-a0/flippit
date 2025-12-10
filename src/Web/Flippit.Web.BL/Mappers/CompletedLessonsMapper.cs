using System.Collections.Generic;
using Flippit.Common.Models.CompletedLesson;
using Riok.Mapperly.Abstractions;

namespace Flippit.Web.BL.Mappers
{
    [Mapper]
    public partial class CompletedLessonMapper
    {
        [MapperIgnoreSource(nameof(CompletedLessonDetailModel.UserId))]
        public partial Flippit.Common.Models.CompletedLesson.CompletedLessonListModel DetailToListModel(Flippit.Common.Models.CompletedLesson.CompletedLessonDetailModel detail);

        public partial IList<Flippit.Common.Models.CompletedLesson.CompletedLessonListModel> DetailToListModels(IEnumerable<Flippit.Common.Models.CompletedLesson.CompletedLessonDetailModel> details);
    }
}
