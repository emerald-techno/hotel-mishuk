using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodCategory;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HotelManagement;

public class FoodCategoryController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IFoodCategoryService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;

    public FoodCategoryController(IUnitOfWork iUnitOfWork,
                            IFoodCategoryService iService,
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
    //[Authorize(Permissions.FoodCategories.Create)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult Create()
    {
        var model = new FoodCategoryVm();
        model.FoodCategoryLookUp = _dropdownService.GetFoodCategorySelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(FoodCategoryVm modelVm)
    {
        modelVm.FoodCategoryLookUp = _dropdownService.GetFoodCategorySelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var isAdded = await _iService.FoodCategoryAddAsync(modelVm);

            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            await DU.Utility.UploadFileToFolderAsync(photoFile);

            SaveSuccessMsg();

            return RedirectToAction("Search");
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
    //[Authorize(Permissions.FoodCategories.Edit)]
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
            var model = _iMapper.Map<FoodCategoryVm>(data);

            var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoUrl);
            ViewBag.Photo = photoDoc;

            model.FoodCategoryLookUp = _dropdownService.GetFoodCategorySelectListItems();

            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(FoodCategoryVm modelVm)
    {
        //modelVm.FoodCategoryLookUp = _dropdownService.GetFoodCategorySelectListItems();

        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var model = _iMapper.Map<RsFoodCategory>(modelVm);

            model.UpdatedById = UserId;
            model.UpdateDate = DU.Utility.GetBdDateTimeNow();
            var isAdded = await _iService.UpdateAsync(model);
            if (!isAdded)
            {
                modelVm.FoodCategoryLookUp = _dropdownService.GetFoodCategorySelectListItems();
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

    #region Delete
    [HttpGet]
    //[Authorize(Permissions.FoodCategories.Delete)]
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

    #region Search
    [HttpGet]
    //[Authorize(Permissions.FoodCategories.ListView)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult Search()
    {
        var vm = new FoodCategorySearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<FoodCategorySearchVm, FoodCategorySearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<FoodCategorySearchVm, FoodCategorySearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new FoodCategorySearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region JsonData

    public async Task<IActionResult> GetFoodCatgoryInfoById(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            return BadRequest("Food Category Not Found..!!!");
        }
        var model = _iMapper.Map<FoodCategorySearchVm>(data);
        return Ok(model);
    }

    public IActionResult GetFoodCategoryJsonData()
    {
        var dataList = _dropdownService.GetFoodCategoryDynamicData();
        return Ok(dataList);
    }

    #endregion
}
