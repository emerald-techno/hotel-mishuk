using AutoMapper;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.FinancialYear;
using Interface.Repository.Admin;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Admin
{
    public class SetFincYearService : BaseService<SetFincYear>, ISetFincYearService
    {
        #region Config
        private ISetFincYearRepository _iRepository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public SetFincYearService(ISetFincYearRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
            : base(iRepository, iUnitOfWork)
        {
            _iRepository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion

        #region GetFincYearByDate

        public async Task<SetFincYear> GetFincYearByDate(DateTime queryDate)
        {
            var data = await _iRepository.GetFirstOrDefaultAsync(c => queryDate >= c.YearStartDate && queryDate <= c.YearEndDate);
            return data;
        }

        #endregion

        #region GetFinancialYearByDate

        public SetFincYear GetFinancialYearByDate(DateTime queryDate)
        {
            var data = _iRepository.GetFirstOrDefault(c => queryDate >= c.YearStartDate && queryDate <= c.YearEndDate);
            return data;

        }
        #endregion

        #region GetFinancialYearById

        public async Task<SetFincYear> GetFinancialYearById(long fincYearId)
        {
            var data = await _iRepository.GetFirstOrDefaultAsync(c => c.Id == fincYearId && !c.IsDeleted);
            return data;

        }

        #endregion

        #region Search
        public async Task<DataTablePagination<SetFincYearSearchVm, SetFincYearSearchVm>> SearchAsync(DataTablePagination<SetFincYearSearchVm, SetFincYearSearchVm> model)
        {
            var dataList = await _iRepository.SearchAsync(model);
            return dataList;
        }
        #endregion
    }
}
