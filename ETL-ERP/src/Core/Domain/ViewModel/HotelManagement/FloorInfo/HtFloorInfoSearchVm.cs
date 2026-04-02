using Domain.ModelInterface;

namespace Domain.ViewModel.HotelManagement.FloorInfo;

public class HtFloorInfoSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string FloorName { get; set; }
    public int TotalRoom { get; set; }
    public string StartRoomNo { get; set; }
    public string EndRoomNo { get; set; }
    public string Remarks { get; set; }
    public bool IsActive { get; set; }
    public DateTime ActionDate { get; set; }
    public long ActionById { get; set; }
    public DateTime? UpdateDate { get; set; }
    public long? UpdatedById { get; set; }
    public bool IsDeleted { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
