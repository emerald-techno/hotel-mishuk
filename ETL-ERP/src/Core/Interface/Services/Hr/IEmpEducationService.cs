using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpEducation;
using Interface.Base;

namespace Interface.Services.Hr
{
    public interface IEmpEducationService : IService<EmpEducation>
    {
        Task<List<EmpEducationVm>> GetEmpEducationByEmpId(long empId);
    }
}
