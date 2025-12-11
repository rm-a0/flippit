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
        [MapperIgnoreTarget(nameof(CollectionEntity.OwnerId))]
        public partial CollectionEntity ModelToEntity(CollectionDetailModel model);
        
        [MapperIgnoreSource(nameof(CollectionEntity.OwnerId))]
        public partial CollectionDetailModel ToDetailModel(CollectionEntity entity);
        
        public partial IList<CollectionListModel> ToListModels(IEnumerable<CollectionEntity> entities);
        public partial CollectionListModel ToListModel(CollectionEntity entity);
    }
}
