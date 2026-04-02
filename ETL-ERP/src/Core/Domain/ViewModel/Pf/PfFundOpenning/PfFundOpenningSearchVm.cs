using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Pf.PfFundOpenning
{
    public class PfFundOpenningSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public DateTime OpenningDate { get; set; }
        public DateTime PfStartDate { get; set; }
        public double EmpCon { get; set; }
        public double CompCon { get; set; }
        public double Interest { get; set; }
        public double TotalAmount { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //---FK---

        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }

        public long ActionById { get; set; }
        public DateTime ActionDate { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        public IEnumerable<SelectListItem> EmployeeLookup { get; set; }
    }
}
