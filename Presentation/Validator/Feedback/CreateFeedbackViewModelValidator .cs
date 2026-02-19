using FluentValidation;
using Presentation.ViewModels.Feedback;
namespace Presentation.Validator.Feedback
{

    public class CreateFeedbackViewModelValidator
        : AbstractValidator<CreateFeedbackViewModel>
    {
        public CreateFeedbackViewModelValidator()
        {
            RuleFor(x => x.ReservationId)
                .NotEmpty()
                .WithMessage("ReservationId is required.");

            RuleFor(x => x.Rating)
                 .Must(value => new float[] { 1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5 }.Contains(value))
            .WithMessage("Rating must be one of the following: 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5");

            RuleFor(x => x.Comments)
                .NotEmpty()
                .WithMessage("Comments are required.")
                .MaximumLength(500)
                .WithMessage("Comments cannot exceed 500 characters.");
        }
    }
}
