using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Leave.LeaveType
{
    public class LeaveTypeVm
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Type Name required")]
        [DisplayName("Type Name: ")]
        public string TypeName { get; set; }
        [DisplayName("Balance: ")]
        public int Balance { get; set; }
        [StringLength(120, ErrorMessage = "Description can not be more than 120 characters")]
        [DisplayName("Description: ")]
        public string Desc { get; set; }
        [Required]
        [StringLength(1)]
        [DisplayName("Gender: ")]
        public string Gender { get; set; }
        public IEnumerable<SelectListItem> GenderLookUp { get; set; }

        //-------------------------FK---------------------
        public long ActionById { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
