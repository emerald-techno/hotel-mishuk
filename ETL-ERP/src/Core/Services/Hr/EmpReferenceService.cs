using AutoMapper;
using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpReference;
using Interface.Repository.Hr;
using Interface.Services.Hr;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Hr
{
    public class EmpReferenceService : BaseService<EmpReference>, IEmpReferenceService
    {
        #region Config

        private IEmpReferenceRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public EmpReferenceService(IEmpReferenceRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }

        #endregion

        #region GetReferenceByEmpId

        public async Task<List<EmpReferenceVm>> GetReferenceByEmpId(long empId, short refType)
        {
            if (empId <= 0) return new List<EmpReferenceVm>();
            var data = await Repository.GetAsync(c => c.EmployeeId == empId && !c.IsDeleted);
            var dataList = _iMapper.Map<List<EmpReferenceVm>>(data);
            if (refType > 0) dataList = dataList.Where(c => c.RefType == refType).ToList();
            return dataList;
        }

        #endregion
    }
}
