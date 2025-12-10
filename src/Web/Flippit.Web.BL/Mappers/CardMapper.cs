using System.Collections.Generic;
using Flippit.Common.Models.Card;
using Riok.Mapperly.Abstractions;

namespace Flippit.Web.BL.Mappers
{
    [Mapper]
    public partial class CardMapper
    {
        public partial CardListModel DetailToListModel(CardDetailModel detail);
        public partial IList<CardListModel> DetailToListModels(IEnumerable<CardDetailModel> details);
    }
}
