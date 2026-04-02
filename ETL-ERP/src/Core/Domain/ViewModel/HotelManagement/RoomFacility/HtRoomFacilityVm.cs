using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ViewModel.HotelManagement.RoomFacility;

public class HtRoomFacilityVm
{
    public long Id { get; set; }
    public long FacilityCategoryId { get; set; }
    public string FacilityCategoryName { get; set; }

    [StringLength(120, ErrorMessage = "Facility name name can not be more than 120 characters")]
    [Required(ErrorMessage = "Facility name name is required")]
    [Remote(action: "IsNameExist", controller: "RoomFacility", AdditionalFields = "InitName")]
    public string FacilityName { get; set; }
    public bool IsActive { get; set; }

    [StringLength(120, ErrorMessage = "Description can not be more than 120 characters")]
    public string Description { get; set; }
    public IEnumerable<SelectListItem> FacilitCategoryLookUp { get; set; }
}
