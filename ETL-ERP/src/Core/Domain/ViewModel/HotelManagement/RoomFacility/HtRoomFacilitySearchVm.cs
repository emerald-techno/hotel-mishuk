
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.RoomFacility;

public class HtRoomFacilitySearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public long FacilityCategoryId { get; set; }
    public string FacilityCategoryName { get; set; }
    public string FacilityName { get; set; }
    public string Description { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }

    public IEnumerable<SelectListItem> FacilitCategoryLookUp { get; set; }
}
