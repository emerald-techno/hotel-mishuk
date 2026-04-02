using Domain.ModelInterface;

namespace Domain.ViewModel.Hr.EmpReference
{
    public class EmpReferenceSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string RefName { get; set; }
        public int RefType { get; set; }
        public string Occupation { get; set; }
        public DateTime? Dob { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Relation { get; set; }
        public int SlNo { get; set; }
        public double OwnerShip { get; set; }
        public string Remarks { get; set; }
        public long EmployeeId { get; set; }
        //-----------------------------------------------------
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
