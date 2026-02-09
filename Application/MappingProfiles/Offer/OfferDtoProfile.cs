using Application.Dtos.Offers;
using Application.Dtos.Room;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfiles.Offer
{
    public class OfferDtoProfile : Profile
    {
        public OfferDtoProfile()
        {

            CreateMap<Domain.Models.Offer, OfferResponseDto>()
                .ForMember(d => 
                    d.RoomDetails,
                    opt => opt.MapFrom(
                        s => s.RoomOffers
                        .Select(ro => new RoomDetailsDto { 
                            Id = ro.RoomId,
                            RoomNumber = ro.Room.RoomNumber,
                            Type = ro.Room.Type.Name
                        })
                    )
                );

            CreateMap<CreateOfferDto, Domain.Models.Offer>()
                .ForMember(d => d.RoomOffers, opt => opt.MapFrom(src => src.RoomIds.Select(id => new RoomOffer { RoomId = id })));

            CreateMap<UpdateOfferDto, Domain.Models.Offer>();

        }
    }
}