namespace Application.Dtos.Facility
{
    public class AssignFacilitiesToRoomDto
    {
        public Guid RoomId { get; set; }
        public IEnumerable<int> RoomIds { get; set; }
    }
}
