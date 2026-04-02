
using AutoMapper;
using Domain.Entities.Attendance;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.ShiftManagement;
using Interface.Repository.Attendance;
using Interface.Services.Attendance;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.Attendance;

public class ShiftManagementService : BaseService<ShiftManagement>, IShiftManagementService
{
    #region Config
    private IShiftManagementRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public ShiftManagementService(IShiftManagementRepository repository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(repository, iUnitOfWork)
    {
        _iRepository = repository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }
    #endregion

    #region AddOrUpdate
    public async Task<bool> AddOrUpdate(ShiftManagementVm vm)
    {
        if (vm?.ShiftManagementVms == null || vm.ShiftManagementVms.Count <= 0) throw new Exception("Sorry! No rooms found to assign!");

        List<ShiftManagement> addableList = null;
        List<ShiftManagement> updateableList = null;
        List<ShiftManagement> deletableList = null;

        if (vm?.ShiftManagementVms?.Count > 0)
        {
            var dataListForAdd = vm?.ShiftManagementVms?.Where(c => c.Id == 0).ToList();

            var updatableItemIds = vm?.ShiftManagementVms?.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            updateableList = (await _iRepository.GetAsync(c => updatableItemIds.Contains(c.Id))).ToList();

            if (updateableList?.Count > 0)
            {
                foreach (var updateRoom in updateableList)
                {
                    var filterData = vm.ShiftManagementVms.Where(c => c.Id == updateRoom.Id).FirstOrDefault();

                    updateRoom.UpdateDate = DateTime.Now;
                    updateRoom.UpdatedById = CurrentUserId;
                }

            }

            var oldIds = updateableList?.Select(c => c.Id).ToList();        

            if(vm.EmployeeId > 0)
            {
                deletableList = (await _iRepository.GetAsync(c => c.Month.Year == vm.SelectYear && c.Month.Month == vm.SelectMonth && c.EmployeeId == vm.EmployeeId && !oldIds.Contains(c.Id))).ToList();
            }
            else
            {
                deletableList = (await _iRepository.GetAsync(c => c.Month.Year == vm.SelectYear && c.Month.Month == vm.SelectMonth && c.EmployeeId == null && !oldIds.Contains(c.Id))).ToList();
            }

            if (dataListForAdd?.Count > 0)
            {
                dataListForAdd.ForEach(x =>
                {
                    x.StartDate = (DateTime)(!string.IsNullOrEmpty(x.StartDateStr) ? Utility.ConvertStrToDate(x.StartDateStr) : x.StartDate);
                    x.EndDate = (DateTime)(!string.IsNullOrEmpty(x.EndDateStr) ? Utility.ConvertStrToDate(x.EndDateStr) : x.EndDate);
                });

                addableList = _iMapper.Map<List<ShiftManagement>>(dataListForAdd);

                foreach (var (v, i) in addableList.GetItemWithIndex())
                {
                    var shiftMonth = new DateTime((int)vm.SelectYear, (int)vm.SelectMonth, 1);
                    v.Month = shiftMonth;
                    v.EmployeeId = vm.EmployeeId;
                    v.ActionDate = DateTime.Now;
                    v.ActionById = CurrentUserId;
                }
            }

        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        if (addableList?.Count > 0)
        {
            _iRepository.AddRange(addableList);
        }

        if (updateableList?.Count > 0)
        {
            _iRepository.UpdateRange(updateableList);
        }

        if (deletableList?.Count > 0)
        {
            _iRepository.RemoveRange(deletableList);
        }

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }
    #endregion

    #region Search
    public async Task<DataTablePagination<ShiftManagementSearchVm, ShiftManagementSearchVm>> SearchAsync(DataTablePagination<ShiftManagementSearchVm, ShiftManagementSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
    #endregion

    #region GetShiftByMonthOfYear

    public async Task<List<ShiftManagementVm>> GetShiftByMonthOfYearAsync(short year, short month, long? employeeId)
    {
        if (!(year > 1999) && !(month > 0))
            throw new Exception("Month And Year Information Is Not Correct...!");

        var existList = await _iRepository.GetAsync(c => c.Month.Year == year && c.Month.Month == month, x => x.PermanentShift, d => d.DutyShift);

        if (employeeId > 0)
        {
            existList = existList.Where(x => x.EmployeeId == employeeId).ToList();
        }
        else
        {
            existList = existList.Where(x => x.EmployeeId == null).ToList();
        }

        var modelList = _iMapper.Map<List<ShiftManagementVm>>(existList);
        if (modelList.Count > 0)
        {
            foreach (var item in modelList)
            {
                var filterData = existList.FirstOrDefault(x => x.Id == item.Id);

                item.PermanentShiftName = filterData.PermanentShift?.ShiftName;
                item.DutyShiftName = filterData.DutyShift?.ShiftName;
                item.StartDateStr = filterData.StartDate.ToString("dd/MM/yyyy");
                item.EndDateStr = filterData.EndDate.ToString("dd/MM/yyyy");
            }
        }
        return modelList;
    }

    #endregion
}
