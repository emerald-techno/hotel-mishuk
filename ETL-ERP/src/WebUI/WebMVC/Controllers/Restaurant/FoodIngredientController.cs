using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodIngredient;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Restaurant;

public class FoodIngredientController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IFoodIngredientService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;

    public FoodIngredientController(IUnitOfWork iUnitOfWork,
                            IFoodIngredientService iService,
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
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult Create()
    {
        var model = new FoodIngredientVM();

        model.FoodItemLookUp = _dropdownService.GetFoodItemSelectListItems();
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        model.Quantity = 0;
        model.Price = 0;
        model.Amount = 0;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(FoodIngredientVM modelVm)
    {
        modelVm.FoodItemLookUp = _dropdownService.GetFoodItemSelectListItems();
        modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }

            var existingIngredient = await _iService.GetAsync(x=>x.FoodItemId==modelVm.FoodItemId && x.ItemId==modelVm.ItemId && x.IsDeleted==false);

            if (existingIngredient.Count()>0)
            {
                SaveFailedMsg("Information already exists..!!");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.FoodIngredientAddAsync(modelVm);

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

    #region Search 
    
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult Search()
    {
        var vm = new FoodIngredientSearchVM();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<FoodIngredientSearchVM, FoodIngredientSearchVM> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<FoodIngredientSearchVM, FoodIngredientSearchVM>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new FoodIngredientSearchVM();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Delete
    [HttpPost]
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
        return RedirectToAction("Search", "FoodItem");
    }
    #endregion

    #region ActiveStatus
    [HttpGet]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<IActionResult> ActiveStatus(long id, bool status)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
                throw new Exception("No Ingredient Item Found...!!");

            data.IsActive = status;

            var isUpdate = await _iService.UpdateAsync(data);
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

    #region RemoveIngredient
    [HttpGet]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<IActionResult> RemoveIngredient(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
                throw new Exception("No Ingredient Item Found...!!");

            data.IsDeleted = true;

            var isUpdate = await _iService.UpdateAsync(data);
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
}
