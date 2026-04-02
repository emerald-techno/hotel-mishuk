using Domain.ModelInterface;

namespace Domain.ViewModel.Hr.EmpEducation
{
    public class EmpEducationSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string CourseName { get; set; }
        public string InstitutionName { get; set; }
        public string BoardUniversity { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public int? PassingYear { get; set; }
        public string SubjectGroup { get; set; }
        public string Result { get; set; }
        public string CertificateFile { get; set; }
        public int SlNo { get; set; }
        public string Remarks { get; set; }

        //------------------------------------------
        public long EmployeeId { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
