using Flippit.Api.DAL.Common.Entities.Interfaces;
using Flippit.Common.Enums;

namespace Flippit.Api.DAL.Common.Entities;

public record UserEntity : IEntity
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public string? PhotoUrl { get; set; }
    public Role Role { get; set; }
}
