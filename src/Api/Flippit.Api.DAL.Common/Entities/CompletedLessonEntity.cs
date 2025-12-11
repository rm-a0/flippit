namespace Flippit.Api.DAL.Common.Entities
{
    public record CompletedLessonEntity : EntityBase
    {
        public required string AnswersJson { get; set; }
        public required string StatisticsJson { get; set; }
        public required Guid UserId { get; set; }
        public required Guid CollectionId { get; set; }
    }
}
