using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Payroll
{
    public class MonthlyAttendanceSheetMst : IAuditable
    {
        public long Id { get; set; }
        public short Year { get; set; }
        public short Month { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //-----------------------------------------
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<MonthlyAttendanceSheetDtl> MonthlyAttendanceSheetDtls { get; set; }
    }
}
