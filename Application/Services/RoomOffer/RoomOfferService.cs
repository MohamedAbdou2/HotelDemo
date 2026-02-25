using Application.Dtos;
using Application.Dtos.Offers;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.RoomOffer
{
    public class RoomOfferService : IRoomOfferService
    {
        private readonly IGenericRepository<Domain.Models.RoomOffer> _offerRoomRepo;
        private readonly IMapper _mapper;
        public RoomOfferService(IGenericRepository<Domain.Models.RoomOffer> offerRoomRepo, IMapper mapper)
        {
            _offerRoomRepo = offerRoomRepo;
            _mapper = mapper;
        }

        public async Task<ResponseDto<IEnumerable<OfferResponseDto>>> GetOffersByRoomIdAsync(Guid roomId)
        {
            var offers = await _offerRoomRepo.Find(ro => ro.RoomId == roomId)
                                              .Include(ro => ro.Offer)
                                              .ProjectTo<OfferResponseDto>(_mapper.ConfigurationProvider)
                                              .ToListAsync();
            return ResponseDto<IEnumerable<OfferResponseDto>>.Success(offers);

        }
        public async Task<ResponseDto<RoomOfferResponseDto>> GetOfferForRoomAsync(Guid roomId, Guid offerId)
        {
            var offer = await _offerRoomRepo.Find(ro => ro.RoomId == roomId && ro.OfferId == offerId)
                                              .Include(ro => ro.Offer)
                                              .ProjectTo<RoomOfferResponseDto>(_mapper.ConfigurationProvider)
                                              .FirstOrDefaultAsync();
            return ResponseDto<RoomOfferResponseDto>.Success(offer!);

        }
    }
}
