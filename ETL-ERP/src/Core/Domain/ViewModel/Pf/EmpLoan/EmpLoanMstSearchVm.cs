using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Pf.EmpLoan
{
    public class EmpLoanMstSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public DateTime LoanPassDate { get; set; }
        public DateTime LoanPayDate { get; set; }
        public double LoanAmount { get; set; }
        public double InterestRate { get; set; }
        public DateTime FirstInsDate { get; set; } // First Installment Date
        public string LoanFileUrl { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public IEnumerable<SelectListItem> EmployeeLookup { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        public int InstalmentCount { get; set; }
    }
}
