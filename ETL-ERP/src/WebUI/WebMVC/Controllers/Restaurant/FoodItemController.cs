using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodItem;
using Interface.Services.Inventory;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Inventory;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Restaurant;

public class FoodItemController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IFoodItemService _iService;
    private readonly IInventoryReportService _iInventoryReportService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _iDropdownService;

    public FoodItemController(IUnitOfWork iUnitOfWork,
                            IFoodItemService iService,
                            IInventoryReportService iInventoryReportService,
                            IMapper iMapper,
                            DropdownService dropdownService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iInventoryReportService = iInventoryReportService;
        _iMapper = iMapper;
        _iDropdownService = dropdownService;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.FoodItems.Create)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<IActionResult> Create()
    {
        var model = new FoodItemVm();
        //model.ItemCode = await _iService.GetFoodItemCode();
        model.FoodCategoryLookUp = _iDropdownService.GetFoodCategorySelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(FoodItemVm modelVm)
    {
        modelVm.FoodCategoryLookUp = _iDropdownService.GetFoodCategorySelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var isAdded = await _iService.FoodAddAsync(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            await DU.Utility.UploadFileToFolderAsync(photoFile);

            SaveSuccessMsg();
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("Create", modelVm);
        }
    }
    #endregion

    #region Edit
    [HttpGet]
    //[Authorize(Permissions.FoodItems.Edit)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<FoodItemVm>(data);

            var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoUrl);
            ViewBag.Photo = photoDoc;

            model.FoodCategoryLookUp = _iDropdownService.GetFoodCategorySelectListItems();

            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(FoodItemVm modelVm)
    {
        modelVm.FoodCategoryLookUp = _iDropdownService.GetFoodCategorySelectListItems();

        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var model = _iMapper.Map<RsFoodItem>(modelVm);

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
    //[Authorize(Permissions.FoodItems.ListView)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult Search()
    {
        var vm = new FoodItemSearchVm();
        vm.FoodCategoryLookUp = _iDropdownService.GetFoodCategorySelectListItems();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<FoodItemSearchVm, FoodItemSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<FoodItemSearchVm, FoodItemSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new FoodItemSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Details
    //[Authorize(Permissions.FoodItems.DetailView)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<IActionResult> Details(long id)
    {
        try
        {
            var data = await _iService.FoodDetails(id);
            data.ItemLookup = _iDropdownService.GetItemSelectListItems();
            data.UnitLookup = _iDropdownService.GetUnitSelectListItems();
            data.FoodItemLookup = _iDropdownService.GetFoodItemSelectListItems();
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

    #region EXISTING CHECK

    [AcceptVerbs("Get", "Post")]
    public async Task<IActionResult> IsNameExist(string itemName, string initItemName, long categoryId)
    {
        if (!string.IsNullOrEmpty(itemName) && !string.IsNullOrEmpty(initItemName) && itemName.ToUpper().Equals(initItemName.ToUpper()))
        {
            return Json(true);
        }

        var result = await _iService.GetFirstOrDefaultAsync(c => c.ItemName.Equals(itemName) && c.CategoryId == categoryId && !c.IsDeleted);
        if (result == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Food Item {itemName} is already exist..!!");
        }

    }

    [AcceptVerbs("Get", "Post")]
    public async Task<IActionResult> IsCodeExist(string itemCode, string initCode)
    {
        if (!string.IsNullOrEmpty(itemCode) && !string.IsNullOrEmpty(initCode) && itemCode.ToUpper().Equals(initCode.ToUpper()))
        {
            return Json(true);
        }

        var result = await _iService.GetFirstOrDefaultAsync(c => c.ItemCode.Equals(itemCode) && !c.IsDeleted);
        if (result == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Code {itemCode} Is Already Exist..!!");
        }

    }

    #endregion

    #region Delete
    [HttpGet]
    //[Authorize(Permissions.FoodItems.Delete)]
    [Authorize(Permissions.Module.RestaurantModule)]
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

    #region JsonData
    public async Task<IActionResult> GetFoodInfoById(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            return BadRequest("Food Item Not Found..!!!");
        }
        var model = _iMapper.Map<FoodItemVm>(data);
        return Ok(model);
    }

    public async Task<IActionResult> GetFoodItems()
    {
        var data = await _iService.GetFoodByCategoryIdAsync();
        return Ok(data);
    }

    public async Task<IActionResult> GetFoodByCategoryId(long categoryId)
    {
        var data = await _iService.GetFoodByCategoryIdAsync(categoryId);
        return Ok(data);
    }

    public async Task<IActionResult> GetFoodByNameOrCode(string value)
    {
        var data = await _iService.GetFoodByNameOrCodeAsync(value);
        return Ok(data);
    }

    public async Task<IActionResult> GetSetItemsByFoodId(long id)
    {
        var data = await _iService.GetSetMenuItems(id);
        return Ok(data);
    }
    [HttpPost]
    public IActionResult GetFoodItemJsonData()
    {
        var dataList = _iDropdownService.GetFoodItemDynamicData();
        return Ok(dataList);
    }
    public async Task<IActionResult> GetItemAverageAmountJsonData(long id)
    {
        var data = await _iInventoryReportService.GetItemAverageAmountJsonData(id);
        if (data == null)
            throw new Exception("Data Not Found");
        else
            return Ok(data);
    }
    #endregion

    #region FoodSetItemAdd

    [HttpPost]
    public async Task<IActionResult> FoodSetItemAdd(FoodSetItemVm dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                )
                });
            }
            ;

            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.SetFoodItemAddAsync(dto);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region SetMenuItemStatus
    [HttpGet]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<IActionResult> SetMenuItemStatus(long id, bool status)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isUpdate = await _iService.SetFoodItemActiveInActiveAsync(id, status);
            return Ok(isUpdate);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Update Failed..!!",
                errors = e.Message
            });
        }
    }
    #endregion

    #region RemoveMenuItem
    [HttpGet]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<IActionResult> RemoveMenuItem(long id)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isUpdate = await _iService.DeleteSetItem(id);
            return Ok(isUpdate);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Remove Failed..!!",
                errors = e.Message
            });
        }
    }
    #endregion

    #region Copy And Create a Setmenu
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult CreateCopy(long id)
    {
        try
        {
            var data = _iService.GetById(id);

            if (data == null)
            {
                return NotFoundMsg();
            }

            var model = _iMapper.Map<FoodItemVm>(data);
            model.FoodItemLookup = _iDropdownService.GetFoodItemSelectListItems();
            model.FoodCategoryLookUp = _iDropdownService.GetFoodCategorySelectListItems();

            var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoUrl);
            ViewBag.Photo = photoDoc;
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [Authorize(Permissions.Module.RestaurantModule)]
    [HttpPost]
    public async Task<IActionResult> CreateCopy(FoodItemVm model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", model);
            }
            _iService.CurrentUserId = UserId;
            var photoFile = model.GetAppFileToUploadFolder();
            var isAdded = await _iService.SetItemCopyAddAsync(model);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", model);
            }

            await DU.Utility.UploadFileToFolderAsync(photoFile);
            SaveSuccessMsg();
            return RedirectToAction("Search");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }
    #endregion

    #region GetFoodItemRateById
    public async Task<double> GetFoodItemRateById(int id)
    {
        var data = await _iService.GetByIdAsync(id);
        var netRate = data.NetRate;
        return netRate;
    }
    #endregion

}