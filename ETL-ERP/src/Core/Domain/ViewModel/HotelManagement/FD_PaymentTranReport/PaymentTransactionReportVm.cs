using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.FD_PaymentTranReport;

public class PaymentTransactionReportVm
{
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public string BookingNo { get; set; }
    public long BookingId { get; set; }
    public DateTime BookingDate { get; set; }
    public string BillNumber { get; set; }
    public long? BillId { get; set; }
    public string RoomNoList { get; set; }
    public string Description { get; set; }

    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public string CompanyName { get; set; }

    public long PaymentId { get; set; }
    public double PaidAmount { get; set; }
    public DateTime PaidDate { get; set; }
    public string ReceivedBy { get; set; }
    public string TransactionNo { get; set; }
    public int? PayMode { get; set; }
    public int? PayType { get; set; }
   
    public string AuditRemarks { get; set; }
    public DateTime? AuditDate { get; set; }
    public long? AuditById { get; set; }
    public string AuditBy { get; set; }
    public bool IsDueCollection { get; set; }
    public bool ShowOnlyDue { get; set; }
    public PaymentStatusEnum PaymentStatus { get; set; }
    public IEnumerable<SelectListItem> PayModeLookUp { get; set; }

}
