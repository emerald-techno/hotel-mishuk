using Domain.ModelInterface;

namespace Domain.ViewModel.Admin.Designation
{
    public class DesignationSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Remarks { get; set; }
        public bool IsTeacher { get; set; }

        //-------------------------------------
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
