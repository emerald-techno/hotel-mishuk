using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Domain.ViewModel.Inventory.Transaction;

public class TransactionSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public int SerialNo { get; set; }
    public string TranNo { get; set; }
    public DateTime TranDate { get; set; }
    public string TranFileUrl { get; set; }
    public DateTime? QcDate { get; set; }
    public string QcDesc { get; set; }
    public string QcFileUrl { get; set; }
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? FromDate { get; set; }

    [DisplayName("From Date")]
    public string SFromDate { get; set; }
    public DateTime? ToDate { get; set; }

    [DisplayName("To Date")]
    public string SToDate { get; set; }    
    public string TranType { get; set; }
    public long? RefTranId { get; set; }
    
    
    public long TranById { get; set; }
    
    public long? QcById { get; set; }
    
    

   
    public IEnumerable<SelectListItem> SupplierLookUp { get; set; }
    public IEnumerable<SelectListItem> OrderLookUp { get; set; }
    public IEnumerable<SelectListItem> ItemLookUp { get; set; }
    public IEnumerable<SelectListItem> IssueDptLookup { get; set; }
    public IEnumerable<SelectListItem> IssueEmpLookup { get; set; }
    public IEnumerable<SelectListItem> RoomLookup { get; set; }
    public IEnumerable<SelectListItem> ReqLookup { get; set; }
    public long? OrderId { get; set; }
    public string OrderNo { get; set; }
    public DateTime? OrderDate { get; set; }
    public long? ReqMstId { get; set; }
    public string ReqNo { get; set; }
    public DateTime? ReqDate { get; set; }
    public long? SupplierId { get; set; }
    public string SupplierName { get; set; }
    public long? IssueDeptId { get; set; }
    public string IssueDeptName { get; set; }
    public long? IssueEmpId { get; set; }
    public string IssueEmpName { get; set; }
    public long? IssueRoomId { get; set; }
    public string IssueRoomNo { get; set; }
    public long ActionById { get; set; }
    public string ActionByName { get; set; }
    public long? UpdatedById { get; set; }
    public string UpdatedByName { get; set; }
    public double Rate { get; set; }

    public long UserId { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
