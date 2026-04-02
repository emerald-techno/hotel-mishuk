using Domain.ViewModel.Pf.PfFund;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repository.Pf;

public interface IPfReportRepository
{
    Task<List<PfFundSchduelVm>> GetPfScheduleReportData(PfFundMstVm vm);
    Task<string> GetPfScheduleReportHtml(PfFundMstVm vm);
}
