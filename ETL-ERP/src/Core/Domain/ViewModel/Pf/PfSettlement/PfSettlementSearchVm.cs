using Domain.Entities.HR;

namespace Domain.ViewModel.Pf.PfSettlement
{
    public class PfSettlementSearchVm
    {
        public long Id { get; set; }
        public DateTime SettlementDate { get; set; }
        public string SettlementDateStr { get; set; }
        public DateTime? PfStartDate { get; set; }
        public string PfStartDateStr { get; set; }
        public DateTime? PfEndDate { get; set; }
        public string PfEndDateStr { get; set; }
        public string Status { get; set; }
        public string StatusText => Status switch { "L" => "LEFT", "T" => "TRANSFER", "C" => "CANCEL", _ => "TRANSFER" };
        public DateTime StatusDate { get; set; }
        public string StatusDateStr { get; set; }
        public double EmpCon { get; set; }
        public double CompCon { get; set; }
        public double Interest { get; set; }
        public double TotalAmount { get; set; }
        public double Tax { get; set; }
        public double Vat { get; set; }
        public double NetAmount { get; set; }
        public string FileDocUrl { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string ApproveDateStr { get; set; }
        public string Remarks { get; set; }
        public short PfLength { get; set; }
        public string CompConType { get; set; }
        public string CompConTypeText => CompConType switch { "N" => "Not Contribute", "H" => "Half", "F" => "Full", _ => "---Select---" };

        //-----------------------------------------
        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public string EmployeeName { get; set; }
        public bool IsDeleted { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
