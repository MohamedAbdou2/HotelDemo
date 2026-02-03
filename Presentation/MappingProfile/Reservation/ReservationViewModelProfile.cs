using Application.Dtos.Reservation;
using AutoMapper;
using Presentation.ViewModels.Reservation;

namespace Presentation.MappingProfile.Reservation
{
    public class ReservationViewModelProfile : Profile
    {
        public ReservationViewModelProfile()
        {
            // ViewModel to DTO (Input)
            CreateMap<CreateReservationViewModel, ReservationDto>();

            // DTO to ViewModel (Output)
            CreateMap<ReservationResponseDto, ReservationResponseViewModel>()
                .ForMember(dest => dest.NumberOfNights, 
                    opt => opt.MapFrom(src => (src.CheckOutDate - src.CheckInDate).Days))
                .ForMember(dest => dest.StatusCode, 
                    opt => opt.MapFrom(src => (int)src.ReservationStatusId))
                .ForMember(dest => dest.ExpiresIn, 
                    opt => opt.MapFrom(src => GetTimeRemaining(src.ExpiresAt)));
        }

        private string? GetTimeRemaining(DateTime? expiresAt)
        {
            if (!expiresAt.HasValue)
                return null;

            var remaining = expiresAt.Value - DateTime.UtcNow;

            if (remaining.TotalSeconds <= 0)
                return "Expired";

            if (remaining.TotalMinutes < 1)
                return $"{(int)remaining.TotalSeconds} seconds";

            if (remaining.TotalHours < 1)
                return $"{(int)remaining.TotalMinutes} minutes";

            return $"{(int)remaining.TotalHours} hours, {remaining.Minutes} minutes";
        }
    }
}
