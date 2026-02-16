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
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Comments)
                .NotEmpty()
                .WithMessage("Comments are required.")
                .MaximumLength(500)
                .WithMessage("Comments cannot exceed 500 characters.");
        }
    }
}
