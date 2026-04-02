using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpPosting;
using Interface.Base;

namespace Interface.Repository.Hr
{
    public interface IEmpPostingRepository : IRepository<EmpPosting>
    {
        Task<List<EmpPostingVm>> GetPostingByEmpId(long empId);
    }
}
