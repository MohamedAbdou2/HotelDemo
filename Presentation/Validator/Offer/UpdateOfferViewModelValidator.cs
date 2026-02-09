using FluentValidation;
using Presentation.ViewModels.Offer;

namespace Presentation.Validator.Offer
{
    public class UpdateOfferViewModelValidator : AbstractValidator<UpdateOfferViewModel>
    {
        public UpdateOfferViewModelValidator()
        {
            //Offer Title Validation
            RuleFor(o => o.Title)
                .MaximumLength(150).WithMessage("Max Length Is 150 Character");

            //Offer Description Validation
            RuleFor(o => o.Description)
                .MaximumLength(1000).WithMessage("Max Length Is 1000 Character.");

            //Discount Percentage Validation
            RuleFor(o => o.DiscountPercentage)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Discount Percentage Cannot Be Less Than Or Equal 0.")
                .LessThanOrEqualTo(100.00m)
                .WithMessage("Discount Percentage Cannot Be Greater Than 100.00.");

            //Start Date Validation
            RuleFor(o => o.StartDate)
                .Must(sd => sd is null || sd >= DateTime.UtcNow)
                .WithMessage("Start Date Must Be In The Future Or Present.");

            //End Date Validation
            RuleFor(o => o.EndDate)
                .Must((o, ed) => ed is null || ed > o.StartDate)
                .WithMessage("End date must be greater than or equal to start date.")
                .Must(ed => ed is null || ed <= DateTime.UtcNow.AddYears(20))
                .WithMessage("End date cannot be more than 20 years in the future.");


        }
    }
}
