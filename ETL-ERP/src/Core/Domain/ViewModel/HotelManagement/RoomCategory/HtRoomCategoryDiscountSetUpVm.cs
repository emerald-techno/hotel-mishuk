using Domain.Entities.HotelManagement;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.RoomCategory;

public class HtRoomCategoryDiscountSetUpVm
{
    //public long Id { get; set; }
    public long RoomCategoryId { get; set; }
    public string RoomCategory { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string FromDateStr { get; set; }
    public string ToDateStr { get; set; }
    public DiscountTypeEnum DiscountType { get; set; }
    public double DiscountAmount { get; set; }
    public double ActualAmount { get; set; }
    public double NewOfferRate { get; set; }
    public bool isPercent { get; set; }

}
