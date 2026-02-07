using Application.Dtos.Facility;
using Application.Interfaces;
using AutoMapper;
using HotelDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Presentation.ViewModels.Facility;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[Controller]/[Action]")]
public class FacilityController : ControllerBase
{
    private readonly IFacilityService _facilityService;
    private readonly IMapper _mapper;

    public FacilityController(IFacilityService facilityService, IMapper mapper)
    {
        _facilityService = facilityService;
        _mapper = mapper;
    }


    [HttpGet]
    public async Task<ResponseViewModel<IEnumerable<FacilityResponseViewModel>>> GetAllFacilitiesAsync()
    {
        var responseDto = await _facilityService.GetAllFacilitiesAsync();

        var facilityViewModels = _mapper.Map<IEnumerable<FacilityResponseViewModel>>(responseDto.Data);

        return ResponseViewModel<IEnumerable<FacilityResponseViewModel>>.Success(facilityViewModels);
    }

}
