using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flippit.Common.Enums;
using FluentValidation;

namespace Flippit.Common.Models.User
{
    public record UserDetailModel : IWithId
    {
        public Guid Id { get; init; }
        public required string Name { get; set; }
        public string? PhotoUrl { get; set; }
        public Role Role { get; set; }

    }

    public class UserDetailModelValidator : AbstractValidator<UserDetailModel>
    {
        public UserDetailModelValidator() 
        { 
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name field is required")
                .MinimumLength(3)
                .WithMessage("Name must be at least 3 characters")
                .MaximumLength(50)
                .WithMessage("Name cannot exceed 50 characters");

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role field is required")
                .IsInEnum()
                .WithMessage("Role must be a valid value");
        }
    }
}
