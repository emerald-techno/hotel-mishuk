using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.ItemInfo;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Inventory;

public class ItemInfoController : AppBaseController
{
    #region Config
    private readonly IMapper _iMapper;
    private readonly IItemInfoService _iService;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly DropdownService _dropdownService;
    private readonly CacheStoreService _cacheStoreService;

    public ItemInfoController(IItemInfoService iService, IMapper iMapper,
        IUnitOfWork iUnitOfWork, DropdownService dropdownService,
        CacheStoreService cacheStoreService)
        : base(iUnitOfWork, "ItemInfo")
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
    [Authorize(Permissions.ItemInfo.Create)]
    public async Task<ActionResult> Create()
    {
        var model = new ItemInfoVm();
        model.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        model.CategoryLookUp = _dropdownService.GetCategoryListItems();
        model.ItemCode = await _iService.GetItemInfoCode();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ItemInfoVm modelVm)
    {
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        modelVm.CategoryLookUp = _dropdownService.GetCategoryListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            var photoFile = modelVm.GetAppFileToUploadFolder();
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<ItemInfo>(modelVm);
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
            _cacheStoreService.AddOrUpdate<ItemInfo>(CacheEnum.ItemInfoList.ToString());
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
    [Authorize(Permissions.ItemInfo.Edit)]
    public ActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<ItemInfoVm>(data);
            model.UnitLookUp = _dropdownService.GetUnitSelectListItems();
            model.CategoryLookUp = _dropdownService.GetCategoryListItems();
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ItemInfoVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
            _iService.CurrentUserId = UserId;
            var photoFile = modelVm.GetAppFileToUploadFolder();
            var model = _iMapper.Map<ItemInfo>(modelVm);
            model.ActionById = UserId;
            model.UpdatedById = UserId;
            model.UpdateDate = DU.Utility.GetBdDateTimeNow();
            var isAdded = await _iService.UpdateAsync(model);
            if (!isAdded)
            {
                UpdateFailedMsg();
                return View("Edit", modelVm);
            }

            await DU.Utility.UploadFileToFolderAsync(photoFile);

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
    [Authorize(Permissions.ItemInfo.ListView)]
    public IActionResult Search()
    {
        var model = new ItemInfoSearchVm();
        model.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        model.CategoryLookUp = _dropdownService.GetCategoryListItems();
        return View(model);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<ItemInfoSearchVm, ItemInfoSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<ItemInfoSearchVm, ItemInfoSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new ItemInfoSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }
    #endregion

    #region Delete
    //[HttpDelete("{id}")]
    [HttpGet]
    [Authorize(Permissions.ItemInfo.Delete)]
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
    public IActionResult GetItemJsonData()
    {
        var dataList = _dropdownService.GetItemDynamicData();
        return Ok(dataList);
    }

    [HttpPost]
    public IActionResult GetUnitByItemId(long id)
    {
        try
        {
            var data = _iService.GetFirstOrDefault(c => c.Id == id, u => u.Unit, c => c.Category);
            var model = _iMapper.Map<ItemInfoVm>(data);
            model.UnitName = data.Unit.UnitName;
            model.CategoryId = data.CategoryId;
            model.LedgerId = data.Category.LedgerId;
            return Ok(model);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }


    [HttpPost]
    public async Task<IActionResult> GetItemCurrentStockByItemId(long id)
    {
        try
        {
            var stockValue = await _iService.GetItemCurrentStockByItemId(id);
            return Ok(stockValue);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }

    #endregion

    #region ForView
    //[HttpGet]
    //public IActionResult Details(long id)
    //{
    //    try
    //    {
    //        var data = new ItemInfoVm { Id = (int)id };
    //        return View(data);
    //    }
    //    catch (Exception e)
    //    {
    //        ExceptionMsg(e.Message);
    //        return View("_404");
    //    }
    //}

    //[HttpGet]
    //public IActionResult GetDelete(long id)
    //{
    //    return View();
    //}
    #endregion

    #region PARTIAL LOAD

    //public PartialViewResult GetDetailPartial(long id)
    //{
    //    var model = _iService.GetFirstOrDefault(c => c.Id == id && !c.IsDeleted, c => c.Unit);
    //    var vm = _iMapper.Map<ItemInfoVm>(model);
    //    return model != null ? PartialView("PartialView/ItemInfo/_ItemInfoDetails", vm) : PartialView("_404");
    //}

    //public PartialViewResult GetServiceDetailPartial(long id)
    //{
    //    var model = _iService.GetFirstOrDefault(c => c.Id == id && !c.IsDeleted, c => c.Unit);
    //    var vm = _iMapper.Map<ItemInfoVm>(model);
    //    return model != null ? PartialView("PartialView/ItemInfo/_ServiceInfoDetails", vm) : PartialView("_404");
    //}


    #endregion

    #region Service Create
    //[HttpGet]
    //public async Task<ActionResult> ServiceCreate()
    //{
    //    var model = new ItemInfoVm();
    //    model.UnitLookUp = _dropdownService.GetUnitSelectListItems();
    //    model.CategoryLookUp = _dropdownService.GetCategoryListItems();
    //    model.ItemCode = await _iService.GetItemInfoCode();
    //    return View(model);
    //}

    //[HttpPost]
    //public async Task<IActionResult> ServiceCreate(ItemInfoVm modelVm)
    //{
    //    modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
    //    modelVm.CategoryLookUp = _dropdownService.GetCategoryListItems();
    //    try
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            SaveFailedMsg("Information is not correct");
    //            return View("ServiceCreate", modelVm);
    //        }
    //        var photoFile = modelVm.GetAppFileToUploadFolder();
    //        _iService.CurrentUserId = UserId;
    //        var model = _iMapper.Map<ItemInfo>(modelVm);
    //        model.ActionById = UserId;
    //        model.ActionDate = DU.Utility.GetBdDateTimeNow();
    //        var isAdded = await _iService.AddAsync(model);
    //        if (!isAdded)
    //        {
    //            SaveFailedMsg();
    //            return View("ServiceCreate", modelVm);
    //        }
    //        SaveSuccessMsg();

    //        await DU.Utility.UploadFileToFolderAsync(photoFile);
    //        _cacheStoreService.AddOrUpdate<ItemInfo>(CacheEnum.ItemInfoList.ToString());
    //        return RedirectToAction("ServiceCreate");
    //    }
    //    catch (Exception e)
    //    {
    //        ExceptionMsg(e.Message);
    //        return View("ServiceCreate", modelVm);
    //    }
    //}

    #endregion

    #region API

    [HttpPost]
    public async Task<IActionResult> ItemEntry(ItemInfoVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Information is not correct");
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<ItemInfo>(modelVm);
            model.ItemCode = await _iService.GetItemInfoCode();
            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();
            var isAdded = await _iService.AddAsync(model);
            if (!isAdded)
            {
                return BadRequest("Item info entry failed..!");
            }

            return Ok(model.Id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region Details
    //[Authorize(Permissions.FoodItems.DetailView)]
    [Authorize(Permissions.Module.InventoryModule)]
    public async Task<IActionResult> Details(long id)
    {
        try
        {
            var data = await _iService.Details(id);
           
            //data.UnitLookUp = _dropdownService.GetUnitSelectListItems();
            //data.CategoryLookUp = _dropdownService.GetFoodItemSelectListItems();
            if (data == null)
                throw new Exception("No Food Item Found..!");

            return View(data);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    #endregion
}
