using AutoMapper;
using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpJournal;
using Interface.Repository.Hr;
using Interface.Services.Hr;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Hr
{
    public class EmpJournalService : BaseService<EmpJournal>, IEmpJournalService
    {
        #region Config

        private IEmpJournalRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public EmpJournalService(IEmpJournalRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }

        #endregion

        #region Search

        public async Task<List<EmpJournalVm>> GetEmpJournalByEmpId(long empId)
        {
            if (empId <= 0) return new List<EmpJournalVm>();
            var data = await Repository.GetAsync(c => c.EmployeeId == empId && !c.IsDeleted);
            var dataList = _iMapper.Map<List<EmpJournalVm>>(data);
            return dataList;
        }

        #endregion
    }
}
