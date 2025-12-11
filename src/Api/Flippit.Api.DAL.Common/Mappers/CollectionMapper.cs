using Flippit.Api.DAL.Common.Entities;
using Riok.Mapperly.Abstractions;

namespace Flippit.Api.DAL.Common.Mappers
{
    [Mapper]
    public partial class CollectionMapper
    {
        [MapperIgnoreSource(nameof(CollectionEntity.Id))]
        public partial void UpdateEntity(CollectionEntity source, CollectionEntity destination);
    }
}
