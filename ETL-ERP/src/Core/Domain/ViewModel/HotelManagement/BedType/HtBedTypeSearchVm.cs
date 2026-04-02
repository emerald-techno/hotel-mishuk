using Domain.ModelInterface;

namespace Domain.ViewModel.HotelManagement.BedType;

public class HtBedTypeSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string TypeName { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}