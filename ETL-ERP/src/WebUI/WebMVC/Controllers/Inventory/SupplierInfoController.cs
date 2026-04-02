using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.SupplierInfo;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace MPSC.Controllers;

public class SupplierInfoController : AppBaseController
{
    #region Config
    private readonly IMapper _iMapper;
    private readonly ISupplierInfoService _iService;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly DropdownService _dropdownService;
    private readonly CacheStoreService _cacheStoreService;

    public SupplierInfoController(ISupplierInfoService iService, IMapper iMapper,
        IUnitOfWork iUnitOfWork, DropdownService dropdownService,
        CacheStoreService cacheStoreService)
        : base(iUnitOfWork, "SupplierInfo")
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
    [Authorize(Permissions.SupplierInfo.Create)]
    public async Task<ActionResult> Create()
    {
        var model = new SupplierInfoVm();
        model.SupplierCode = await _iService.GetSupplierInfoCode();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SupplierInfoVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            var photoFile = modelVm.GetAppFileToUploadFolder();
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<SupplierInfo>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();
            var isAdded = await _iService.AddAsync(model);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }
            SaveSuccessMsg();

            await DU.Utility.UploadFileToFolderAsync(photoFile);

            _cacheStoreService.AddOrUpdate<SupplierInfo>(CacheEnum.SupplierInfoList.ToString());

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
    [Authorize(Permissions.SupplierInfo.Edit)]
    public ActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<SupplierInfoVm>(data);
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SupplierInfoVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<SupplierInfo>(modelVm);
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
    [Authorize(Permissions.SupplierInfo.ListView)]
    public IActionResult Search()
    {
        var vm = new SupplierInfoSearchVm();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<SupplierInfoSearchVm, SupplierInfoSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<SupplierInfoSearchVm, SupplierInfoSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new SupplierInfoSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }
    #endregion

    #region Detail
    [HttpGet]
    [Authorize(Permissions.SupplierInfo.Detail)]
    public IActionResult Details(long id)
    {
        try
        {
            var data = new SupplierInfoVm { Id = (int)id };
            return View(data);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region PARTIAL LOAD
    public PartialViewResult GetDetailPartial(long id)
    {
        var model = _iService.GetFirstOrDefault(c => c.Id == id && !c.IsDeleted);
        var vm = _iMapper.Map<SupplierInfoVm>(model);
        return model != null ? PartialView("PartialView/SupplierInfo/_SupplierInfoDetails", vm) : PartialView("_404");
    }
    #endregion

    #region Delete
    [HttpGet]
    [Authorize(Permissions.SupplierInfo.Delete)]
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

    #region Json Data
    [HttpPost]
    public IActionResult GetItemBySupplierId(long id)
    {
        try
        {
            var data = _iService.GetFirstOrDefault(c => c.Id == id);
            return Ok(data);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }


    #endregion

}
