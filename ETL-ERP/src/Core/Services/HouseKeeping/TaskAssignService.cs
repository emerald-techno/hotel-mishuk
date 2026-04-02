using AutoMapper;
using Domain.Entities.HouseKeeping;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.TaskAssign;
using Interface.Repository.HouseKeeping;
using Interface.Services.HouseKeeping;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;

namespace Services.HouseKeeping;

public class TaskAssignService : BaseService<HkTaskAssign>, ITaskAssignService
{
    #region Config
    private ITaskAssignRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public TaskAssignService(ITaskAssignRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }
    #endregion

    #region AssignRoomAdd
    public async Task<bool> AddAssignTask(TaskAssignSaveVm vm)
    {
        if (vm?.TaskAssignSaveVms == null || vm.TaskAssignSaveVms.Count <= 0) throw new Exception("Sorry! No task found to assign!");

        List<HkTaskAssign> addableList = null;

        if (vm?.TaskAssignSaveVms?.Count > 0)
        {
            var dataListForAdd = vm?.TaskAssignSaveVms?.Where(c => c.AssignId > 0 && c.TaskId > 0).ToList();

            if (dataListForAdd?.Count > 0)
            {
                addableList = _iMapper.Map<List<HkTaskAssign>>(dataListForAdd);

                foreach (var (v, i) in addableList.GetItemWithIndex())
                {
                    v.AssignDate = DateTime.Now;
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

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }
    #endregion

    #region Search

    public async Task<DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm>>
        SearchAsync(DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region GetTaskByAssign

    public async Task<List<TaskAssignSearchVm>> GetTaskByAssignId(long assignId)
    {
        if(assignId == 0)
            throw new ArgumentNullException(nameof(assignId));

        var searchVm = new DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm>();
        searchVm.SearchModel = new TaskAssignSearchVm();
        searchVm.SearchModel.AssignId = assignId;

        var dataList = await _iRepository.AssignTasks(searchVm);
        return dataList.data;
    }

    #endregion
}
