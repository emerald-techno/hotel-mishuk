using AutoMapper;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.Designation;
using Interface.Repository.Admin;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Admin
{
    public class DesignationService : BaseService<Designation>, IDesignationService
    {
        #region Config
        private IDesignationRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public DesignationService(IDesignationRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<DesignationSearchVm, DesignationSearchVm>> SearchAsync(DataTablePagination<DesignationSearchVm, DesignationSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion
    }
}
