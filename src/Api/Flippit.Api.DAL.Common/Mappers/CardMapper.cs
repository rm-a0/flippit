using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flippit.Api.DAL.Common.Entities;
using Riok.Mapperly.Abstractions;

namespace Flippit.Api.DAL.Common.Mappers
{
    [Mapper]
    public partial class CardMapper
    {
        [MapperIgnoreSource(nameof(CardEntity.Id))]
        public partial void UpdateEntity(CardEntity source, CardEntity destination);
    }
}
