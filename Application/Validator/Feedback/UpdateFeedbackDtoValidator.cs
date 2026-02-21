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
            .Must(value => new float[] { 1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5 }.Contains(value.Value))
            .WithMessage("Rating must be one of the following: 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5");

            RuleFor(x => x.Comments)
                .MaximumLength(500)
                .WithMessage("Comments cannot exceed 500 characters");
        }
    }
}
