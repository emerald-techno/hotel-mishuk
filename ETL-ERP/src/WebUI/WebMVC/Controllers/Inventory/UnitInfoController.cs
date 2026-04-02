using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.UnitInfo;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Inventory;

public class UnitInfoController : AppBaseController
{
    #region Config
    private readonly IMapper _iMapper;
    private readonly IUnitInfoService _iService;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly DropdownService _dropdownService;
    private readonly CacheStoreService _cacheStoreService;

    public UnitInfoController(IUnitInfoService iService, IMapper iMapper, IUnitOfWork iUnitOfWork,
        DropdownService dropdownService,
        CacheStoreService cacheStoreService) :
        base(iUnitOfWork, "UnitInfo")
    {
        _iService = iService;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _dropdownService = dropdownService;
        _cacheStoreService = cacheStoreService;
    }

    #endregion

    #region Create
    [HttpGet]
    [Authorize(Permissions.UnitInfo.Create)]
    public async Task<ActionResult> Create()
    {
        var model = new UnitInfoVm();
        model.UnitCode = await _iService.GetUnitInfoCode();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UnitInfoVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<UnitInfo>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();
            var isAdded = await _iService.AddAsync(model);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }
            SaveSuccessMsg();
            _cacheStoreService.AddOrUpdate<UnitInfo>(CacheEnum.UnitInfoList.ToString());

            return RedirectToAction("Create");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("Create", modelVm);
        }
    }
    #endregion

    #region Edit
    [HttpGet]
    [Authorize(Permissions.UnitInfo.Edit)]
    public ActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<UnitInfoVm>(data);
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UnitInfoVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<UnitInfo>(modelVm);
            model.ActionById = UserId;
            model.UpdatedById = UserId;
            model.UpdateDate = DU.Utility.GetBdDateTimeNow();
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

    #region Search
    [HttpGet]
    [Authorize(Permissions.UnitInfo.ListView)]
    public IActionResult Search()
    {
        var vm = new UnitInfoSearchVm();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<UnitInfoSearchVm, UnitInfoSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<UnitInfoSearchVm, UnitInfoSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new UnitInfoSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }
    #endregion

    #region Delete
    [HttpGet]
    [Authorize(Permissions.UnitInfo.Delete)]
    //[HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            return NotFound();
        }
        data.IsDeleted = true;
        var isRemoved = _iService.Update(data);
        //return Ok(isRemoved);
        return RedirectToAction("Search");
    }
    #endregion

    #region JSON Data

    //public IActionResult GetUnitJsonData()
    //{
    //    var dataList = _dropdownService.GetUnitDynamicData();

    //    return Json(dataList);
    //}

    #endregion
}