namespace Domain.ViewModel.HotelManagement.HotelReport
{
    public class RoomOccupancyReportVm
    {
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set;}
        public DateTime FromDate { get; set;}
        public DateTime ToDate { get; set;}

        public string StrReportDate { get; set;}
        public DateTime ReportDate { get; set;}
        public int TotalRoom { get; set; }
        public int RoomSold { get; set; }
        public decimal SalesAmount { get; set; }

        public decimal OccupencyRate { get; set; }
    }

}
