using Domain.ModelInterface;

namespace Domain.ViewModel.Attendance.MonthlyAttSheet
{
    public class MonthlyAttSheetSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public short Year { get; set; }
        public short Month { get; set; }
        public DateTime DateForm { get; set; }
        public DateTime DateTo { get; set; }
        public string Remarks { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public short TotalDay { get; set; }
        public short Holiday { get; set; }
        public short OffDay { get; set; }

        // ------------FK---------------
        public long ActionById { get; set; }
        public long? UpdatedById { get; set; }

        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
