using Application.Dtos.Reservation;
using Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validator.Reservation
{
    public class ReservationDtoValidator : AbstractValidator<ReservationDto>
    {
        public ReservationDtoValidator()
        {

            RuleFor(x => x.RoomId)
                .NotEmpty().WithMessage("RoomId is required.")
               .NotEqual(Guid.Empty).WithMessage("RoomId cannot be an empty GUID.");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required.")
                .NotEqual(Guid.Empty).WithMessage("CustomerId cannot be an empty GUID.");

            RuleFor(x => x.CheckInDate)
                    .NotEmpty().WithMessage("CheckInDate is required.")
                .GreaterThan(DateTime.Now).WithMessage("CheckInDate must be in the future.");

            RuleFor(x => x.CheckOutDate)
                .NotEmpty().WithMessage("CheckOutDate is required.")
                .GreaterThan(x => x.CheckInDate).WithMessage("CheckOutDate must be after CheckInDate.");

         
            RuleFor(x => x.ReservationStatusId)
                .IsInEnum().WithMessage("ReservationStatusId must be a valid enum value.")
                .Must(x => x >= ReservationStatusCode.Pending && x <= ReservationStatusCode.Cancelled);

            RuleFor(x => x.PaymentId)
                .NotEqual(Guid.Empty).When(x => x.PaymentId.HasValue).WithMessage("PaymentId cannot be an empty GUID if provided.");


        }


    }
}
