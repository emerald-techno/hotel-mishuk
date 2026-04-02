using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Complementary;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.HotelManagement;

public class ComplementaryController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IComplementaryService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;

    public ComplementaryController(IUnitOfWork iUnitOfWork,
                            IComplementaryService iService,
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
    //[Authorize(Permissions.Complementary.Create)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Create()
    {
        var model = new HtComplementaryVm();
        return View(model);
    }

    [HttpPost]
    public Task<IActionResult> Create(HtComplementaryVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return Task.FromResult<IActionResult>(View("Create", modelVm));
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<HtComplementary>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
            model.IsActive = true;

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

    #region Edit
    [HttpGet]
    //[Authorize(Permissions.Complementary.Edit)]
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
            var model = _iMapper.Map<HtComplementaryVm>(data);
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HtComplementaryVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var model = _iMapper.Map<HtComplementary>(modelVm);
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
    //[Authorize(Permissions.Complementary.Delete)]
    [Authorize(Permissions.Module.HotelManagementModule)]
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
    //[Authorize(Permissions.Complementary.ListView)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Search()
    {
        var vm = new HtComplementarySearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<HtComplementarySearchVm, HtComplementarySearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<HtComplementarySearchVm, HtComplementarySearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new HtComplementarySearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region JsonData

    public async Task<IActionResult> GetComplementaryById(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            return BadRequest("Complementary Not Found..!!!");
        }
        var model = _iMapper.Map<HtComplementaryVm>(data);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult GetComplementaryJsonData()
    {
        var dataList = _dropdownService.GetComplementaryDynamicData();
        return Ok(dataList);
    }

    #endregion
}
