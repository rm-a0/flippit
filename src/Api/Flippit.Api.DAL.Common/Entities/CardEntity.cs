using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flippit.Common.Enums;

namespace Flippit.Api.DAL.Common.Entities
{
    public record CardEntity : EntityBase
    {
        public required QAType QuestionType { get; set; }
        public required QAType AnswerType { get; set; }
        public required string Question {  get; set; }
        public required string Answer { get; set; }
        public string? Description { get; set; }
        public required Guid CollectionId { get; set; }
    }
}
