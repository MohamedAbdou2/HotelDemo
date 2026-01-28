using FluentValidation;

namespace Presentation.ViewModels.Room.RoomValidators
{
    public class RoomFilterRequestViewModelValidator : AbstractValidator<RoomFilterRequestViewModel>
    {
        public RoomFilterRequestViewModelValidator()
        {
            RuleFor(x => x.MinPrice)
                   .GreaterThanOrEqualTo(0).WithMessage("MinPrice must be greater than or equal to 0.")
                   .LessThanOrEqualTo(x => x.MaxPrice).WithMessage("MinPrice must be less than or equal to MaxPrice.");
            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(x => x.MinPrice).WithMessage("MaxPrice must be greater than or equal to MinPrice.");
            RuleFor(x => x.RoomTypeId)
                .IsInEnum().WithMessage("Invalid RoomTypeId.");

        }
    }
}
