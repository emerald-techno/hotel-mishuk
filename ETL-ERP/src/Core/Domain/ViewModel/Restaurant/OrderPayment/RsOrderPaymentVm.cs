using Domain.Enums.AppEnums;

namespace Domain.ViewModel.Restaurant.OrderPayment;

public class RsOrderPaymentVm
{
    public long Id { get; set; }
    public DateTime PaidDate { get; set; }
    public double PaidAmount { get; set; } = 0;
    public PayModeEnum PayMode { get; set; }
    public string TransactionNo { get; set; }
    public string ChequeNo { get; set; }
    public string AccountNo { get; set; }
    public string Remarks { get; set; }

    // --- Fk ---

    public long OrderId { get; set; }
    public string OrderNo { get; set; }
    public long? BankId { get; set; }
}

public class PayRsOrderVm
{
    public long OrderId { get; set; }
    public double PaidAmount { get; set; }
    public PayModeEnum PayMode { get; set; }
    public string TransactionNo { get; set; }
    public string ChequeNo { get; set; }
    public string AccountNo { get; set; }
    public string Description { get; set; }
    public string PaidDateStr { get; set; }
}
