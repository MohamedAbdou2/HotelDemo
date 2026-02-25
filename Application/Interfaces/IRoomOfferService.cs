using Application.Dtos;
using Application.Dtos.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRoomOfferService
    {
        Task<ResponseDto<IEnumerable<OfferResponseDto>>> GetOffersByRoomIdAsync(Guid roomId);
        Task<ResponseDto<RoomOfferResponseDto>> GetOfferForRoomAsync(Guid roomId, Guid offerId);

    }
}
