using Flippit.Common;

namespace Flippit.Api.DAL.Common.Entities.Interfaces
{
    public interface IEntity : IWithId
    {
        string? OwnerId { get; set; }
    }
}
