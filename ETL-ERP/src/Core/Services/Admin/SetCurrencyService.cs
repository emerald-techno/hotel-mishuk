using AutoMapper;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.SetCurrency;
using Interface.Repository.Admin;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Admin
{
    public class SetCurrencyService : BaseService<SetCurrency>, ISetCurrencyService
    {
        #region Config
        private ISetCurrencyRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public SetCurrencyService(ISetCurrencyRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<SetCurrencySearchVm, SetCurrencySearchVm>> SearchAsync(DataTablePagination<SetCurrencySearchVm, SetCurrencySearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion
    }
}
