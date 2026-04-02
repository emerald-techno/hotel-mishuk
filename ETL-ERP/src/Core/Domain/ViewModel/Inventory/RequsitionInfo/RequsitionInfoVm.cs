using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Inventory.RequsitionInfo
{
    public class RequsitionInfoVm
    {
        public long Id { get; set; }

        [Required]
        public string ReqNo { get; set; }

        [Required]
        public string ReqDateStr { get; set; }
        public DateTime ReqDate { get; set; }

        public string Priority { get; set; } // N=Normal, H=High, A=Argent
        public string PriorityText => Priority switch { "N" => "Normal", "H" => "High", "A" => "Argent", _ => "--" };
        public string StatusText => Status switch { 0 => "FRESH", 1 => "REVIEW", 2 => "REJECTED", 3 => "APPROVED", _ => "--" };
        public string Remarks { get; set; }

        public short? Status { get; set; } // 0=FRESH, 1 = REVIEW, 2=REJECTED, 3 = APPROVED
        public short? IsStatus { get; set; }
        public string IsStatusText => IsStatus switch { 0 => "NOT", 1 => "PARTIAL", 2 => "FULL", 3 => "FOURCE_FULL", _ => "--" };
        public long? DeptId { get; set; }
        public string Departement { get; set; }
        public string RequsitionFor { get; set; }
        public long? ReqById { get; set; }
        public long? SubmitById { get; set; }
        public string SubmitByName { get; set; }
        public long? ActionById { get; set; }
        public DateTime? ActionDate { get; set; }

        public IEnumerable<SelectListItem> DepartmentLookUp { get; set; }
        public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
        public IEnumerable<SelectListItem> ItemLookUp { get; set; }
        public IEnumerable<SelectListItem> UnitLookUp { get; set; }
        public IEnumerable<SelectListItem> PriorityLookUp { get; set; }
        public virtual ICollection<RequsitionInfoDtlVm> RequsitionInfoDtls { get; set; }
    }
}
