using AutoMapper;
using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.DptLeaveReviewer;
using Interface.Repository.Leave;
using Interface.Services.Leave;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Leave
{
    public class DptLeaveReviewerService : BaseService<DptLeaveReviewer>, IDptLeaveReviewerService
    {
        #region Config
        private IDptLeaveReviewerRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public DptLeaveReviewerService(IDptLeaveReviewerRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<DptLeaveReviewerSearchVm, DptLeaveReviewerSearchVm>>
            SearchAsync(DataTablePagination<DptLeaveReviewerSearchVm, DptLeaveReviewerSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion
    }
}
