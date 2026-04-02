using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpPosting;
using Interface.Base;

namespace Interface.Services.Hr
{
    public interface IEmpPostingService : IService<EmpPosting>
    {
        Task<List<EmpPostingVm>> GetPostingByEmpId(long empId);
    }
}
