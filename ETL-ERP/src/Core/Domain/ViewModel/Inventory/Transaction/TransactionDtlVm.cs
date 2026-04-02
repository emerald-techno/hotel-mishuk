namespace Domain.ViewModel.Inventory.Transaction;

public class TransactionDtlVm
{
    public long Id { get; set; }
    public long SlNo { get; set; }
    public double ItemQty { get; set; } = 0;
    public double? ActualRcvQty { get; set; }
    public double Stock { get; set; } = 0;
    public double? OrderQty { get; set; }
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    public long TranMstId { get; set; }
    public long ItemId { get; set; }
    public string ItemName { get; set; }
    public long ItemUnitId { get; set; }
    public string ItemUnitName { get; set; }
    public long? RcvItemUnitId { get; set; }
    public string RcvItemUnitName { get; set; }
    public double? UnitPrice { get; set; }

    public long? CategoryId { get; set; }
    public string CategoryName { get; set; }
    public long? LedgerId { get; set; } = 0;
    public string LedgerName { get; set; }

    public string HeadCode { get; set; }

}
