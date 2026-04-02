using AutoMapper;
using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpDisciplinary;
using Interface.Repository.Hr;
using Interface.Services.Hr;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Hr
{
    public class EmpDisciplinaryService : BaseService<EmpDisciplinary>, IEmpDisciplinaryService
    {
        #region Config

        private IEmpDisciplinaryRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public EmpDisciplinaryService(IEmpDisciplinaryRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }

        #endregion

        #region Search

        public async Task<List<EmpDisciplinaryVm>> GetEmpDisciplinaryByEmpId(long empId)
        {
            if (empId <= 0) return new List<EmpDisciplinaryVm>();
            var data = await Repository.GetAsync(c => c.EmployeeId == empId && !c.IsDeleted);
            var dataList = _iMapper.Map<List<EmpDisciplinaryVm>>(data);
            return dataList;
        }

        #endregion
    }
}
