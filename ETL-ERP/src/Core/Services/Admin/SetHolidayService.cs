using AutoMapper;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.SetHoliday;
using Interface.Repository.Admin;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Admin
{
    public class SetHolidayService : BaseService<SetHoliday>, ISetHolidayService
    {
        #region Config
        private ISetHolidayRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public SetHolidayService(ISetHolidayRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<SetHolidaySearchVm, SetHolidaySearchVm>> SearchAsync(DataTablePagination<SetHolidaySearchVm, SetHolidaySearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion
    }
}
