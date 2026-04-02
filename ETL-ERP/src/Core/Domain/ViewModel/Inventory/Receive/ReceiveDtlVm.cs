namespace Domain.ViewModel.Inventory.Receive;

public class ReceiveDtlVm
{
    public long Id { get; set; }
    public long SlNo { get; set; }
    public double ItemQty { get; set; }
    public double ActualRcvQty { get; set; }
    public double Stock { get; set; }
    public double OrderQty { get; set; }
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    public long TranMstId { get; set; }
    public long ItemId { get; set; }
    public string ItemName { get; set; }
    public long ItemUnitId { get; set; }
    public string ItemUnitName { get; set; }
    public long RcvItemUnitId { get; set; }
    public double? UnitPrice { get; set; }
}
