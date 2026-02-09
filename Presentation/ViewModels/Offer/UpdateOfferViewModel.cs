namespace Presentation.ViewModels.Offer
{
    public class UpdateOfferViewModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
