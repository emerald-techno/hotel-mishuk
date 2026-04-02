using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrEmpSalaryPart;
using Domain.ViewModel.Payroll.PrSalaryPart;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Payroll;

public class PrEmpSalaryPartController : AppBaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitWork;
    private readonly IMapper _iMapper;
    private readonly IPrEmpSalaryPartService _iService;
    private readonly IPrSalaryPartService _iPrSalaryPartService;
    private readonly DropdownService _dropdownService;

    public PrEmpSalaryPartController(IPrEmpSalaryPartService iService,
                            IPrSalaryPartService iPrSalaryPartService,
                            IMapper iMapper,
                            IUnitOfWork iUnitOfWork,
                            DropdownService dropdownService) : base(iUnitOfWork)
    {
        _iService = iService;
        _iPrSalaryPartService = iPrSalaryPartService;
        _iMapper = iMapper;
        _iUnitWork = iUnitOfWork;
        _dropdownService = dropdownService;
    }

    #endregion

    #region Create

    [Authorize(Permissions.PrEmpSalaryParts.Create)]

    public IActionResult Create()
    {
        var model = new PrEmpSalaryPartVm();

        model.EmployeeLookup = _dropdownService.GetPrEmployeeSelectListItems();
        model.ValueTypeLookUp = _dropdownService.GetValueTypeSelectListItems();

        var salaryParts = _iPrSalaryPartService.Get(c => c.IsEnable && c.IsEmpWise);
        model.PrSalaryParts = _iMapper.Map<List<PrSalaryPartVm>>(salaryParts);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(PrEmpSalaryPartVm vm)
    {
        vm.EmployeeLookup = _dropdownService.GetPrEmployeeSelectListItems();
        vm.ValueTypeLookUp = _dropdownService.GetValueTypeSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("Create", vm);
            };
            _iService.CurrentUserId = UserId;
            var dataList = _iMapper.Map<List<PrEmpSalaryPart>>(vm.PrEmpSalaryParts);

            dataList.ForEach(c => c.ActionById = UserId);
            dataList.ForEach(c => c.ActionDate = DU.Utility.GetBdDateTimeNow());

            var isAdded = await _iService.AddRangeAsync(dataList);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", vm);
            }

            SaveSuccessMsg();

            return RedirectToAction("Create");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("Create", vm);
        }
    }

    #endregion

    #region Search

    [HttpGet]
    [Authorize(Permissions.PrEmpSalaryParts.ListView)]
    public IActionResult Search()
    {
        var vm = new PrEmpSalaryPartSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<PrEmpSalaryPartSearchVm, PrEmpSalaryPartSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<PrEmpSalaryPartSearchVm, PrEmpSalaryPartSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new PrEmpSalaryPartSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region SingleAdd

    [HttpPost]
    public async Task<IActionResult> SingleAdd(PrEmpSalaryPartVm vm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("Details", new { employeeId = vm.EmployeeId });
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<PrEmpSalaryPart>(vm);
            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();

            var existingEmpPart = _iService.GetSingleOrDefault(d => d.EmployeeId == model.EmployeeId && d.SalaryPartId == model.SalaryPartId);

            if (existingEmpPart != null)
            {
                SaveFailedMsg("Part Already Exist..!!");
                return RedirectToAction("Details", new { employeeId = vm.EmployeeId });
            }

            var isAdded = await _iService.AddAsync(model);

            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("Details", new { employeeId = vm.EmployeeId });
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", new { employeeId = vm.EmployeeId });

        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return RedirectToAction("Details", new { employeeId = vm.EmployeeId });
        }
    }

    #endregion

    #region Edit

    [HttpGet]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetFirstOrDefault(c => c.Id == id, e => e.Employee, p => p.SalaryPart);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<PrEmpSalaryPartVm>(data);
            model.EmployeeName = data.Employee.Name;
            model.SalaryPartName = data.SalaryPart.PartName;
            model.ValueTypeLookUp = _dropdownService.GetValueTypeSelectListItems();
            model.PartTypeLookUp = _dropdownService.GetSalaryPartTypeSelectListItems();
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }


    [HttpPost]
    public async Task<IActionResult> Edit(PrEmpSalaryPartVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
            var model = _iMapper.Map<PrEmpSalaryPart>(modelVm);
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
            return RedirectToAction("Details", new { employeeId = modelVm.EmployeeId });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View(modelVm);
        }
    }

    #endregion

    #region Details

    [Authorize(Permissions.PrEmpSalaryParts.DetailsView)]

    public async Task<ActionResult> Details(long employeeId)
    {
        try
        {
            if (employeeId == 0) throw new Exception("Employee Not Found...!");
            var salaryParts = _iPrSalaryPartService.Get(c => c.IsEnable && c.IsEmpWise);
            var dataList = await _iService.GetAsync(c => c.EmployeeId == employeeId, p => p.SalaryPart, e => e.Employee);
            var model = _iMapper.Map<List<PrEmpSalaryPartVm>>(dataList);
            if (model.Count > 0)
            {
                foreach (var part in model)
                {
                    var filterData = dataList.FirstOrDefault(c => c.Id == part.Id);

                    part.EmpPartName = filterData?.SalaryPart.PartName;
                    part.EmployeeName = filterData?.Employee.Name;
                    part.EmployeeCode = filterData?.Employee.Code;
                    part.EmpPhotoUrl = filterData?.Employee.PhotoUrl;
                    part.SalaryPartLookUp = salaryParts.Select(c => new SelectListItem { Text = $"{c.PartName} - ({c.PartType})", Value = $"{c.Id}" }).ToList();
                    part.ValueTypeLookUp = _dropdownService.GetValueTypeSelectListItems();
                    part.PartTypeLookUp = _dropdownService.GetSalaryPartTypeSelectListItems();
                }
            }
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region Import

    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile importFile)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isImported = await _iService.ImportAsync(importFile);
            return RedirectToAction("Search");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("Import");
        }

    }

    #endregion
}
