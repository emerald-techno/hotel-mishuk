using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpExperience;
using Interface.Base;

namespace Interface.Services.Hr
{
    public interface IEmpExperienceService : IService<EmpExperience>
    {
        Task<List<EmpExperienceVm>> GetEmpExperienceByEmpId(long empId);
    }
}
