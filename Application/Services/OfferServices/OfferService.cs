using Application.Dtos;
using Application.Dtos.Offers;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.OfferServices
{
    public class OfferService : IOffers
    {
        private readonly IGenericRepository<Offer> _repository;

        private readonly IMapper _mapper;
        private readonly IGenericRepository<Room> roomRepository;

        public OfferService(IGenericRepository<Offer> repository,
                            IMapper mapper, IGenericRepository<Room> _roomRepository)
        {
            _repository = repository;
            _mapper = mapper;
            roomRepository = _roomRepository;
        }

        public async Task<ResponseDto<string>> CreateAsync(CreateOfferDto dto)
        {
            if (dto.StartDate >= dto.EndDate)
                return ResponseDto<string>.Fail(Domain.Enums.ErrorCode.ValidationError, "EndDate must be after StartDate");

            //if (!await _repository.RoomsExistAsync(dto.RoomIds))
            //    return ResponseDto<string>.Fail(Domain.Enums.ErrorCode.ValidationError, "One or more room ids are invalid.");

            var offer = _mapper.Map<Offer>(dto);
            offer.IsActive = dto.IsActive ?? true;

            var res = await _repository.Add(offer);
            if (res == false)
                return ResponseDto<string>.Fail(Domain.Enums.ErrorCode.ServerError, "Failed to create offer");

            return ResponseDto<string>.Success("new", "Offer created successfully");
        }

        public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
        {
            var offeQueryable = await _repository.GetbyId(id);
            var offer = await offeQueryable.FirstOrDefaultAsync();
            if (offer == null)
                return ResponseDto<bool>.Fail(Domain.Enums.ErrorCode.NotFound, "Offer not found");

            var isDeleted = await _repository.UpdateIncludeAsync(offer, nameof(Offer.IsDeleted));
            if (!isDeleted)
                return ResponseDto<bool>.Fail(Domain.Enums.ErrorCode.ServerError, "Failed to delete offer");

            return ResponseDto<bool>.Success(true, "Offer deleted");
        }

        public async Task<ResponseDto<IEnumerable<OfferDto>>> GetAllAsync(bool onlyActive = false)
        {
            var listQuerable = await _repository.GetAll(x => x.IsActive == onlyActive);
            var offersList = await _mapper.ProjectTo<OfferDto>(listQuerable).ToListAsync();
            return ResponseDto<IEnumerable<OfferDto>>.Success(offersList);
        }

        public async Task<ResponseDto<OfferDto>> GetByIdAsync(Guid id)
        {
            var offerQueryable = await _repository.GetbyId(id);
            var offer = await offerQueryable.FirstOrDefaultAsync();
            if (offer == null)
                return ResponseDto<OfferDto>.Fail(Domain.Enums.ErrorCode.NotFound, "Offer not found");

            var dto = _mapper.Map<OfferDto>(offerQueryable);
            return ResponseDto<OfferDto>.Success(dto);
        }

        public async Task<ResponseDto<bool>> UpdateAsync(Guid id, UpdateOfferDto dto)
        {
            // dto.Id = id;

            if (!await _repository.IsExist(e => e.Id == id))
                return ResponseDto<bool>.Fail(Domain.Enums.ErrorCode.NotFound, "Offer not found");

            if (dto.StartDate != default && dto.EndDate != default)
            {
                if (dto.StartDate >= dto.EndDate)
                    return ResponseDto<bool>.Fail(Domain.Enums.ErrorCode.ValidationError, "EndDate must be after StartDate");
            }
            var start = dto.StartDate;
            var end = dto.EndDate;
            if (start >= end)
                return ResponseDto<bool>.Fail(Domain.Enums.ErrorCode.ValidationError, "EndDate must be after StartDate");

            //if (dto.RoomIds != null && !await _repository.RoomsExistAsync(dto.RoomIds))
            //    return ResponseDto<bool>.Fail(Domain.Enums.ErrorCode.ValidationError, "One or more room ids are invalid.");

            // Map non-null fields from dto to entity (AutoMapper configured to skip nulls)
            var newOffer = new Offer { Id = id };
            _mapper.Map(dto, newOffer);

            var paramsToUpdate = typeof(UpdateOfferDto).GetProperties()
                .Where(p => p.GetValue(dto) != null && p.Name.ToLower() != "Id".ToLower())
                .Select(p => p.Name)
                .ToArray();

            var updated = await _repository.UpdateIncludeAsync(newOffer, paramsToUpdate);
            if (!updated)
                return ResponseDto<bool>.Fail(Domain.Enums.ErrorCode.ServerError, "Failed to update offer");

            return ResponseDto<bool>.Success(true, "Offer updated successfully");
        }
    }
}