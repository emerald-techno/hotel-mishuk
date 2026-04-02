using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.BedType;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.HotelManagement;

public class BedTypeController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IBedTypeService _iService;
    private readonly IMapper _iMapper;

    public BedTypeController(IUnitOfWork iUnitOfWork,
                            IBedTypeService iService,
                            IMapper iMapper) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.BedTypes.Create)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Create()
    {
        var model = new HtBedTypeVm();
        return View(model);
    }

    [HttpPost]
    public Task<IActionResult> Create(HtBedTypeVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return Task.FromResult<IActionResult>(View("Create", modelVm));
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<HtBedType>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

            var isAdded = _iService.Add(model);
            if (!isAdded)
            {
                SaveFailedMsg();
                return Task.FromResult<IActionResult>(View("Create", modelVm));
            }
            SaveSuccessMsg();

            return Task.FromResult<IActionResult>(RedirectToAction("Create"));
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return Task.FromResult<IActionResult>(View("Create", modelVm));
        }
    }
    #endregion

    #region Edit
    [HttpGet]
    //[Authorize(Permissions.BedTypes.Edit)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<HtBedTypeVm>(data);
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HtBedTypeVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var model = _iMapper.Map<HtBedType>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
            model.UpdatedById = UserId;
            model.UpdateDate = Domain.Utility.Utility.GetBdDateTimeNow();
            var isAdded = await _iService.UpdateAsync(model);
            if (!isAdded)
            {
                UpdateFailedMsg();
                return View("Edit", modelVm);
            }
            UpdateSuccessMsg();

            return RedirectToAction("Search");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View(modelVm);
        }
    }
    #endregion

    #region Delete
    [HttpGet]
    [Authorize(Permissions.Module.HotelManagementModule)]
    //[Authorize(Permissions.BedTypes.Delete)]
    public async Task<IActionResult> Delete(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            DeleteFailedMsg();
            return NotFound();
        }
        data.IsDeleted = true;
        DeleteSuccessMsg();
        var isRemove = _iService.Update(data);
        return RedirectToAction("Search");
    }
    #endregion

    #region Search
    [HttpGet]
    //[Authorize(Permissions.BedTypes.ListView)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Search()
    {
        var vm = new HtBedTypeSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<HtBedTypeSearchVm, HtBedTypeSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<HtBedTypeSearchVm, HtBedTypeSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new HtBedTypeSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

}