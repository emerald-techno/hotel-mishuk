using System.ComponentModel.DataAnnotations;
using Domain.Entities.Identity;

namespace Domain.Entities.Pf
{
    public class PfSetting
    {
        public long Id { get; set; }

        [Required]
        [StringLength(1)]
        public string PfSource { get; set; }
        public double EmpCon { get; set; } // Defult = 0.35
        public double CompCon { get; set; } // Defult = 0.35
        public short MaturityMonth { get; set; } // Defult = 0
        public short LoanAfter { get; set; } // Defult = 12
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }

    }
}
