using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.User;
using FluentValidation;

namespace Application.Validator.UserValidator
{
    public class UpdateUserValidator:AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FirstName)
           .MaximumLength(100).WithMessage("First name must not exceed 50 characters");

            RuleFor(x => x.LastName)
                .MaximumLength(100).WithMessage("Last name must not exceed 50 characters");

            RuleFor(x => x.Username)
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\d{10,15}$")
                .WithMessage("Phone number must be between 10 and 15 digits.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email must be valid.");

        }
    }
}
