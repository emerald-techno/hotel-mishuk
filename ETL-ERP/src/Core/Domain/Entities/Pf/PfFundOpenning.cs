using System.ComponentModel.DataAnnotations;
using Domain.Entities.HR;
using Domain.Entities.Identity;

namespace Domain.Entities.Pf
{
    public class PfFundOpenning
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
        public Employee Employee { get; set; }

        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }

        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
