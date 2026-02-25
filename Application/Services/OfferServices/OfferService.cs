using Application.Dtos;
using Application.Dtos.Offers;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.OfferServices
{
    public class OfferService : IOfferService
    {
        private readonly IGenericRepository<Offer> _offerRepo;
        private readonly IGenericRepository<Room> _roomRepo;
        private readonly IMapper _mapper;

        public OfferService(IGenericRepository<Offer> repository,
                            IMapper mapper, IGenericRepository<Room> roomRepo)
        {
            _offerRepo = repository;
            _mapper = mapper;
            _roomRepo = roomRepo;
            
        }

        public async Task<ResponseDto<object>> CreateOfferAsync(CreateOfferDto dto)
        {
            var allRoomIdsExist = await _roomRepo.Find(r => dto.RoomIds.Contains(r.Id))
                                          .CountAsync() == dto.RoomIds.Distinct().Count();

            if (!allRoomIdsExist)
                return ResponseDto<object>.Fail(ErrorCode.RoomNotFound, "One Room Or More Not Found.");

            var offer = _mapper.Map<Offer>(dto);

            var res = await _offerRepo.Add(offer);

            if (!res)
                return ResponseDto<object>.Fail(ErrorCode.OfferCreationFailed, "Failed to create offer");

            return ResponseDto<object>.Success(null, "Offer created successfully");
        }

        public async Task<ResponseDto<IEnumerable<OfferResponseDto>>> GetAllOffersAsync()
        {
            var offers = await _offerRepo.GetAll().ProjectTo<OfferResponseDto>(_mapper.ConfigurationProvider)
                                          .ToListAsync();

            return ResponseDto<IEnumerable<OfferResponseDto>>.Success(offers);
        }

        public async Task<ResponseDto<OfferResponseDto>> GetOfferByIdAsync(Guid id)
        {
            var offer = await _offerRepo.GetbyId(id).ProjectTo<OfferResponseDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

            if (offer is null)
                return ResponseDto<OfferResponseDto>.Fail(ErrorCode.NotFound, "Offer not found");

            return ResponseDto<OfferResponseDto>.Success(offer);
        }

        public async Task<ResponseDto<object>> UpdateOfferAsync(Guid id, UpdateOfferDto dto)
        {
            var isExistOffer = await _offerRepo.IsExist(e => e.Id == id);

            if (!isExistOffer)
                return ResponseDto<object>.Fail(ErrorCode.NotFound, "Offer not found");

            var newOffer = new Offer { Id = id };
            _mapper.Map(dto, newOffer);

            var paramsToUpdate = typeof(UpdateOfferDto).GetProperties()
                                                       .Where(p => p.GetValue(dto) is not null)
                                                       .Select(p => p.Name)
                                                       .ToArray();

            var updated = await _offerRepo.UpdateIncludeAsync(newOffer, paramsToUpdate);

            if (!updated)
                return ResponseDto<object>.Fail(ErrorCode.OfferUpdateFailed, "Failed to update offer");

            return ResponseDto<object>.Success(null, "Offer updated successfully");
        }

        public async Task<ResponseDto<object>> DeleteOfferAsync(Guid id)
        {

            var offer = await _offerRepo.GetbyId(id).FirstOrDefaultAsync();

            if (offer is null)
                return ResponseDto<object>.Fail(ErrorCode.OfferNotFound, "Offer Not Found");

            var isDeleted = await _offerRepo.SoftDeleteAsync(offer);


            if (!isDeleted)
                return ResponseDto<object>.Fail(ErrorCode.ServerError, "Failed To Delete Offer");

            return ResponseDto<object>.Success(null, "Offer deleted");

        }
      
    }
}