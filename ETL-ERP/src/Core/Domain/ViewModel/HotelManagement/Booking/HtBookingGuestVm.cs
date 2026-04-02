using Domain.Enums.AppEnums;

namespace Domain.ViewModel.HotelManagement.RoomBooking;

public class HtBookingGuestVm
{
    public long Id { get; set; }
    public long GuestId { get; set; }
    public string GuestName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string CompanyName { get; set; }
    public long? CompanyId { get; set; }
    public string Country { get; set; }
    public long? DistrictId { get; set; }
    public string District { get; set; }
    public long? CountryId { get; set; }
    public DateTime? Dob { get; set; }
    public string Gender { get; set; }
    public string IdentityNo { get; set; }
    public IdentityTypeEnum? IdentityType { get; set; }
    public string GuestMobile { get; set; }
    public bool IsMain { get; set; }
    public bool IsVip { get; set; }
}
