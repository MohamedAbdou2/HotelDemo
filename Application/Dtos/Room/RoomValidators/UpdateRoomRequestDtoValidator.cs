using Domain.Enums;
using FluentValidation;

namespace Application.Dtos.Room.RoomValidators
{
    public class UpdateRoomRequestDtoValidator : AbstractValidator<UpdateRoomRequestDto>
    {
        public UpdateRoomRequestDtoValidator()
        {
            RuleFor(x => x.RoomNumber)
                .NotEmpty()
                .MaximumLength(20)
                .Matches(@"^[A-Za-z0-9\-]+$")
                .WithMessage("Room number can contain only letters, numbers, and hyphens.");

            RuleFor(x => x.PricePerNight)
                .GreaterThan(0)
                .WithMessage("Price per night must be greater than zero.");

            RuleFor(x => x.RoomTypeId)
                .Must(BeValidRoomType)
                .WithMessage("Invalid room type.");

            RuleFor(x => x.RoomPictures)
                .NotNull();

            RuleForEach(x => x.RoomPictures)
                .NotEmpty()
                .Must(BeValidImageUrl)
                .WithMessage("Invalid image URL.");

            RuleFor(x => x.RoomPictures)
                .Must(p => p.Count() <= 10)
                .WithMessage("A room cannot have more than 10 pictures.");
        }

        private bool BeValidRoomType(int roomTypeId)
        {
            return Enum.IsDefined(typeof(RoomTypeCode), roomTypeId);
        }

        private bool BeValidImageUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }


    }
}
