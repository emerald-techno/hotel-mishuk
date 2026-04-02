using Domain.ModelInterface;

namespace Domain.ViewModel.Inventory.CategoryInfo
{
    public class CategoryInfoSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryType { get; set; } //I=Item, S=Service
        public string CategoryTypeText => CategoryType switch { "I" => "Item", "S" => "Service", _ => "--" };
        public string Remarks { get; set; }
        public string PhotoDocUrl { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        public long? LedgerId { get; set; }
        public string LedgerName { get; set; }

        public string HeadCode { get; set; }
    }
}
