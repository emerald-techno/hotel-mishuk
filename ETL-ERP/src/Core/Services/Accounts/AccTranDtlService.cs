using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccTranDtl;
using Interface.Repository.Accounts;
using Interface.Services.Accounts;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Accounts
{
    public class AccTranDtlService : BaseService<AccTranDtl>, IAccTranDtlService
    {
        #region Config

        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;
        private IAccTranDtlRepository Repository;

        public AccTranDtlService(IAccTranDtlRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }

        #endregion

        public async Task<DataTablePagination<AccTranDtlSearchVm, AccTranDtlSearchVm>>
                                        SearchAsync(DataTablePagination<AccTranDtlSearchVm, AccTranDtlSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }
    }
}
