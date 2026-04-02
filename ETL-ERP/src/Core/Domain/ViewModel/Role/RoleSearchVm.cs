using Domain.Enums;
using Domain.ModelInterface;

namespace Domain.ViewModel.Role;

public class RoleSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public ApplicationRoleStatusEnum? Status { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
