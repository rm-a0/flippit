using Flippit.Api.DAL.Common.Entities;
using Riok.Mapperly.Abstractions;

namespace Flippit.Api.DAL.Common.Mappers
{
    [Mapper]
    public partial class CompletedLessonMapper
    {
        [MapperIgnoreSource(nameof(CompletedLessonEntity.Id))]
        public partial void UpdateEntity(CompletedLessonEntity source, CompletedLessonEntity destination);
    }
}
