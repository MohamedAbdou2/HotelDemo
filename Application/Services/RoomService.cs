using Application.Dtos;
using Application.Dtos.Room;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
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
        private readonly IMapper _mapper;
        public RoomService(IGenericRepository<Room> roomRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        public async Task<ResponseDto<IEnumerable<GetRoomResponseDto>>> GetAllRooms()
        {
            var Query = await _roomRepository.GetAll();
            var rooms = Query.ProjectTo<GetRoomResponseDto>(_mapper.ConfigurationProvider).ToList();
            var response = ResponseDto<IEnumerable<GetRoomResponseDto>>.Success(rooms);
            return response;

        }



        public async Task<ResponseDto<GetRoomResponseDto>> GetRoomById(Guid roomId)
        {
            var Query = await _roomRepository.GetbyId(roomId);
            var room = Query.ProjectTo<GetRoomResponseDto>(_mapper.ConfigurationProvider).FirstOrDefault();
            if (room == null)
            {
                return ResponseDto<GetRoomResponseDto>.Fail(ErrorCode.RoomNotFound, "Room not found");
            }
            var response = ResponseDto<GetRoomResponseDto>.Success(room);
            return response;
        }

        public async Task<ResponseDto<bool>> CreateRoom(CreateRoomRequestDto dto)
        {
            var room = _mapper.Map<Room>(dto);
            var result = await _roomRepository.Add(room);
            if (!result)
            {
                return ResponseDto<bool>.Fail(ErrorCode.RoomCreationFailed, "Failed to create room");
            }
            return ResponseDto<bool>.Success(true, "Room created successfully");

        }


        public async Task<ResponseDto<bool>> UpdateRoom(Guid roomId, UpdateRoomRequestDto dto)
        {
            var roomExists = await _roomRepository.IsExist(r => r.Id == roomId);
            if (!roomExists)
            {
                return ResponseDto<bool>.Fail(ErrorCode.RoomNotFound, "Room not found");
            }
            var roomToUpdate = _mapper.Map<Room>(dto);
            roomToUpdate.Id = roomId;
            var propsToUpdate = new[] { nameof(Room.PricePerNight), nameof(Room.IsAvailable), nameof(Room.RoomTypeId) };
            var result = await _roomRepository.UpdateIncludeAsync(roomToUpdate, propsToUpdate);
            if (!result)
            {
                return ResponseDto<bool>.Fail(ErrorCode.RoomUpdateFailed, "Failed to update room");
            }
            return ResponseDto<bool>.Success(true, "Room updated successfully");
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



        public async Task<PaginatedResponseDto<IEnumerable<GetRoomResponseDto>>> GetRoomsByFilter(RoomFilterRequestDto filterDto)
        {
            var query = await _roomRepository.GetAll(r =>
                (!filterDto.RoomTypeId.HasValue || r.RoomTypeId == filterDto.RoomTypeId) &&
                (!filterDto.MinPrice.HasValue || r.PricePerNight >= filterDto.MinPrice) &&
                (!filterDto.MaxPrice.HasValue || r.PricePerNight <= filterDto.MaxPrice) &&
                (!filterDto.IsAvailable.HasValue || r.IsAvailable == filterDto.IsAvailable)
            );
            var rooms = query.ProjectTo<GetRoomResponseDto>(_mapper.ConfigurationProvider).ToList();
            var totalCount = rooms.Count;
            var pagedRooms = rooms
                .Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
                .Take(filterDto.PageSize)
                .ToList();
            var totalPages = (int)Math.Ceiling(totalCount / (double)filterDto.PageSize);
            var response = new PaginatedResponseDto<IEnumerable<GetRoomResponseDto>>
            {
                Data = pagedRooms,
                PageNumber = filterDto.PageNumber,
                PageSize = filterDto.PageSize,
                Count = totalCount,
                TotalPages = totalPages
            };

            return response;


        }
    }

    }
