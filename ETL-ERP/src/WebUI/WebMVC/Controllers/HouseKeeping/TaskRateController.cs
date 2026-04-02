using AutoMapper;
using Domain.Entities.HouseKeeping;
using Domain.ViewModel.HouseKeeping.TaskName;
using Domain.ViewModel.HouseKeeping.TaskRate;
using Interface.Services.HouseKeeping;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.HouseKeeping;

public class TaskRateController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly ITaskRateService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;

    public TaskRateController(IUnitOfWork iUnitOfWork,
                            ITaskRateService iService,
                            IMapper iMapper,
                            DropdownService dropdownService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _dropdownService = dropdownService;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.TaskNames.Create)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public IActionResult Create()
    {
        var model = new HkTaskRateVm();
        model.TaskNameLookUp = _dropdownService.GetTaskNameSelectListItems();
        return View(model);
    }

    [HttpPost]
    public Task<IActionResult> Create(HkTaskRateVm modelVm)
    {
        modelVm.TaskNameLookUp = _dropdownService.GetTaskNameSelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return Task.FromResult<IActionResult>(View("Create", modelVm));
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<HkTaskRate>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

            var isAdded = _iService.Add(model);
            if (!isAdded)
            {
                SaveFailedMsg();
                return Task.FromResult<IActionResult>(View("Create", modelVm));
            }
            SaveSuccessMsg();

            return Task.FromResult<IActionResult>(RedirectToAction("Search"));
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return Task.FromResult<IActionResult>(View("Create", modelVm));
        }
    }
    #endregion
}
