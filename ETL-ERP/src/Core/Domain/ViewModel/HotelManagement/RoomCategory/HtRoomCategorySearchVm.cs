using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.RoomCategory;

public class HtRoomCategorySearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string CategoryName { get; set; }
    public bool IsAc { get; set; }
    public bool IsBalcony { get; set; }
    public long BedTypeId { get; set; }
    public string BedTypeName { get; set; }
    public int BedNumber { get; set; }
    public string OtherInfo { get; set; }
    public int Capacity { get; set; }
    public double Rent { get; set; }
    public double ServiceCharge { get; set; }
    public double TotalRent { get; set; }
    public string Remarks { get; set; }
    public string PhotoUrl { get; set; }
    public bool isActive { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }

    //---------------------------------------
    public IEnumerable<SelectListItem> BedTypeLookUp { get; set; }
}
