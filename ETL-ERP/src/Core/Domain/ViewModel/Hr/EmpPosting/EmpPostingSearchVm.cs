using Domain.ModelInterface;

namespace Domain.ViewModel.Hr.EmpPosting
{
    public class EmpPostingSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public DateTime? DateTo { get; set; }
        public string PostingType { get; set; }
        public double NetSalary { get; set; }
        public int? PreDepartmentId { get; set; }
        public int? PreDesignationId { get; set; }
        public double PreNetSalary { get; set; }
        public string PostingDoc { get; set; }
        public int SlNo { get; set; }
        public string Remarks { get; set; }


        //--------------------------------------------------
        public long EmployeeId { get; set; }
        public long? DepartmentId { get; set; }
        public long? DesignationId { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string PreDepartmentName { get; set; }
        public string PreDesignationName { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
