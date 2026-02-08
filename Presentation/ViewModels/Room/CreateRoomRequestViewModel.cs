namespace Presentation.ViewModels.Room
{
    public class CreateRoomRequestViewModel
    {
        public string RoomNumber { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int RoomTypeId { get; set; }
        public IEnumerable<string> RoomPictures { get; set; } = new List<string>();
        public IEnumerable<int> FacilitiesIds { get; set; } = new List<int>();

    }
}
