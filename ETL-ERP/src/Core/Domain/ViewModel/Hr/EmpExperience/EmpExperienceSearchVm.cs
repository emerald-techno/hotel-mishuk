using Domain.ModelInterface;

namespace Domain.ViewModel.Hr.EmpExperience
{
    public class EmpExperienceSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string CompanyName { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Designation { get; set; }
        public string Responsibility { get; set; }
        public double LastDrawnSalary { get; set; }
        public string LeftReason { get; set; }
        public string ExperienceFile { get; set; }
        public int SlNo { get; set; }
        public string Remarks { get; set; }

        //--------------------------------------
        public long EmployeeId { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
