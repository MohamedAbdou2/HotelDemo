using HotelDemo.Data.Enums;

namespace HotelDemo.Data.Models
{
    public class Report : BaseModel
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;

        public ReportTypeCode? ReportTypeId { get; set; }

        public ReportType? ReportType { get; set; }

        public ReportPeriodCode? ReportPeriodId { get; set; }
        public ReportPeriod? ReportPeriod { get; set; }


    }
}
