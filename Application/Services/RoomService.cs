using Application.Dtos;
using Application.Dtos.Room;
using Application.Dtos.Room.RoomValidators;
using Application.Interfaces;
using Application.Validator;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IGenericRepository<Room> _roomRepository;
        private readonly IGenericRepository<RoomPicture> _roomPictureRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateRoomRequestDto> _createRoomValidator;
        private readonly IValidator<UpdateRoomRequestDto> _updateRoomValidator;
        private readonly IValidator<RoomFilterRequestDto> _roomFilterValidator;

        public RoomService(IGenericRepository<Room> roomRepository, IGenericRepository<RoomPicture> roomPictureRepository, IMapper mapper, IValidator<CreateRoomRequestDto> createRoomValidator, IValidator<UpdateRoomRequestDto> updateRoomValidator, IValidator<RoomFilterRequestDto> roomFilterValidator)
        {
            _roomRepository = roomRepository;
            _roomPictureRepository = roomPictureRepository;
            _mapper = mapper;
            _createRoomValidator = createRoomValidator;
            _updateRoomValidator = updateRoomValidator;
            _roomFilterValidator = roomFilterValidator;
        }

        public async Task<ResponseDto<IEnumerable<GetRoomResponseDto>>> GetAllRooms()
        {
            var rooms = await _roomRepository.GetAll().ProjectTo<GetRoomResponseDto>(_mapper.ConfigurationProvider).ToListAsync();
            var response = ResponseDto<IEnumerable<GetRoomResponseDto>>.Success(rooms);
            return response;

        }



        public async Task<ResponseDto<GetRoomResponseDto>> GetRoomById(Guid roomId)
        {
            var room =await _roomRepository.GetbyId(roomId).ProjectTo<GetRoomResponseDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
            if (room == null)
            {
                return ResponseDto<GetRoomResponseDto>.Fail(ErrorCode.RoomNotFound, "Room not found");
            }
            var response = ResponseDto<GetRoomResponseDto>.Success(room);
            return response;
        }

        public async Task<ResponseDto<bool>> CreateRoom(CreateRoomRequestDto dto)
        {
            var validator = _createRoomValidator.Validate(dto);
            if (!validator.IsValid)
                return ResponseDto<bool>.ValidationFail(validator);

            var room = _mapper.Map<Room>(dto);
            room.RoomFacilities = dto.FacilitiesIds.Select(x => new RoomFacility { RoomId = room.Id,FacilityId = (RoomFacilityCode)x}).ToList();
            var result = await _roomRepository.Add(room);
            if (!result)
            {
                return ResponseDto<bool>.Fail(ErrorCode.RoomCreationFailed, "Failed to create room");
            }
            return ResponseDto<bool>.Success(true, "Room created successfully");

        }


        public async Task<ResponseDto<bool>> UpdateRoom(Guid roomId, UpdateRoomRequestDto dto)
        {
            var validator = _updateRoomValidator.Validate(dto);
            if (!validator.IsValid)
                return ResponseDto<bool>.ValidationFail(validator);
            
            var roomExists = await _roomRepository.IsExist(r => r.Id == roomId);
            if (!roomExists)
            {
                return ResponseDto<bool>.Fail(ErrorCode.RoomNotFound, "Room not found");
            }
            var roomToUpdate = _mapper.Map<Room>(dto);
            roomToUpdate.Id = roomId;
            var propsToUpdate = new[] 
            { 
                nameof(Room.RoomNumber), 
                nameof(Room.PricePerNight), 
                nameof(Room.IsAvailable), 
                nameof(Room.RoomTypeId) 
            };
            
            var result = await _roomRepository.UpdateIncludeAsync(roomToUpdate, propsToUpdate);
            if (!result)
            {
                return ResponseDto<bool>.Fail(ErrorCode.RoomUpdateFailed, "Failed to update room");
            }

            await UpdateRoomPicturesAsync(roomId, dto.RoomPictures);

            return ResponseDto<bool>.Success(true, "Room updated successfully");
        }

        private async Task UpdateRoomPicturesAsync(Guid roomId, List<string> newPictureUrls)
        {
            var existingPictureIds = await _roomPictureRepository.GetAll(rp => rp.RoomId == roomId).Select(rp => rp.Id).ToListAsync();
            
            foreach (var pictureId in existingPictureIds)
            {
                await _roomPictureRepository.Delete(pictureId);
            }

            foreach (var pictureUrl in newPictureUrls)
            {
                var newPicture = new RoomPicture
                {
                    RoomId = roomId,
                    PictureUrl = pictureUrl
                };
                await _roomPictureRepository.Add(newPicture);
            }
        }



        public async Task<ResponseDto<bool>> DeleteRoom(Guid roomId)
        {
            var roomExists = await _roomRepository.IsExist(r => r.Id == roomId);
            if (!roomExists)
            {
                return ResponseDto<bool>.Fail(ErrorCode.RoomNotFound, "Room not found");
            }
            var result = await _roomRepository.Delete(roomId);
            if (!result)
            {
                return ResponseDto<bool>.Fail(ErrorCode.RoomDeletionFailed, "Failed to delete room");
            }
            return ResponseDto<bool>.Success(true, "Room deleted successfully");
        }



        public async Task<ResponseDto<PaginatedListResponseDto<GetRoomResponseDto>>> GetRoomsByFilter(RoomFilterRequestDto filterDto)
        {
            var validator = _roomFilterValidator.Validate(filterDto);
            if (!validator.IsValid)
                return ResponseDto<PaginatedListResponseDto<GetRoomResponseDto>>.ValidationFail(validator);
            var filteredRooms =  _roomRepository.GetAll(r =>
                (!filterDto.RoomTypeId.HasValue || r.RoomTypeId == filterDto.RoomTypeId) &&
                (!filterDto.MinPrice.HasValue || r.PricePerNight >= filterDto.MinPrice) &&
                (!filterDto.MaxPrice.HasValue || r.PricePerNight <= filterDto.MaxPrice) &&
                (!filterDto.IsAvailable.HasValue || r.IsAvailable == filterDto.IsAvailable)
            ).ProjectTo<GetRoomResponseDto>(_mapper.ConfigurationProvider);

            var paginatedRooms = await PaginatedListResponseDto<GetRoomResponseDto>.CreateAsync(
                filteredRooms,
                filterDto.PageNumber,
                filterDto.PageSize
            );
            var response = ResponseDto<PaginatedListResponseDto<GetRoomResponseDto>>.Success(paginatedRooms);
            return response;

        }
    }

}
