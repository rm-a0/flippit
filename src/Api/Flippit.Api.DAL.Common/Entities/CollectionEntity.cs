using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flippit.Api.DAL.Common.Entities
{
    public record CollectionEntity : EntityBase
    {
        public required string Name { get; set; }
        public required Guid CreatorId { get; set; } 
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
    }
}
