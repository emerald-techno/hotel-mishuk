using Domain.Entities.HR;

namespace Domain.Entities.Payroll
{
    public class PrGuestSalaryDtl
    {
        public long Id { get; set; }
        public double Amount { get; set; }
        public string Remarks { get; set; }
        public bool IsDeleted { get; set; }
        public long GuestSalaryMstId { get; set; }
        public PrGuestSalaryMst GuestSalaryMst { get; set; }
        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
