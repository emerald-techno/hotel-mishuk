using AutoMapper;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.FinancialYear;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Admin;

public class SetFincYearController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly ISetFincYearService _iService;
    private readonly IMapper _iMapper;

    public SetFincYearController(IUnitOfWork iUnitOfWork,
                            ISetFincYearService iService,
                            IMapper iMapper) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
    }
    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.SetFincYear.Create)]
    public IActionResult Create()
    {
        var model = new SetFincYearVm();
        return View(model);
    }

    [HttpPost]
    public IActionResult Create(SetFincYearVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<SetFincYear>(modelVm);
            model.YearStartDate = (DateTime)(!string.IsNullOrEmpty(modelVm.YearStartDateStr) ? DU.Utility.ConvertStrToDate(modelVm.YearStartDateStr) : modelVm.YearStartDate);
            model.YearEndDate = (DateTime)(!string.IsNullOrEmpty(modelVm.YearEndDateStr) ? DU.Utility.ConvertStrToDate(modelVm.YearEndDateStr) : modelVm.YearEndDate);
            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();
            model.IsActive = true;
            model.IsDeleted = false;

            SetFincYear existingData = _iService.GetSingleOrDefault(d => d.YearName == model.YearName);
            if (existingData != null)
            {
                ModelState.AddModelError(string.Empty, "Year already exists");
            }

            var isAdded = _iService.Add(model);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }
            SaveSuccessMsg();

            return RedirectToAction("Create");

        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("Create", modelVm);
        }
    }
    #endregion

    #region Search
    [HttpGet]
    [Authorize(Permissions.SetFincYear.ListView)]
    public IActionResult Search()
    {
        var vm = new SetFincYearSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<SetFincYearSearchVm, SetFincYearSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<SetFincYearSearchVm, SetFincYearSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new SetFincYearSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Edit
    [HttpGet]
    [Authorize(Permissions.SetFincYear.Edit)]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<SetFincYearVm>(data);
            model.YearStartDateStr = model.YearStartDate.ToString("dd/MM/yyyy");
            model.YearEndDateStr = model.YearEndDate.ToString("dd/MM/yyyy");
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SetFincYearVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
            var model = _iMapper.Map<SetFincYear>(modelVm);
            model.YearStartDate = (DateTime)(!string.IsNullOrEmpty(modelVm.YearStartDateStr) ? DU.Utility.ConvertStrToDate(modelVm.YearStartDateStr) : modelVm.YearStartDate);
            model.YearEndDate = (DateTime)(!string.IsNullOrEmpty(modelVm.YearEndDateStr) ? DU.Utility.ConvertStrToDate(modelVm.YearEndDateStr) : modelVm.YearEndDate);
            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();
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

    #region Delete
    [HttpGet]
    [Authorize(Permissions.SetFincYear.Delete)]
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

    #region EXISTING CHECK

    [AcceptVerbs("Get", "Post")]
    public async Task<IActionResult> IsYearExist(string yearName, string initYear)
    {
        if (!string.IsNullOrEmpty(yearName) && !string.IsNullOrEmpty(initYear) && yearName.ToUpper().Equals(initYear.ToUpper()))
        {
            return Json(true);
        }

        var result = await _iService.GetFirstOrDefaultAsync(c => c.YearName.Equals(yearName) && !c.IsDeleted);
        if (result == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Year {yearName} Is Already Exist..!!");
        }

    }

    #endregion
}
