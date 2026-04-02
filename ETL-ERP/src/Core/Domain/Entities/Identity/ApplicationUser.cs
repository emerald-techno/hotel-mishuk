using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<long>
    {

        [Required]
        public string FullName { get; set; }

        [StringLength(30)]
        public string Mobile { get; set; }

        [StringLength(120)]
        public string PhotoUrl { get; set; }

        [StringLength(50)]
        public string ActivationCode { get; set; }

        public bool IsMaskEmail { get; set; }

        //-------------------

        public short UserLevelId { get; set; }

        [NotMapped]
        public string LayoutName { get; set; }

        [NotMapped]
        public string ProfileUrl { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public List<long> RoleIds { get; set; }

        [NotMapped]
        public long CurrentRoleId { get; set; }

        [StringLength(1)]
        public string Gender { get; set; }
        public string LastPassword { get; set; }
        public DateTime? LastPassChangeDate { get; set; }
        public int PasswordChangedCount { get; set; }
        public ApplicationUserStatusEnum Status { get; set; }
        public DateTime ActionDate { get; set; }
        public long? ActionById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }

        [ForeignKey("ActionById")]
        public virtual ApplicationUser ActionBy { get; set; }

        [ForeignKey("UpdatedById")]
        public virtual ApplicationUser UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public IList<ApplicationUserRole> UserRoles { get; set; }

        public ApplicationUser() : base()
        {
            IsActive = true;
            IsDeleted = false;
            UserRoles = new List<ApplicationUserRole>();
        }

        public static implicit operator ApplicationUser(long v)
        {
            throw new NotImplementedException();
        }
    }
}
