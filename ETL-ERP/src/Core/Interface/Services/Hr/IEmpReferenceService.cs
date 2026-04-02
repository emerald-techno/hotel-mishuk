using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpReference;
using Interface.Base;

namespace Interface.Services.Hr
{
    public interface IEmpReferenceService : IService<EmpReference>
    {
        Task<List<EmpReferenceVm>> GetReferenceByEmpId(long empId, short refType);
    }
}
