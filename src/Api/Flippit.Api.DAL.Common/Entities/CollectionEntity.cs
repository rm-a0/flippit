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
