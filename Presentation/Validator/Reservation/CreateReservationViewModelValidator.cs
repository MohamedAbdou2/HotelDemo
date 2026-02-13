using FluentValidation;
using Presentation.ViewModels.Reservation;

namespace Presentation.Validator.Reservation
{
    public class CreateReservationViewModelValidator :AbstractValidator<CreateReservationViewModel>
    {
        public CreateReservationViewModelValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty().WithMessage("RoomId is required.");

            RuleFor(x => x.CheckInDate)
                .NotEmpty().WithMessage("CheckInDate is required.")
                .GreaterThan(DateTime.Now).WithMessage("CheckInDate must be in the future.");

            RuleFor(x => x.CheckOutDate)
                .NotEmpty().WithMessage("CheckOutDate is required.")
                .GreaterThan(x => x.CheckInDate).WithMessage("CheckOutDate must be after CheckInDate.");

            RuleFor(x => x.TotalPrice)
                .NotEmpty()
                .GreaterThan(0).WithMessage("TotalPrice must be greater than zero.");   

        }
    }
}
