using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Common.Models.Card;
using Flippit.Common.Models.CompletedLesson;
using Riok.Mapperly.Abstractions;

namespace Flippit.Api.BL.Mappers
{
    [Mapper]
    public partial class CompletedLessonMapper
    {
        [MapperIgnoreTarget(nameof(CompletedLessonEntity.OwnerId))]
        public partial CompletedLessonEntity ModelToEntity(CompletedLessonDetailModel model);
        
        [MapperIgnoreSource(nameof(CompletedLessonEntity.OwnerId))]
        public partial CompletedLessonDetailModel ToDetailModel(CompletedLessonEntity entity);
        
        public partial IList<CompletedLessonListModel> ToListModels(IEnumerable<CompletedLessonEntity> entities);

        [MapperIgnoreSource(nameof(CompletedLessonEntity.UserId))]
        [MapperIgnoreSource(nameof(CompletedLessonEntity.OwnerId))]
        public partial CompletedLessonListModel ToListModel(CompletedLessonEntity entity);
    }
}
