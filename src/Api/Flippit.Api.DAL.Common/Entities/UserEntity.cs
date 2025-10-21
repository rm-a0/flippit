using 
using Flippit.Api.DAL.Common.Entities.Interfaces;

namespace Flippit.Api.DAL.Common.Entities;

public record UserEntity : IEntity
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public string? photoUrl { get; set; }
    public Role Role { get; set; }
}