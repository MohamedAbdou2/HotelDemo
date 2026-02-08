using Application.Dtos;
using Application.Dtos.Offers;

namespace Application.Interfaces
{
    public interface IOffers
    {
        Task<ResponseDto<string>> CreateAsync(CreateOfferDto dto);
        Task<ResponseDto<IEnumerable<OfferDto>>> GetAllAsync(bool onlyActive = false);
        Task<ResponseDto<OfferDto>> GetByIdAsync(Guid id);
        Task<ResponseDto<bool>> UpdateAsync(Guid id, UpdateOfferDto dto);
        Task<ResponseDto<bool>> DeleteAsync(Guid id);
    }
}
