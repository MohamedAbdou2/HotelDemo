namespace Presentation.ViewModels.Room
{
    public class UpdateRoomRequestViewModel
    {
        public string RoomNumber { get; set; } = null!;
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
        public List<string> RoomPictures { get; set; } = new List<string>();
    }
}
