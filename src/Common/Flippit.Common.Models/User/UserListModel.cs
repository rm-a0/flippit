using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flippit.Common.Enums;

namespace Flippit.Common.Models.User
{
    public record UserListModel : IWithId
    {
        public Guid Id { get; init; }
        public required string Name { get; set; }
        public string? photoUrl { get; set; }
        public Role Role { get; set; }

    }
}
