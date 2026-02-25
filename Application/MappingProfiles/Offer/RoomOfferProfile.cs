using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles.Offer
{
    public class RoomOfferProfile :Profile
    {
        public RoomOfferProfile() { 
        
            CreateMap<Domain.Models.RoomOffer, Dtos.Offers.RoomOfferResponseDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Offer.Title))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.Offer.DiscountPercentage))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Offer.IsActive))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Offer.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Offer.EndDate));


        }
    }
}
