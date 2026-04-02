using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Waiter;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Restaurant;

public class WaiterController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IWaiterService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;

    public WaiterController(IUnitOfWork iUnitOfWork,
                            IWaiterService iService,
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
        var model = new WaiterVm();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(WaiterVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;
            modelVm.Dob = (!string.IsNullOrEmpty(modelVm.DobStr)) ? (DateTime)DU.Utility.ConvertStrToDate(modelVm.DobStr) : modelVm.Dob;
            modelVm.JoinDate = (!string.IsNullOrEmpty(modelVm.JoinDateStr)) ? (DateTime)DU.Utility.ConvertStrToDate(modelVm.JoinDateStr) : modelVm.JoinDate;

            var isAdded = await _iService.WaiterAddAsync(modelVm);

            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

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
            var model = _iMapper.Map<WaiterVm>(data);

            model.DobStr = model.Dob?.ToString("dd/MM/yyyy");
            model.JoinDateStr = model.JoinDate?.ToString("dd/MM/yyyy");

            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(WaiterVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var model = _iMapper.Map<RsWaiter>(modelVm);

            model.Dob = !string.IsNullOrEmpty(modelVm.DobStr) ? DU.Utility.ConvertStrToDate(modelVm.DobStr) : null;
            model.JoinDate = !string.IsNullOrEmpty(modelVm.JoinDateStr) ? DU.Utility.ConvertStrToDate(modelVm.JoinDateStr) : null;

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
        var vm = new WaiterSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<WaiterSearchVm, WaiterSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<WaiterSearchVm, WaiterSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new WaiterSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion
}
