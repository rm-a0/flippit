using System.Collections.Generic;
using Flippit.Common.Models.Card;
using Riok.Mapperly.Abstractions;

namespace Flippit.Web.BL.Mappers
{
    [Mapper]
    public partial class CardMapper
    {
        public partial Flippit.Common.Models.Card.CardListModel DetailToListModel(Flippit.Common.Models.Card.CardDetailModel detail);
        public partial IList<Flippit.Common.Models.Card.CardListModel> DetailToListModels(IEnumerable<Flippit.Common.Models.Card.CardDetailModel> details);
    }
}
