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

    [HttpPost]
    public async Task<ResponseViewModel<bool>> CreateFacilityAsync([FromBody] CreateFacilityViewModel vm)
    {
        var createFaciltyDto = _mapper.Map<CreateFacilityDto>(vm);
        var responseDto = await _facilityService.CreateFacilityAsync(createFaciltyDto);

        if (!responseDto.IsSuccess)
            return ResponseViewModel<bool>.Fail(responseDto.ErrorCode, responseDto.Message);

        return ResponseViewModel<bool>.Success(responseDto.Data);
    }

    [HttpGet]
    public async Task<ResponseViewModel<IEnumerable<FacilityResponseViewModel>>> GetAllFacilitiesAsync()
    {
        var responseDto = await _facilityService.GetAllFacilitiesAsync();
        if (!responseDto.IsSuccess)
        {
            return ResponseViewModel<IEnumerable<FacilityResponseViewModel>>.Fail(responseDto.ErrorCode, responseDto.Message);
        }
        var facilityViewModels = _mapper.Map<IEnumerable<FacilityResponseViewModel>>(responseDto.Data);
        return ResponseViewModel<IEnumerable<FacilityResponseViewModel>>.Success(facilityViewModels);
    }

    [HttpGet]
    public async Task<ResponseViewModel<FacilityResponseViewModel>> GetFacilityByIdAsync([FromQuery] int facilityId)
    {
        var responseDto = await _facilityService.GetFacilityByIdAsync(facilityId);
        if (!responseDto.IsSuccess)
        {
            return ResponseViewModel<FacilityResponseViewModel>.Fail(responseDto.ErrorCode, responseDto.Message);
        }
        var facilityViewModel = _mapper.Map<FacilityResponseViewModel>(responseDto.Data);
        return ResponseViewModel<FacilityResponseViewModel>.Success(facilityViewModel);
    }

    [HttpPut("{id}")]
    public async Task<ResponseViewModel<bool>> UpdateFacilityAsync(int id, [FromBody] UpdateFacilityViewModel vm)
    {
        var updateFacilityDto = _mapper.Map<UpdateFacilityDto>(vm);
        var responseDto = await _facilityService.UpdateFacilityAsync(id, updateFacilityDto);

        if (!responseDto.IsSuccess)
            return ResponseViewModel<bool>.Fail(responseDto.ErrorCode, responseDto.Message);

        return ResponseViewModel<bool>.Success(responseDto.Data);
    }

    [HttpDelete("{id}")]
    public async Task<ResponseViewModel<bool>> DeleteFacilityAsync([FromQuery] int id)
    {
        var responseDto = await _facilityService.DeleteFacilityAsync(id);

        if (!responseDto.IsSuccess)
            return ResponseViewModel<bool>.Fail(responseDto.ErrorCode, responseDto.Message);

        return ResponseViewModel<bool>.Success(responseDto.Data, "DeleteFacilityAsync is not implemented.");
    }

}
