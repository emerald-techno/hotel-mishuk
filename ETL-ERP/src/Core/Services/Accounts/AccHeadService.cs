using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccHead;
using Interface.Repository.Accounts;
using Interface.Repository.Common;
using Interface.Services.Accounts;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Accounts
{
    public class AccHeadService : BaseService<AccHead>, IAccHeadService
    {
        #region Config
        private IAccHeadRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IAutoCodeRepository _iAutoCodeRepository;

        public AccHeadService(IAccHeadRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IAutoCodeRepository iAutoCodeRepository)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
            _iAutoCodeRepository = iAutoCodeRepository;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<AccHeadSearchVm, AccHeadSearchVm>> SearchAsync(DataTablePagination<AccHeadSearchVm, AccHeadSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion

        #region GetAccHeadCode

        public async Task<string> GetAccHeadCodeAsync(long groupId, long? parentId)
        {
            var data = await _iAutoCodeRepository.GetAccHeadCodeAsync(groupId, parentId);
            return data;
        }

        #endregion

        #region GetAccHeadGroupAutoCode

        public async Task<long> GetAccHeadGroupAutoCode(long groupId, long? parentId)
        {
            var data = await _iAutoCodeRepository.GetAccHeadGroupCode(groupId, parentId);
            return data;
        }

        #endregion

        #region GetAccHeadMaxAutoCode

        public async Task<string> GetAccHeadMaxAutoCode()
        {
            var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.AccHeads.ToString(), "HeadCode", "HEAD", 5);
            return data;
        }

        #endregion
    }
}
