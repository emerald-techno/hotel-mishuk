using AutoMapper;
using Domain.Entities.HR;
using Domain.Utility.Common;
using Domain.ViewModel.Hr.Employees;
using Interface.Repository.Hr;
using Interface.Services.Hr;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Hr;

public class EmployeeController : AppBaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitWork;
    private readonly IEmployeeService _iService;
    private readonly DropdownService _dropdownService;
    private readonly IMapper _iMapper;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _iHttpContextAccessor;
    private IEmployeeRepository _Repository;

    public EmployeeController(IUnitOfWork iUnitOfWork,
                            IEmployeeService iService,
                            DropdownService dropdownService,
                            IMapper iMapper,
                            IWebHostEnvironment iWebHostEnvironment,
                            IEmployeeRepository Repository,
                            IHttpContextAccessor iHttpContextAccessor) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _dropdownService = dropdownService;
        _iMapper = iMapper;
        _env = iWebHostEnvironment;
        _iHttpContextAccessor = iHttpContextAccessor;
        _Repository = Repository;
    }

    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.Employees.Create)]
    public async Task<IActionResult> Create()
    {
        var model = new EmployeeVm();
        model.Code = await _iService.GetEmployeeCode();
        model.RoleLookUp = _dropdownService.GetRoleSelectListItems();
        model.GenderLookUp = _dropdownService.GetGenderSelectListItems();
        model.BloodGroupLookUp = _dropdownService.GetBloodGroupSelectListItems();
        model.MaritalStatusLookUp = _dropdownService.GetMaritalStatusSelectListItems();
        model.EmployeeStatusLookUp = _dropdownService.GetEmployeeStatusSelectListItems();
        model.SalaryTypeLookUp = _dropdownService.GetSalaryTypeSelectListItems();
        model.DesignationLookUp = _dropdownService.GetDesignationSelectListItems();
        model.DepartmentLookUp = _dropdownService.GetNonAcademicDepartmentSelectListItems();
        model.AcademicDepartmentLookUp = _dropdownService.GetAcademicDepartmentSelectListItems();
        model.ReligionLookUp = _dropdownService.GetReligionSelectListItems();
        model.DutyShiftLookUp = _dropdownService.GetPermanentShiftSelectListItems();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(EmployeeVm modelVm)
    {
        modelVm.RoleLookUp = _dropdownService.GetRoleSelectListItems();
        modelVm.GenderLookUp = _dropdownService.GetGenderSelectListItems();
        modelVm.BloodGroupLookUp = _dropdownService.GetBloodGroupSelectListItems();
        modelVm.MaritalStatusLookUp = _dropdownService.GetMaritalStatusSelectListItems();
        modelVm.EmployeeStatusLookUp = _dropdownService.GetEmployeeStatusSelectListItems();
        modelVm.SalaryTypeLookUp = _dropdownService.GetSalaryTypeSelectListItems();
        modelVm.DesignationLookUp = _dropdownService.GetDesignationSelectListItems();
        modelVm.DepartmentLookUp = _dropdownService.GetNonAcademicDepartmentSelectListItems();
        modelVm.AcademicDepartmentLookUp = _dropdownService.GetAcademicDepartmentSelectListItems();
        modelVm.ReligionLookUp = _dropdownService.GetReligionSelectListItems();
        modelVm.DutyShiftLookUp = _dropdownService.GetPermanentShiftSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var photoFile = modelVm.GetAppFileToUploadFolder();
            var signatureFile = modelVm.GetSignatureFileToUploadFolder();
            var nidFile = modelVm.GetNidFileToUploadFolder();

            var isAdded = await _iService.EmployeeAddAsync(modelVm);

            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            await DU.Utility.UploadFileToFolderAsync(photoFile);
            await DU.Utility.UploadFileToFolderAsync(signatureFile);
            await DU.Utility.UploadFileToFolderAsync(nidFile);

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
    [Authorize(Permissions.Employees.ListView)]
    public IActionResult Search()
    {
        var vm = new EmployeeSearchVm();
        vm.DesignationLookUp = _dropdownService.GetDesignationSelectListItems();
        vm.DepartmentLookUp = _dropdownService.GetDepartmentSelectListItems();
        vm.EmployeeIsEnableLookUp = _dropdownService.GetEmployeeIsEnableSelectListItems();
        vm.EnableStatus = 2;
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<EmployeeSearchVm, EmployeeSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<EmployeeSearchVm, EmployeeSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new EmployeeSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Edit

    [HttpGet]
    [Authorize(Permissions.Employees.Edit)]
    public async Task<IActionResult> Edit(long id)
    {
        try
        {
            var data = await _iService.GetByIdAsync(id);

            if (data == null)
            {
                return NotFoundMsg();
            }

            var model = _iMapper.Map<EmployeeVm>(data);

            model.DobStr = model.Dob?.ToString("dd/MM/yyyy");
            model.JoinDateStr = model.JoinDate.ToString("dd/MM/yyyy");
            model.ProbationDateStr = model.ProbationDate?.ToString("dd/MM/yyyy") ?? "";
            model.ConfirmationDateStr = model.ConfirmationDate?.ToString("dd/MM/yyyy") ?? "";

            var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoUrl);
            ViewBag.Photo = photoDoc;

            //var path = Path.Combine(_env.ContentRootPath, $"{DU.Utility.UploadingFolderPath}", $"{model.PhotoUrl}");
            //var imageFileStream = System.IO.File.OpenRead(path);

            model.GenderLookUp = _dropdownService.GetGenderSelectListItems();
            model.BloodGroupLookUp = _dropdownService.GetBloodGroupSelectListItems();
            model.MaritalStatusLookUp = _dropdownService.GetMaritalStatusSelectListItems();
            model.EmployeeStatusLookUp = _dropdownService.GetEmployeeStatusSelectListItems();
            model.SalaryTypeLookUp = _dropdownService.GetSalaryTypeSelectListItems();
            model.DesignationLookUp = _dropdownService.GetDesignationSelectListItems();
            model.DepartmentLookUp = _dropdownService.GetNonAcademicDepartmentSelectListItems();
            model.AcademicDepartmentLookUp = _dropdownService.GetAcademicDepartmentSelectListItems();
            model.ReligionLookUp = _dropdownService.GetReligionSelectListItems();
            model.DutyShiftLookUp = _dropdownService.GetPermanentShiftSelectListItems();

            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EmployeeVm modelVm)
    {
        modelVm.GenderLookUp = _dropdownService.GetGenderSelectListItems();
        modelVm.BloodGroupLookUp = _dropdownService.GetBloodGroupSelectListItems();
        modelVm.MaritalStatusLookUp = _dropdownService.GetMaritalStatusSelectListItems();
        modelVm.EmployeeStatusLookUp = _dropdownService.GetEmployeeStatusSelectListItems();
        modelVm.SalaryTypeLookUp = _dropdownService.GetSalaryTypeSelectListItems();
        modelVm.DesignationLookUp = _dropdownService.GetDesignationSelectListItems();
        modelVm.DepartmentLookUp = _dropdownService.GetNonAcademicDepartmentSelectListItems();
        modelVm.AcademicDepartmentLookUp = _dropdownService.GetAcademicDepartmentSelectListItems();
        modelVm.ReligionLookUp = _dropdownService.GetReligionSelectListItems();
        modelVm.DutyShiftLookUp = _dropdownService.GetPermanentShiftSelectListItems();

        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var model = _iMapper.Map<Employee>(modelVm);

            model.Dob = !string.IsNullOrEmpty(modelVm.DobStr) ? DU.Utility.ConvertStrToDate(modelVm.DobStr) : modelVm.Dob;
            model.JoinDate = (DateTime)(!string.IsNullOrEmpty(modelVm.JoinDateStr) ? DU.Utility.ConvertStrToDate(modelVm.JoinDateStr) : modelVm.JoinDate);
            model.ProbationDate = !string.IsNullOrEmpty(modelVm.ProbationDateStr) ? DU.Utility.ConvertStrToDate(modelVm.ProbationDateStr) : modelVm.ProbationDate;
            model.ConfirmationDate = !string.IsNullOrEmpty(modelVm.ConfirmationDateStr) ? DU.Utility.ConvertStrToDate(modelVm.ConfirmationDateStr) : modelVm.ConfirmationDate;
            model.RetirementDate = !string.IsNullOrEmpty(modelVm.RetirementDateStr) ? DU.Utility.ConvertStrToDate(modelVm.RetirementDateStr) : modelVm.RetirementDate;

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
            return RedirectToAction("Details", new { id = modelVm.Id });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View(modelVm);
        }
    }

    #endregion

    #region Disable

    [HttpGet]
    [Authorize(Permissions.Employees.Disable)]
    public async Task<IActionResult> Disable(long id)
    {
        try
        {
            var data = await _iService.GetEmployeeDataAsync(id);

            if (data == null)
            {
                return NotFoundMsg();
            }

            var model = _iMapper.Map<EmployeeVm>(data);
            model.JoinDateStr = model.JoinDate.ToString("dd/MM/yyyy");

            var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoUrl);
            ViewBag.Photo = photoDoc;

            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Disable(EmployeeVm modelVm)
    {
        try
        {
            var data = await _iService.GetEmployeeDataAsync(modelVm.Id);

            if (data == null)
            {
                return NotFoundMsg();
            }

            var model = _iMapper.Map<Employee>(data);

            model.RetirementDate = !string.IsNullOrEmpty(modelVm.RetirementDateStr) ? DU.Utility.ConvertStrToDate(modelVm.RetirementDateStr) : modelVm.RetirementDate;
            model.DisableDate = !string.IsNullOrEmpty(modelVm.DisableDateStr) ? DU.Utility.ConvertStrToDate(modelVm.DisableDateStr) : modelVm.DisableDate;
            model.DisableReason = modelVm.DisableReason;
            model.UpdatedById = UserId;
            model.UpdateDate = DU.Utility.GetBdDateTimeNow();
            model.IsEnable = false;

            var isAdded = await _iService.UpdateAsync(model);
            if (!isAdded)
            {
                UpdateFailedMsg();
                return View("Disable", modelVm);
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

    #region Detail
    [Authorize(Permissions.Employees.DetailsView)]
    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var model = await _iService.GetEmployeeDataAsync(id);
            model.EmpRefTypeLookUp = _dropdownService.GetEmpRefTypeSelectListItems();
            model.DesignationLookUp = _dropdownService.GetDesignationSelectListItems();
            model.DepartmentLookUp = _dropdownService.GetNonAcademicDepartmentSelectListItems();
            model.PostingTypeLookUp = _dropdownService.GetPostingTypeSelectListItems();
            model.ActionTypeLookUp = _dropdownService.GetEmployeeActionTypeSelectListItems();
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

    #region JsonData

    public async Task<ActionResult> GetEmployeeById(long id)
    {
        try
        {
            if (id == 0) return BadRequest();
            var model = await _iService.GetByIdAsync(id);
            if (model == null) return NotFound();
            return Ok(model);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    #endregion

    #region Print

    [Authorize(Permissions.Employees.ReportView)]
    public async Task<ActionResult> EmployeeListPrint(long? desId, long? dptId, short enstatus)
    {
        var searchVm = new DataTablePagination<EmployeeSearchVm, EmployeeSearchVm>();
        searchVm.SearchModel = new EmployeeSearchVm();

        searchVm.SearchModel.DepartmentId = dptId;
        searchVm.SearchModel.DesignationId = desId;
        searchVm.SearchModel.EnableStatus = enstatus;

        var html = await _iService.EmployeeListPrintHtml(searchVm);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Employee List Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Employee List Report_" + DateTime.Now.ToString("dd/MM/yyyy");

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");
    }

    [Authorize(Permissions.Employees.ReportView)]
    public async Task<ActionResult> EmployeeCVPrint(long id, short refType)
    {
        string html = await _iService.EmployeeCVPrintHtml(id, refType);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Employee Information";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Employee Information Details" + DateTime.Now.ToString("dd/MM/yyyy");


        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");

    }

    #endregion

    #region ExcelFileDownload
    [Authorize(Permissions.Employees.ReportView)]
    public async Task<ActionResult> ExcelFileDownload(long? desId, long? dptId)
    {
        var searchVm = new DataTablePagination<EmployeeSearchVm, EmployeeSearchVm>();
        searchVm.SearchModel = new EmployeeSearchVm();
        searchVm.SearchModel.DepartmentId = dptId;
        searchVm.SearchModel.DesignationId = desId;

        byte[] fileContents = await _iService.ExcelFileDownload(searchVm);

        return File(
               fileContents,
               "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "EmployeeList.xlsx"
               );
    }
    #endregion
}
