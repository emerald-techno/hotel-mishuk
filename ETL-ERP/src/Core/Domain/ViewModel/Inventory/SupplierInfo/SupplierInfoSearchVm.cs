using Domain.ModelInterface;

namespace Domain.ViewModel.Inventory.SupplierInfo
{
    public class SupplierInfoSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhotoDoc { get; set; }
        public string Remarks { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
