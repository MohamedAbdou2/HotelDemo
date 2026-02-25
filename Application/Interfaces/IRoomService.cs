using Application.Dtos;
using Application.Dtos.Room;

namespace Application.Interfaces
{
    public interface IRoomService
    {
        Task<ResponseDto<IEnumerable<GetRoomResponseDto>>> GetAllRooms();
        Task<ResponseDto<GetRoomResponseDto>> GetRoomById(Guid roomId);

        Task<ResponseDto<bool>> CreateRoom(CreateRoomRequestDto dto);

        Task<ResponseDto<bool>> UpdateRoom(Guid roomId, UpdateRoomRequestDto dto);

        Task<ResponseDto<bool>> DeleteRoom(Guid roomId);
        Task<ResponseDto<PaginatedListResponseDto<GetRoomResponseDto>>> GetRoomsByFilter(RoomFilterRequestDto filterDto);

       

    }
}
