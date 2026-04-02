using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Pf
{
    public class PfSettlement : IAuditable
    {
        public long Id { get; set; }
        public DateTime SettlementDate { get; set; }
        public DateTime? PfStartDate { get; set; }
        public DateTime? PfEndDate { get; set; }

        [StringLength(1)]
        public string Status { get; set; } // L=LEFT, T =TRANSFER, C=CANCEL // Default = T
        public DateTime StatusDate { get; set; } //Transfer/Left Date
        public double EmpCon { get; set; }
        public double CompCon { get; set; }
        public double Interest { get; set; }
        public double TotalAmount { get; set; }
        public double Tax { get; set; }
        public double Vat { get; set; }
        public double NetAmount { get; set; }
        public string FileDocUrl { get; set; }
        public DateTime? ApproveDate { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }
        public short PfLength { get; set; } // In month

        [StringLength(1)]
        public string CompConType { get; set; } // N=Not Contribute ,H=Half ,F=Full

        //-----------------------------------------

        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
