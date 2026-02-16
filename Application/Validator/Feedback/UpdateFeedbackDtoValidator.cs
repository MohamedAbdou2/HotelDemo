using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Feedback;
using FluentValidation;

namespace Application.Validator.Feedback
{
    public class UpdateFeedbackDtoValidator: AbstractValidator<UpdateFeedbackDto>
    {
        public UpdateFeedbackDtoValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.Comments)
                .MaximumLength(500)
                .WithMessage("Comments cannot exceed 500 characters");
        }
    }
}
