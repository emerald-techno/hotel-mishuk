using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Pf.PfSetting
{
    public class PfSettingVm
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Provident fund source required")]
        [DisplayName("Pf Source: ")]
        [StringLength(1)]
        public string PfSource { get; set; }
        public string PfSourceText => PfSource switch { "G" => "Gross", "B" => "Basic", _ => "__" };

        [DisplayName("Employee Contribution (0.35%): ")]
        public double EmpCon { get; set; } // Defult = 0.35

        [DisplayName("Company Contribution (0.35%): ")]
        public double CompCon { get; set; } // Defult = 0.35

        [DisplayName("Maturity Month: ")]
        public short MaturityMonth { get; set; } // Defult = 0

        [DisplayName("Loan After (12%): ")]
        public short LoanAfter { get; set; } // Defult = 12
        public IEnumerable<SelectListItem> PfSourceLookup { get; set; }

    }
}
