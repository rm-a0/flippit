using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flippit.Common.Models.Collection
{
    public record CollectionListModel : IWithId
    {
        public Guid Id { get; init; }
        public required string Name { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
    }
}
