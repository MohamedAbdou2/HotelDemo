using Application.Dtos;
using Application.Dtos.Facility;

namespace Application.Interfaces;

public interface IFacilityService
{
    Task<ResponseDto<IEnumerable<FacilityResponseDto>>> GetAllFacilitiesAsync();
    Task<ResponseDto<FacilityResponseDto>> GetFacilityByIdAsync(int id);
    Task<ResponseDto<bool>> CreateFacilityAsync(CreateFacilityDto dto);
    Task<ResponseDto<bool>> UpdateFacilityAsync(int id,UpdateFacilityDto dto);
    Task<ResponseDto<bool>> DeleteFacilityAsync(int id);
}
