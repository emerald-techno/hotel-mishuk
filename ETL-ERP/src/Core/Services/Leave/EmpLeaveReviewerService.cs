using AutoMapper;
using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.EmpLeaveReviewer;
using Interface.Repository.Leave;
using Interface.Services.Leave;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Leave
{
    public class EmpLeaveReviewerService : BaseService<EmpLeaveReviewer>, IEmpLeaveReviewerService
    {
        #region Config
        private IEmpLeaveReviewerRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public EmpLeaveReviewerService(IEmpLeaveReviewerRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<EmpLeaveReviewerSearchVm, EmpLeaveReviewerSearchVm>>
            SearchAsync(DataTablePagination<EmpLeaveReviewerSearchVm, EmpLeaveReviewerSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion
    }
}
