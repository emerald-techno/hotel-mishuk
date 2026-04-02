using Domain.Enums;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.User;

public class UserSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public long? UserRoleId { get; set; }
    public string Mobile { get; set; }
    public string Gender { get; set; }
    public string PhotoUrl { get; set; }
    public string UserRoleName { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public ApplicationUserStatusEnum? Status { get; set; }
    public long? RoleId { get; set; }
    public IEnumerable<SelectListItem> UserRoleLookup { get; set; }


    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
