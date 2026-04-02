using Domain.ModelInterface;

namespace Domain.ViewModel.Admin.Country
{
    public class SetCountrySearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string AbbrName { get; set; }
        public string Code { get; set; }
        public bool IsDeleted { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
