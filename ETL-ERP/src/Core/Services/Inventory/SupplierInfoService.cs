using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.SupplierInfo;
using Interface.Repository.Common;
using Interface.Repository.Inventory;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Inventory
{
    public class SupplierInfoService : BaseService<SupplierInfo>, ISupplierInfoService
    {

        #region Config
        private ISupplierInfoRepository Repository { get; }
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IAutoCodeRepository _iAutoCodeRepository;

        public SupplierInfoService(ISupplierInfoRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IAutoCodeRepository iAutoCodeRepository) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
            _iAutoCodeRepository = iAutoCodeRepository;
        }

        #endregion

        public async Task<DataTablePagination<SupplierInfoSearchVm, SupplierInfoSearchVm>>
            SearchAsync(DataTablePagination<SupplierInfoSearchVm, SupplierInfoSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        public async Task<string> GetSupplierInfoCode()
        {
            var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.SupplierInfos.ToString(), "SupplierCode", "SUP", 6);
            return data;
        }
    }
}
