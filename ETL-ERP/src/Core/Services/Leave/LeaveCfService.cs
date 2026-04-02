using AutoMapper;
using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveCf;
using Interface.Repository.Leave;
using Interface.Services.Leave;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Leave
{
    public class LeaveCfService : BaseService<LeaveCf>, ILeaveCfService
    {
        #region Config
        private readonly ILeaveCfRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public LeaveCfService(ILeaveCfRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<LeaveCfSearchVm, LeaveCfSearchVm>> SearchAsync(DataTablePagination<LeaveCfSearchVm, LeaveCfSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion

        #region CalculateDailyCf

        //public async Task<bool> CalculateDailyCf()
        //{
        //    var isUpdated = false;
        //    var currentYear = DateTime.Now.Year;
        //    var currenyMonth = DateTime.Now.Month;
        //    DateTime firstDay = new DateTime(DateTime.Now.Year, 1, 1);

        //    var empCfList = await Repository.GetAsync(c => c.LeaveYear == currentYear && !c.IsDeleted);

        //    var updateCfList = new List<LeaveCf>();

        //    if (empCfList.Count > 0)
        //    {
        //        var totalDays = DU.AppUtility.DaysDiffernce(DateTime.Today, firstDay);
        //        double perDayLeave = totalDays / 12;
        //        var leaveBalance = Math.Round(perDayLeave, MidpointRounding.ToEven);

        //        foreach (var cf in empCfList)
        //        {
        //            cf.LeaveBalance = (int)leaveBalance;

        //            updateCfList.Add(cf);
        //        }

        //        await Repository.UpdateRangeAsync(updateCfList);
        //        isUpdated = await _iUnitOfWork.CompleteAsync();

        //    }

        //    return isUpdated;
        //}


        public async Task<bool> SetupNewCf()
        {
            var result = await Repository.SetupNewYearCf();
            return result;
        }


        #endregion

    }
}
