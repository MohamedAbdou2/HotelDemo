using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Room.RoomValidators
{
    public class RoomFilterRequestDtoValidator : AbstractValidator<RoomFilterRequestDto>
    {
        public RoomFilterRequestDtoValidator()
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
