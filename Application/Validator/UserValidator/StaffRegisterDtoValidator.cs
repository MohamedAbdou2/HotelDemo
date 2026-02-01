using Application.Dtos.User.Staff;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validator.UserValidator
{
    public class StaffRegisterDtoValidator : AbstractValidator<StaffRegisterDto>
    {
        public StaffRegisterDtoValidator()
        {
            RuleFor(x => x.userName)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");
            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
            RuleFor(x => x.confirmPassword)
                .Equal(x => x.password).WithMessage("Passwords do not match.");
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.phoneNumber)
                .Must(num => num.ToString().Length == 10).WithMessage("Phone number must be 10 digits long.");
  
            RuleFor(x => x.HireDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Hire date cannot be in the future.");
            
        }

    }
}
