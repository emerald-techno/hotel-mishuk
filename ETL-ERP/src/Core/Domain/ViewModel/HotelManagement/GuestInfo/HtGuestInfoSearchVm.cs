using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.GuestInfo;

public class HtGuestInfoSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string GuestCode { get; set; }
    public string Salutation { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string Occupation { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public long? CompanyId { get; set; }
    public string CompanyName { get; set; }
    public long CountryId { get; set; }
    public string CountryName { get; set; }
    public long? DistrictId { get; set; }
    public string DistrictName { get; set; }
    public DateTime? Dob { get; set; }
    public IdentityTypeEnum? IdentityType { get; set; }
    public string IdentityNo { get; set; }
    public string IdentityPhotoUrl { get; set; }
    public string PhotoUrl { get; set; }
    public bool IsVip { get; set; }
    public string Note { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }

    public IEnumerable<SelectListItem> SalutationLookUp { get; set; }
    public IEnumerable<SelectListItem> GenderLookUp { get; set; }
    public IEnumerable<SelectListItem> IdentityTypeLookUp { get; set; }
    public IEnumerable<SelectListItem> CountryLookUp { get; set; }
    public IEnumerable<SelectListItem> DistrictLookUp { get; set; }
}
