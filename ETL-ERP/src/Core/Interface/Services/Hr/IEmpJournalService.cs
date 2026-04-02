using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpJournal;
using Interface.Base;

namespace Interface.Services.Hr
{
    public interface IEmpJournalService : IService<EmpJournal>
    {
        Task<List<EmpJournalVm>> GetEmpJournalByEmpId(long empId);
    }
}
