using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.FloorInfo;

public class HtFloorInfoVm
{
    public long Id { get; set; }

    [StringLength(120, ErrorMessage = "Floor name can not be more than 120 characters")]
    [Required(ErrorMessage = "Floor name is required")]
    [Remote(action: "IsNameExist", controller: "FloorInfo", AdditionalFields = "InitName")]
    public string FloorName { get; set; }

    [Required(ErrorMessage = "Total room is required")]
    public int TotalRoom { get; set; }

    [Required(ErrorMessage = "Start room number is required")]
    public string StartRoomNo { get; set; }

    [Required(ErrorMessage = "End room number is required")]
    public string EndRoomNo { get; set; }

    public bool IsActive { get; set; }

    [StringLength(120, ErrorMessage = "Remarks can not be more than 120 characters")]
    public string Remarks { get; set; }

    //--------------------------------

    public DateTime ActionDate { get; set; }
    public long ActionById { get; set; }
}
