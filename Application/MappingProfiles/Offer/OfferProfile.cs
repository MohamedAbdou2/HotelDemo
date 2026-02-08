using Application.Dtos.Offers;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfiles.Offer
{
    public class OfferProfile : Profile
    {
        public OfferProfile()
        {
            // Offer -> OfferDto
            CreateMap<Domain.Models.Offer, OfferDto>()
                .ForMember(d => d.RoomIds, opt => opt.MapFrom(s => s.RoomOffers.Select(ro => ro.RoomId))).ReverseMap();

            // CreateOfferDto -> Offer (map RoomIds to RoomOffers)
            CreateMap<CreateOfferDto, Domain.Models.Offer>()
                .ForMember(d => d.RoomOffers, opt =>
                {
                    opt.PreCondition(src => src.RoomIds != null);
                    opt.MapFrom(src => src.RoomIds.Select(id => new RoomOffer { RoomId = id }));
                }).ReverseMap();

            // UpdateOfferDto -> Offer
            CreateMap<UpdateOfferDto, Domain.Models.Offer>()
                .ForMember(d => d.RoomOffers, opt =>
                {
                    opt.PreCondition(src => src.RoomIds != null);
                    opt.MapFrom(src => src.RoomIds.Select(id => new RoomOffer { RoomId = id }));
                })
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}