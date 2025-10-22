using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Common.Models.Card;
using Riok.Mapperly.Abstractions;

namespace Flippit.Api.BL.Mappers
{
    [Mapper]
    public partial class CardMapper
    {
        public partial CardEntity ModelToEntity(CardDetailModel model);
        public partial CardDetailModel ToDetailModel(CardEntity entity);
        public partial IList<CardListModel> ToListModels(IEnumerable<CardEntity> entities);
        public partial CardListModel ToListModel(CardEntity entity);
    }
}
