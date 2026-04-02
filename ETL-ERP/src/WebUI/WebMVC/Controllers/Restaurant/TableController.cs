using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodItem;
using Domain.ViewModel.Restaurant.Table;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Restaurant;

public class TableController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly ITableService _iService;
    private readonly IMapper _iMapper;

    public TableController(IUnitOfWork iUnitOfWork,
                            ITableService iService,
                            IMapper iMapper) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.Tables.Create)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult Create()
    {
        var model = new RsTableVm();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RsTableVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var model = _iMapper.Map<RsTable>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();
            model.IsActive = true;

            RsTable exixtingTable = _iService.GetSingleOrDefault(d => d.TableNo == model.TableNo);

            if (exixtingTable != null) 
            {
                ModelState.AddModelError(string.Empty, "Table already exists");
                SaveFailedMsg();
                return View("Create", modelVm);
            }
            else
            {
                var isAdded = await _iService.AddAsync(model);
                if (!isAdded)
                {
                    SaveFailedMsg();
                    return View("Create", modelVm);
                }

                SaveSuccessMsg();
            }
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
    //[Authorize(Permissions.Tables.Edit)]
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
            var model = _iMapper.Map<RsTableVm>(data);

            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(RsTableVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var model = _iMapper.Map<RsTable>(modelVm);

            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
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
    //[Authorize(Permissions.Tables.ListView)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult Search()
    {
        var vm = new RsTableSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<RsTableSearchVm, RsTableSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<RsTableSearchVm, RsTableSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new RsTableSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region EXISTING CHECK

    [AcceptVerbs("Get", "Post")]
    public async Task<IActionResult> IsNameExist(string tableNo, string initTableNo)
    {
        if (!string.IsNullOrEmpty(tableNo) && !string.IsNullOrEmpty(initTableNo) && tableNo.ToUpper().Equals(initTableNo.ToUpper()))
        {
            return Json(true);
        }

        var result = await _iService.GetFirstOrDefaultAsync(c => c.TableNo.Equals(tableNo) && !c.IsDeleted);
        if (result == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Table {tableNo} is already exist..!!");
        }

    }

    #endregion

    #region Delete
    [HttpGet]
    //[Authorize(Permissions.Tables.Delete)]
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
}