using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Common.Models.Card;
using Flippit.Common.Models.Collection;
using Riok.Mapperly.Abstractions;

namespace Flippit.Api.BL.Mappers
{
    [Mapper]
    public partial class CollectionMapper
    {
        public partial CollectionEntity ModelToEntity(CollectionDetailModel model);
        public partial CollectionDetailModel ToDetailModel(CollectionEntity entity);
        public partial IList<CollectionListModel> ToListModels(IEnumerable<CollectionEntity> entities);

        [MapperIgnoreSource(nameof(CollectionEntity.CreatorId))]
        [MapperIgnoreSource(nameof(CollectionEntity.StartTime))]
        [MapperIgnoreSource(nameof(CollectionEntity.EndTime))]
        public partial CollectionListModel ToListModel(CollectionEntity entity);
    }
}
