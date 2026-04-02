using AutoMapper;
using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpEducation;
using Interface.Repository.Hr;
using Interface.Services.Hr;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Hr
{
    public class EmpEducationService : BaseService<EmpEducation>, IEmpEducationService
    {
        #region Config

        private IEmpEducationRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public EmpEducationService(IEmpEducationRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }

        #endregion

        #region Search

        public async Task<List<EmpEducationVm>> GetEmpEducationByEmpId(long empId)
        {
            if (empId <= 0) return new List<EmpEducationVm>();
            var data = await Repository.GetAsync(c => c.EmployeeId == empId && !c.IsDeleted);
            var dataList = _iMapper.Map<List<EmpEducationVm>>(data);
            return dataList;
        }

        #endregion
    }
}
