using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.SetSalaryGrade;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Payroll;

public class SetSalaryGradeController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IMapper _iMapper;
    private readonly ISetSalaryGradeService _iSetSalaryGradeService;
    public SetSalaryGradeController(IUnitOfWork iUnitOfWork,
                                    ISetSalaryGradeService iSetSalaryGradeService,
                                    IMapper iMapper) : base(iUnitOfWork)
    {
        _iUnitOfWork = iUnitOfWork;
        _iSetSalaryGradeService = iSetSalaryGradeService;
        _iMapper = iMapper;
    }
    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.SetSalaryGrades.Create)]
    public IActionResult Create()
    {
        var model = new SetSalaryGradeVm();
        return View(model);
    }

    [HttpPost]
    public IActionResult Create(SetSalaryGradeVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("Create", modelVm);
            }
            _iSetSalaryGradeService.CurrentUserId = UserId;
            var model = _iMapper.Map<SetSalaryGrade>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

            var isAdded = _iSetSalaryGradeService.Add(model);

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
    [Authorize(Permissions.SetSalaryGrades.ListView)]
    public IActionResult Search()
    {
        var vm = new SetSalaryGradeSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<SetSalaryGradeSearchVm, SetSalaryGradeSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<SetSalaryGradeSearchVm, SetSalaryGradeSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new SetSalaryGradeSearchVm();
        var dataTable = await _iSetSalaryGradeService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }
    #endregion

    #region Edit

    [HttpGet]
    [Authorize(Permissions.SetSalaryGrades.Edit)]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iSetSalaryGradeService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<SetSalaryGradeVm>(data);
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SetSalaryGradeVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
            var model = _iMapper.Map<SetSalaryGrade>(modelVm);

            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
            model.UpdatedById = UserId;
            model.UpdateDate = Domain.Utility.Utility.GetBdDateTimeNow();
            var isAdded = await _iSetSalaryGradeService.UpdateAsync(model);
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
    [Authorize(Permissions.SetSalaryGrades.Delete)]
    public async Task<IActionResult> Delete(long id)
    {
        var data = await _iSetSalaryGradeService.GetByIdAsync(id);
        if (data == null)
        {
            DeleteFailedMsg();
            return NotFound();
        }
        data.IsDeleted = true;
        DeleteSuccessMsg();
        var isRemove = _iSetSalaryGradeService.Update(data);
        return RedirectToAction("Search");
    }
    #endregion
}
