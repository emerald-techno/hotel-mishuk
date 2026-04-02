using AutoMapper;
using Domain.Entities.Identity;
using Domain.ViewModel.User;
using WebMVC.Models.User;
using WebMVC.Models.UserRole;

namespace WebMVC.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region Config

        CreateMap<SaveUserModel, ApplicationUser>();
        CreateMap<ApplicationUser, SaveUserModel>()
            .ForMember(dest => dest.UserRoleId, opt => opt.MapFrom(s => s.UserRoles.Any() ? s.UserRoles.FirstOrDefault().RoleId : 0))
            .ForMember(dest => dest.UserRoleName, opt => opt.MapFrom(src => src.UserRoles.Any() ? src.UserRoles.FirstOrDefault().Role.Name : string.Empty));

        CreateMap<ApplicationUser, UserModel>()
            .ForMember(dest => dest.UserRoleName, opt => opt.MapFrom(src =>
            src.UserRoles.Any() ? (src.UserRoles.Select(ur => ur.Role).FirstOrDefault() != null ?
            src.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault() : null) : null));

        CreateMap<SaveUserRoleModel, ApplicationRole>();
        CreateMap<ApplicationRole, SaveUserRoleModel>();

        CreateMap<ApplicationRole, UserRoleModel>();

        CreateMap<ApplicationUser, UserSearchVm>();

        #endregion
    }
}
