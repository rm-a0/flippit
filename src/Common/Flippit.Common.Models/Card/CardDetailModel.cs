using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flippit.Common.Enums;
using FluentValidation;

namespace Flippit.Common.Models.Card
{
    public record CardDetailModel : IWithId
    {
        public Guid Id { get; init; }
        public required QAType QuestionType { get; set; }
        public required QAType AnswerType { get; set; }
        public required string Question { get; set; }
        public required string Answer { get; set; }
        public string? Description { get; set; }
        public required Guid CreatorId { get; set; }
        public required Guid CollectionId { get; set; }
    }

    public class CardDetailModelValidator : AbstractValidator<CardDetailModel>
    {
        public CardDetailModelValidator() 
        {
            RuleFor(x => x.QuestionType)
                .IsInEnum()
                .WithMessage("QuestionType must be a valid value");

            RuleFor(x => x.AnswerType)
                .IsInEnum()
                .WithMessage("AnswerType must be a valid value");

            RuleFor(x => x.Question)
                .NotEmpty()
                .WithMessage("Question field must not be empty");

            RuleFor(x => x.Answer)
                .NotEmpty()
                .WithMessage("Answer field must not be empty");

            RuleFor(x => x.CreatorId)
                .NotEmpty();

            RuleFor(x => x.CollectionId)
                .NotEmpty();
        }
    }
}
