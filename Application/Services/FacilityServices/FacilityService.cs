using Application.Dtos;
using Application.Dtos.Facility;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.FacilityServices;

public class FacilityService : IFacilityService
{
    private readonly IMapper _mapper;
    private readonly IReadOnlyRepository<Facility> _facilityRepo;
    public FacilityService(IReadOnlyRepository<Facility> facilityRepo, IMapper mapper)
    {
        _mapper = mapper;
        _facilityRepo = facilityRepo;
    }


    public async Task<ResponseDto<IEnumerable<FacilityResponseDto>>> GetAllFacilitiesAsync()
    {
        var facilities = await _facilityRepo.GetAll(null).ProjectTo<FacilityResponseDto>(_mapper.ConfigurationProvider).ToListAsync();

        return ResponseDto<IEnumerable<FacilityResponseDto>>.Success(facilities);
    }
}
