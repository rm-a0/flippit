using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flippit.Common.Models.Card;
using FluentValidation;

namespace Flippit.Common.Models.Collection
{
    public record CollectionDetailModel : IWithId
    {
        public Guid Id { get; init; }
        public required string Name { get; set; }
        public required Guid CreatorId { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
    }

    public class CollectionDetailModelValidator : AbstractValidator<CollectionDetailModel>
    {
        public CollectionDetailModelValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name field is required");

        }
    }
}
