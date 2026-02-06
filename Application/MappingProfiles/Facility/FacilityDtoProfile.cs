using Application.Dtos.Facility;
using AutoMapper;

namespace Application.MappingProfiles.Facility;

public class FacilityDtoProfile : Profile
{
    public FacilityDtoProfile()
    {
        CreateMap<Domain.Models.Facility, FacilityResponseDto>()
            .ForMember(dest => dest.FacilityId,opt => opt.MapFrom(src => src.Id));

        CreateMap<CreateFacilityDto, Domain.Models.Facility>();

        CreateMap<UpdateFacilityDto, Domain.Models.Facility>();
    }
}
