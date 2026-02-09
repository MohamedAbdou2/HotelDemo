using Application.Dtos.Offers;
using AutoMapper;
using Presentation.ViewModels.Offer;

namespace Presentation.MappingProfile.Offer;

public class OfferViewModelProfile : Profile
{
    public OfferViewModelProfile()
    {
        CreateMap<CreateOfferViewModel, CreateOfferDto>();

        CreateMap<OfferResponseDto, OfferResponseViewModel>()
            .ForMember(dest => dest.RoomDetails,opt => opt.MapFrom(src => src.RoomDetails));

        CreateMap<UpdateOfferViewModel, UpdateOfferDto>();
    }
}
