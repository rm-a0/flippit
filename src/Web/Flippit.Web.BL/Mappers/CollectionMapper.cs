using System.Collections.Generic;
using Flippit.Common.Models.Collection;
using Riok.Mapperly.Abstractions;

namespace Flippit.Web.BL.Mappers
{
    [Mapper]
    public partial class CollectionMapper
    {
        public partial Flippit.Common.Models.Collection.CollectionListModel DetailToListModel(Flippit.Common.Models.Collection.CollectionDetailModel detail);

        public partial IList<Flippit.Common.Models.Collection.CollectionListModel> DetailToListModels(IEnumerable<Flippit.Common.Models.Collection.CollectionDetailModel> details);
    }
}
