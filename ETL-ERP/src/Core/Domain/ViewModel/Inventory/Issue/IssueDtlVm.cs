namespace Domain.ViewModel.Inventory.Issue;

public class IssueDtlVm
{
    public long ItemId { get; set; }
    public string ItemName { get; set; }
    public long ItemUnitId { get; set; }
    public string ItemUnitName { get; set; }
    public double ApproveQty { get; set; }
    public double IssueQty { get; set; }
    public double AlreadyIssuedQty { get; set; }
    public double Stock { get; set; }
    public double UnitPrice { get; set; }
}
