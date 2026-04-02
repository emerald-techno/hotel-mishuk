using Domain.ModelInterface;

namespace Domain.ViewModel.Admin.Division
{
    public class SetDivisionSearchVm : IDataTableSearch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameBn { get; set; }
        public int Code { get; set; }

        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        long IDataTableSearch.Id { get; set; }
    }
}
