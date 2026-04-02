using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpDisciplinary;
using Interface.Base;

namespace Interface.Services.Hr
{
    public interface IEmpDisciplinaryService : IService<EmpDisciplinary>
    {
        Task<List<EmpDisciplinaryVm>> GetEmpDisciplinaryByEmpId(long empId);
    }
}
