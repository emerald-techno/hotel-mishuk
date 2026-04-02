using Domain.Entities.Inventory;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Inventory.RequsitionInfo
{
    public class RequsitionInfoSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string ReqNo { get; set; }
        public DateTime ReqDate { get; set; }
        public string Priority { get; set; } // N=Normal, H=High, A=Argent
        public short Status { get; set; } // 0=FRESH, 1 = REVIEW, 2=REJECTED, 3 = APPROVED


        public string StatusText => Status switch { 0 => "FRESH", 1 => "REVIEW", 2 => "REJECTED", 3 => "APPROVED", _ => "--" };

        public short IsStatus { get; set; } // 0=NOT, 1=PARTIAL, 2=FULL, 3=FOURCE FULL
        public string IsStatusText => IsStatus switch { 0 => "NOT", 1 => "PARTIAL", 2 => "FULL", 3 => "FOURCE_FULL", _ => "--" };
        public string Remarks { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string SFromDate { get; set; }
        public string SToDate { get; set; }

        public long? DeptId { get; set; }
        public string DeptName { get; set; }
        public long? ReqById { get; set; }
        public string ReqByName { get; set; }
        public virtual ICollection<RequsitionInfoDtl> RequsitionInfoDtls { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }

        public IEnumerable<SelectListItem> RequsitionByLookUp { get; set; }
        public IEnumerable<SelectListItem> DepartmentLookUp { get; set; }
        public IEnumerable<SelectListItem> PriorityLookUp { get; set; }


    }
}
