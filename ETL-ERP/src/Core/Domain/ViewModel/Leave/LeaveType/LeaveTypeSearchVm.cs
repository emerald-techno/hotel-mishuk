using Domain.ModelInterface;

namespace Domain.ViewModel.Leave.LeaveType
{
    public class LeaveTypeSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string TypeName { get; set; }
        public int Balance { get; set; }
        public string Desc { get; set; }
        public string Gender { get; set; }

        //--------------FK-----------------------
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
