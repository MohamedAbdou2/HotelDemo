using Application.Dtos;
using Application.Dtos.Offers;

namespace Application.Interfaces
{
    public interface IOffers
    {
        Task<ResponseDto<object>> CreateOfferAsync(CreateOfferDto dto);
        Task<ResponseDto<IEnumerable<OfferResponseDto>>> GetAllOffersAsync();
        Task<ResponseDto<OfferResponseDto>> GetOfferByIdAsync(Guid id);
        Task<ResponseDto<object>> UpdateOfferAsync(Guid id, UpdateOfferDto dto);
        Task<ResponseDto<object>> DeleteOfferAsync(Guid id);
    }
}
