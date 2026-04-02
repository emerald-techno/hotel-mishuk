
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.QueryModel
{
    public class UserQuery : IQueryObject
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long? UserRoleId { get; set; }
        public IEnumerable<SelectListItem> UserRoles { get; set; }
        public long? RoleId { get; set; }
        public UserQuery()
        {
            UserRoles = new List<SelectListItem>();
        }
    }
}


