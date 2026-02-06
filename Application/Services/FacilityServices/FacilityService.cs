using Application.Dtos;
using Application.Dtos.Facility;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.FacilityServices;

public class FacilityService : IFacilityService
{
    private readonly IFacilityRepository _facilityRepo;
    private readonly IMapper _mapper;

    public FacilityService(IFacilityRepository facilityRepo, IMapper mapper)
    {
        _facilityRepo = facilityRepo;
        _mapper = mapper;
    }

    public async Task<ResponseDto<bool>> CreateFacilityAsync(CreateFacilityDto dto)
    {
        var facility = _mapper.Map<Facility>(dto);

        await _facilityRepo.AddFacilityAsync(facility);

        return ResponseDto<bool>.Success(true);
    }

    public async Task<ResponseDto<IEnumerable<FacilityResponseDto>>> GetAllFacilitiesAsync()
    {
        var facilities = await _facilityRepo.GetAllFacilities().ProjectTo<FacilityResponseDto>(_mapper.ConfigurationProvider).ToListAsync();
        return ResponseDto<IEnumerable<FacilityResponseDto>>.Success(facilities);

    }

    public async Task<ResponseDto<FacilityResponseDto>> GetFacilityByIdAsync(int id)
    {
        var facility = await _facilityRepo.GetFacilityById(id).FirstOrDefaultAsync();

        var facilityDto = _mapper.Map<FacilityResponseDto>(facility);

        return ResponseDto<FacilityResponseDto>.Success(facilityDto);
    }

    public async Task<ResponseDto<bool>> UpdateFacilityAsync(int id, UpdateFacilityDto dto)
    {
        var exists = await _facilityRepo.GetFacilityById(dto.Id).AnyAsync();

        if (!exists)
            return ResponseDto<bool>.Fail(ErrorCode.NotFound, "Facility not found");

        var facility = _mapper.Map<Facility>(dto);
        await _facilityRepo.UpdateFacility(facility);
        return ResponseDto<bool>.Success(true);
    }

    public async Task<ResponseDto<bool>> DeleteFacilityAsync(int id)
    {
        var facility = await _facilityRepo.GetFacilityById(id).FirstOrDefaultAsync();

        if (facility == null)
            return ResponseDto<bool>.Fail(ErrorCode.NotFound, "Facility not found");

        await _facilityRepo.DeleteFacility(facility);
        return ResponseDto<bool>.Success(true);
    }
}
