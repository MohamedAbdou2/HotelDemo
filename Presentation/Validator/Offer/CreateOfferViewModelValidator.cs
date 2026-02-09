using FluentValidation;
using Presentation.ViewModels.Offer;

namespace Presentation.Validator.Offer;

public class CreateOfferViewModelValidator : AbstractValidator<CreateOfferViewModel>
{
    public CreateOfferViewModelValidator()
    {
        //Offer Title Validation
        RuleFor(o => o.Title)
            .NotEmpty().WithMessage("Offer Title Is Required")
            .MaximumLength(150).WithMessage("Max Length Is 150 Character");

        //Offer Description Validation
        RuleFor(o => o.Description)
            .MaximumLength(1000).WithMessage("Max Length Is 1000 Character.");

        //Discount Percentage Validation
        RuleFor(o => o.DiscountPercentage)
            .NotNull().WithMessage("Discount Percentage Is Required.")
            .GreaterThanOrEqualTo(1)
            .WithMessage("Discount Percentage Cannot Be Less Than Or Equal 0.")
            .LessThanOrEqualTo(100.00m)
            .WithMessage("Discount Percentage Cannot Be Greater Than 100.00.");

        //Start Date Validation
        RuleFor(o => o.StartDate)
            .NotNull().WithMessage("Start Date Is Required.")
            .Must(sd => sd >= DateTime.UtcNow)
            .WithMessage("Start Date Must Be In The Future Or Present.");

        //End Date Validation
        RuleFor(o => o.EndDate)
            .NotNull().WithMessage("End Date Is Required")
            .Must((o,ed) => ed > o.StartDate)
            .WithMessage("End date must be greater than or equal to start date.")
            .Must(ed => ed <= DateTime.UtcNow.AddYears(20))
            .WithMessage("End date cannot be more than 20 years in the future.");

        //Room Ids Validation
        RuleFor(o => o.RoomIds)
            .NotNull().WithMessage("Room Ids Is Required")
            .NotEmpty().WithMessage("Room Ids cannot be empty.")
            .Must(i => i.Count() == i.Distinct().Count())
            .WithMessage("Room Ids cannot contain duplicates.");
    }
}
