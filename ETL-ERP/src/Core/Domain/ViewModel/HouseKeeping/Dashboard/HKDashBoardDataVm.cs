using Domain.Entities.HotelManagement;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HouseKeeping.Dashboard;

public class HKDashBoardDataVm
{
    public long RoomId { get; set; }
    public string RoomNo { get; set; }
    public long? CategoryId { get; set; }
    public string CategoryName { get; set; }
    public long? HouseKeeperId { get; set; }
    public string AssignedHouseKeeperName { get; set; }
    public long? AssignedHouseKeeperId { get; set; }
    public int Status { get; set; } //0 = Available,1 = Occupied, 2 = OutOfOrder,3 = TodayCheckIn ,4 = Expected C/Out ,5 = Reserved,  6 = Already Checked Out,7 = N/A
    //public string StatusText => Status switch { 0 => "No Info", 1 => "Booked", 2 => "Occupied", 3 => "Available", 4 => "Out of Order", 6 => "Today's Checkin", _ => "" };

    public string StatusText => Status switch { 0 => "Available", 1 => "Occupied", 2 => "O.O.O", 3 => "T. C/In", 4 => "E. C/Out", 5 => "Reserved", 6 => "C/Out", 7 => "Not Matched", _ => "" };
    public int CleaningStatus { get; set; } = 0; //0= No Info, 4 = Out Of Order, 5=VD
    public string CleaningStatusText => CleaningStatus switch
    {
       
        0 => "Vacant & Clean",
        1 => "Vacant & Dirty",
        2 => "Occupied",
        3 => "Check Out",
        4 => "Out of Order",
        _ => string.Empty
    };
    public bool IsTodayCheckout { get; set; }
}