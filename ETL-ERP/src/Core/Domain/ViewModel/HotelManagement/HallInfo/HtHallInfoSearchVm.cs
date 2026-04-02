using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.RoomInfo;

public class HtHallInfoSearchVm : IDataTableSearch
{
    public long Id { get; set; }

    [Required]
    [StringLength(25)]
    public string HallName { get; set; }

    [StringLength(500)]
    public string HallInformation { get; set; }

    [StringLength(150)]
    public string OtherInfo { get; set; }
    public double Rent { get; set; }
    public double Vat { get; set; }
    public double ServiceCharge { get; set; }
    public double TotalRent { get; set; }

    [StringLength(120)]
    public string Remakrs { get; set; }

    [StringLength(120)]
    public string PhotoUrl { get; set; }
    public bool IsActive { get; set; }

    // -- DataTable Propertry

    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }


}
