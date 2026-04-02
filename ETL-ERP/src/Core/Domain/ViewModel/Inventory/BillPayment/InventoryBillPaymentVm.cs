using Domain.Enums.AppEnums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Inventory.BillPayment;

public class InventoryBillPaymentVm
{
    public long Id { get; set; }
    public string BillNo { get; set; }
    public DateTime BillDate { get; set; }
    public double BillAmount { get; set; }
    public PayModeEnum PayMode { get; set; }  //C=Cash, H=Cheque
    public string BillFileUrl { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string ApprovedRemarks { get; set; }
    public bool IsAdvance { get; set; }
    public string Remarks { get; set; }
    public long OrderMstId { get; set; }
    public string OrderMstNo { get; set; }
    public long BillById { get; set; }
    public string BillByName { get; set; }
    public long? SupplierId { get; set; }
    public string SupplierName { get; set; }
    public long? ApprovedById { get; set; }
    public string ApprovedByName { get; set; }
    public IEnumerable<SelectListItem> PayModeLookUp { get; set; }
    public IEnumerable<SelectListItem> OrderLookUp { get; set; }
}

public class InvBillSaveVm
{
    public long OrderMstId { get; set; }
    public string OrderMstNo { get; set; }
    public DateTime OrderDate { get; set; }
    public long SupplierId { get; set; }
    public string SupplierName { get; set; }
    public string SupplierMobile { get; set; }
    public string SupplierAddress { get; set; }
    public double TotalAmount { get; set; }
    public double PaidAmount { get; set; }
    public double DueAmount { get; set; }
    public ICollection<BillSaveReceiveVm> BillSaveReceiveVms { get; set; }
}

public class BillSaveReceiveVm
{
    public long RcvId { get; set; }
    public string RcvTranNo { get; set; }
    public DateTime RcvTranDate { get; set; }
    public double RcvAmount { get; set; }
}

public class InventoryBillPaymentDetails
{
    public string OrderNo { get; set; }
    public string OrderDateStr { get; set; }
    public string Status { get; set; } // 0=FRESH, 1 = REVIEW, 2=REJECTED, 3 = APPROVED
    public string ReceiveStatus { get; set; } // 0=NOT, 1=PARTIAL, 2=FULL, 3=FOURCE FULL
    public string SupplierName { get; set; }
    public string SupplierCode { get; set; }
    public string Mobile { get; set; }
    public string BillByName { get; set; } //
    public string BillNo { get; set; } //
    public string BillDateStr { get; set; } //
    public double BillAmount { get; set; }
    public string PayMode { get; set; }  //C=Cash, H=Cheque
}