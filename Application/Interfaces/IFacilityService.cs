using Application.Dtos;
using Application.Dtos.Facility;

namespace Application.Interfaces;

public interface IFacilityService
{
    Task<ResponseDto<IEnumerable<FacilityResponseDto>>> GetAllFacilitiesAsync();
}
