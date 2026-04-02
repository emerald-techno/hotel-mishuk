using Domain.Enums.AppEnums;

namespace Domain.ViewModel.HotelManagement.Billing;

public class PayBillVm
{
    public long BillId { get; set; }
    public double PaidAmount { get; set; }  
    public PayModeEnum PayMode { get; set; }
    public string TransactionNo { get; set; }
    public string ChequeNo { get; set; }
    public string AccountNo { get; set; }
    public string Description { get; set; }
    public string PaidDateStr { get; set; }
}

public class MakeComplimentaryVm
{
    public long BillId { get; set; }
    public string Remarks { get; set; }
    public bool AllService { get; set; }
    public ICollection<long> ServiceIds { get; set; }
}