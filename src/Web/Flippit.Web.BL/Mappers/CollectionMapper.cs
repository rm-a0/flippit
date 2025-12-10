using System.Collections.Generic;
using Flippit.Common.Models.Collection;
using Riok.Mapperly.Abstractions;

namespace Flippit.Web.BL.Mappers
{
    [Mapper]
    public partial class CollectionMapper
    {
        [MapperIgnoreSource(nameof(CollectionDetailModel.CreatorId))]
        [MapperIgnoreSource(nameof(CollectionDetailModel.StartTime))]
        [MapperIgnoreSource(nameof(CollectionDetailModel.EndTime))]
        public partial CollectionListModel DetailToListModel(CollectionDetailModel detail);

        public partial IList<CollectionListModel> DetailToListModels(IEnumerable<CollectionDetailModel> details);
    }
}
