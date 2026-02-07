namespace Presentation.ViewModels.Facility;

public class AssignFacilitiesToRoomViewModel
{
    public Guid RoomId { get; set; }
    public IEnumerable<int> RoomIds { get; set; }

}
