using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Feedback;
using FluentValidation;

namespace Application.Validator.Feedback
{
    public class CreateFeedbackDtoValidator:AbstractValidator<CreateFeedbackDto>
    {
        public CreateFeedbackDtoValidator()
        {
            RuleFor(x => x.ReservationId)
                .NotEmpty().WithMessage("ReservationId is required");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.Comments)
                .NotEmpty().WithMessage("Comments are required")
                .MaximumLength(500)
                .WithMessage("Comments cannot exceed 500 characters");
        }
    }
}
