using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.ItemConversion;
using Domain.ViewModel.Restaurant.FoodItem;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Inventory;

public class ItemConvertionController : BaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IItemConvertionService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;

    public ItemConvertionController(IUnitOfWork iUnitOfWork,
                                    IItemConvertionService iService,
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
    [HttpGet]
    [Authorize(Permissions.Module.InventoryModule)]
    public IActionResult Create()
    {
        var model = new ItemConvertionVm();

        model.Quantity = 1;
        model.ConvertedQuantity = 0;
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ItemConvertionVm modelVm)
    {
        modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.AddAsync(modelVm);

            if (!isAdded)
            {
                if (modelVm.IsAjaxPost)
                    return BadRequest("Ingradients Add Failed..!!");

                SaveFailedMsg();
                return View("Create", modelVm);
            }

            if (modelVm.IsAjaxPost)
                return Ok(true);

            SaveSuccessMsg();

            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            if (modelVm.IsAjaxPost)
                return BadRequest(ex.Message);

            ExceptionMsg(ex.Message);
            return View("Create", modelVm);
        }
    }
    #endregion

    #region Edit
    [HttpGet]
    //[Authorize(Permissions.FoodItems.Edit)]
    [Authorize(Permissions.Module.InventoryModule)]
    public IActionResult Edit(long id)
    {
        
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<ItemConvertionVm>(data);

            //var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoUrl);
            //ViewBag.Photo = photoDoc;

            model.ItemLookUp = _dropdownService.GetItemSelectListItems();
            model.UnitLookUp = _dropdownService.GetUnitSelectListItems();

            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ItemConvertionVm modelVm)
    {
        modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();

        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            //var photoFile = modelVm.GetAppFileToUploadFolder();

            var model = _iMapper.Map<ItemConvertion>(modelVm);

            model.UpdatedById = UserId;
            model.UpdateDate = DU.Utility.GetBdDateTimeNow();
            var isAdded = await _iService.UpdateAsync(model);
            if (!isAdded)
            {
                UpdateFailedMsg();
                return View("Edit", modelVm);
            }

           // await DU.Utility.UploadFileToFolderAsync(photoFile);

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
    [Authorize(Permissions.Module.InventoryModule)]
    public IActionResult Search()
    {
        var vm = new ItemConvertionSearchVm();
        //vm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<ItemConvertionSearchVm, ItemConvertionSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<ItemConvertionSearchVm, ItemConvertionSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new ItemConvertionSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }
    #endregion

    #region Delete
    [HttpGet]
    //[Authorize(Permissions.FoodItems.Delete)]
    [Authorize(Permissions.Module.InventoryModule)]
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
}
