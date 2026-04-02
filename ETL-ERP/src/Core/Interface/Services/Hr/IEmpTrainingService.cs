using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpTraining;
using Interface.Base;

namespace Interface.Services.Hr
{
    public interface IEmpTrainingService : IService<EmpTraining>
    {
        Task<List<EmpTrainingVm>> GetTrainingByEmpId(long empId);

    }
}
