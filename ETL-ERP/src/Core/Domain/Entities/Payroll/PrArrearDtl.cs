using System.ComponentModel.DataAnnotations;
using Domain.Entities.HR;

namespace Domain.Entities.Payroll
{
    public class PrArrearDtl
    {
        public long Id { get; set; }
        public double Amount { get; set; }

        [StringLength(1)]
        public string ArrearFor { get; set; } //N=Normal,S=SALARY,L=LEAVE
        public string Remarks { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public bool IsDeleted { get; set; }
        public long ArrearId { get; set; }
        public PrArrearMst Arrear { get; set; }
        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public long? SalaryId { get; set; }
        public PrSalaryDtl Salary { get; set; }
    }
}
