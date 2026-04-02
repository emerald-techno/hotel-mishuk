using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Payroll
{
    public class PrArrearMst : IAuditable
    {
        public long Id { get; set; }
        public short Year { get; set; }
        public short Month { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedDate { get; set; }

        [StringLength(120)]
        public string ApprovedRemarks { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //-----------------------------------------
        public long? ApprovedById { get; set; }
        public ApplicationUser ApprovedBy { get; set; }
        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<PrArrearDtl> PrArrearDtls { get; set; }

    }
}
