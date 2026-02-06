using Application.Dtos;
using Application.Dtos.Facility;
using AutoMapper;
using HotelDemo.ViewModels;
using Presentation.ViewModels.Facility;

namespace Presentation.MappingProfile.Facility;

public class FacilityViewModelProfile : Profile
{
    public FacilityViewModelProfile()
    {
        CreateMap<ResponseDto<FacilityResponseDto>, ResponseViewModel<FacilityResponseViewModel>>();
        CreateMap<FacilityResponseDto, FacilityResponseViewModel>();

        CreateMap<CreateFacilityViewModel, CreateFacilityDto>();
        CreateMap<ResponseDto<bool>, ResponseViewModel<bool>>();

        CreateMap<UpdateFacilityViewModel, UpdateFacilityDto>();
    }
}
