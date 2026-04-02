using AutoMapper;
using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.TaskType;
using Interface.Repository.HouseKeeping;
using Interface.Services.HouseKeeping;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.HouseKeeping;

public class TaskTypeService : BaseService<HkTaskType>, ITaskTypeService
{
    #region Config
    private ITaskTypeRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public TaskTypeService(ITaskTypeRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    public async Task<DataTablePagination<HkTaskTypeSearchVm, HkTaskTypeSearchVm>> SearchAsync(DataTablePagination<HkTaskTypeSearchVm, HkTaskTypeSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
}
