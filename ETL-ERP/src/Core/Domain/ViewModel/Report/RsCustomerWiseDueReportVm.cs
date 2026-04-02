using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Report;

public class RsCustomerWiseDueReportVm
{
    public string CustomerName { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public string CustomerType { get; set; }
    public short CustomerTypeId { get; set; }

    public decimal OrderAmount { get; set; }
    public decimal Vat { get; set; }
    public decimal Tax { get; set; }
    public decimal ServiceCharge { get; set; }
    public decimal Discount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public decimal NetOrderAmount { get; set; }
    public string StrQueryDate { get; set; }
    public long CustomerId { get; set; }
    public string GroupBy { get; set; }

    public long OrderId { get; set; }
    public string OrderNo { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderDateStr { get; set; }
    public bool ShowWithoutEmployee { get; set; }
    public IEnumerable<SelectListItem> CustomerLookUp { get; set; }
    public IEnumerable<SelectListItem> CustomerTypeLookUp { get; set; }
    public IEnumerable<SelectListItem> GroupByList { get; set; }
}
