using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Attendance.MonthlyAttSheet
{
    public class MonthlyAttSheetMstVm
    {
        public long Id { get; set; }
        public short Year { get; set; }
        public short Month { get; set; }
        public DateTime DateForm { get; set; }
        public DateTime DateTo { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        public ICollection<MonthlyAttSheetDtlVm> MonthlyAttendanceSheetDtls { get; set; }
    }
}
