using AutoMapper;
using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveSetup;
using Interface.Repository.Leave;
using Interface.Services.Leave;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Leave
{
    public class LeaveSetupService : BaseService<LeaveSetup>, ILeaveSetupService
    {
        #region Config
        private readonly ILeaveSetupRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public LeaveSetupService(ILeaveSetupRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<LeaveSetupSearchVm, LeaveSetupSearchVm>> SearchAsync(DataTablePagination<LeaveSetupSearchVm, LeaveSetupSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion
    }
}
