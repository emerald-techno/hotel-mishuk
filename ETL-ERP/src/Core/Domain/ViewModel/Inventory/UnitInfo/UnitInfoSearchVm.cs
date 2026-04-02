using Domain.ModelInterface;

namespace Domain.ViewModel.Inventory.UnitInfo
{
    public class UnitInfoSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public string Remarks { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
