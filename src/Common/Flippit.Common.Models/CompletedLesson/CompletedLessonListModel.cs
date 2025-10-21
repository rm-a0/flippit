using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flippit.Common.Models.CompletedLesson
{
    public record CompletedLessonListModel : IWithId
    {
        public Guid Id { get; init; }
        public required string AnswersJson { get; set; }
        public required string StatisticsJson { get; set; }
        public required Guid CollectionId { get; set; }

    }
}
