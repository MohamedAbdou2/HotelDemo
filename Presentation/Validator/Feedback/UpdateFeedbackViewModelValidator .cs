using FluentValidation;
using Presentation.ViewModels.Feedback;
namespace Presentation.Validator.Feedback
{

    public class UpdateFeedbackViewModelValidator
        : AbstractValidator<UpdateFeedbackViewModel>
    {
        public UpdateFeedbackViewModelValidator()
        {
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
