using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.RoomFacilityCategory;

public class HtRoomFacilityCategoryVm
{
    public long Id { get; set; }

    [StringLength(120, ErrorMessage = "Category name can not be more than 120 characters")]
    [Required(ErrorMessage = "Category name is required")]
    [Remote(action: "IsNameExist", controller: "RoomFacilityCategory", AdditionalFields = "InitName")]
    public string CategoryName { get; set; }

    public bool IsActive { get; set; }

    [StringLength(120, ErrorMessage = "Description can not be more than 120 characters")]
    public string Description { get; set; }
}
