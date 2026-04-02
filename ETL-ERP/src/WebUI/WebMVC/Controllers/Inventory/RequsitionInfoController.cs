using AutoMapper;
using Domain.Entities.HR;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.RequsitionInfo;
using Interface.Services.Hr;
using Interface.Services.Inventory;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Inventory;

public class RequsitionInfoController : AppBaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitWork;
    private readonly IRequsitionInfoService _iService;
    private readonly DropdownService _dropdownService;
    private readonly IMapper _iMapper;
    private readonly IEmployeeService _iEmployeeService;
    private readonly INtfNotificationMsgService _iNtfNotificationMsgService;
    private IHttpContextAccessor _iHttpContextAccessor;

    public RequsitionInfoController(IUnitOfWork iUnitOfWork,
                            IRequsitionInfoService iService,
                            DropdownService dropdownService,
                            IMapper iMapper,
                            IEmployeeService iEmployeeService,
                            IHttpContextAccessor iHttpContextAccessor,
                            INtfNotificationMsgService iNtfNotificationMsgService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _dropdownService = dropdownService;
        _iMapper = iMapper;
        _iEmployeeService = iEmployeeService;
        _iHttpContextAccessor = iHttpContextAccessor;
        _iNtfNotificationMsgService = iNtfNotificationMsgService;
    }

    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.Requisition.Create)]
    public async Task<IActionResult> Create()
    {
        var model = new RequsitionInfoVm();
        model.ReqDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.ReqNo = await _iService.GetRequsitionInfoNo();
        model.DepartmentLookUp = _dropdownService.GetOfficialDepartmentSelectListItems();
        model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        model.PriorityLookUp = _dropdownService.GetReqPrioritySelectListItems(false);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RequsitionInfoVm modelVm)
    {

        modelVm.DepartmentLookUp = _dropdownService.GetOfficialDepartmentSelectListItems();
        modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        modelVm.PriorityLookUp = _dropdownService.GetReqPrioritySelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("Create", modelVm);
            }

            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddAsync(modelVm);

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
    [Authorize(Permissions.Requisition.ListView)]
    public IActionResult Search()
    {
        var vm = new RequsitionInfoSearchVm();
        vm.DepartmentLookUp = _dropdownService.GetOfficialDepartmentSelectListItems();
        vm.RequsitionByLookUp = _dropdownService.GetEmployeeSelectListItems();
        vm.PriorityLookUp = _dropdownService.GetReqPrioritySelectListItems();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<RequsitionInfoSearchVm, RequsitionInfoSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<RequsitionInfoSearchVm, RequsitionInfoSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new RequsitionInfoSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Edit
    public async Task<IActionResult> Edit(long id)
    {
        try
        {
            var modelVm = await _iService.GetRequsitionInfoDataAsync(id);
            modelVm.ReqDateStr = DU.Utility.ConvertDateToStr(modelVm.ReqDate);
            modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
            modelVm.DepartmentLookUp = _dropdownService.GetDepartmentSelectListItems();
            modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
            modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
            modelVm.PriorityLookUp = _dropdownService.GetReqPrioritySelectListItems(false);
            return View(modelVm);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public IActionResult Edit(RequsitionInfoVm modelVm)
    {
        modelVm.DepartmentLookUp = _dropdownService.GetDepartmentSelectListItems();
        modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        modelVm.PriorityLookUp = _dropdownService.GetReqPrioritySelectListItems(false);

        try
        {
            if (!ModelState.IsValid)
            {
                return View(modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = _iService.AddOrUpdate(modelVm);
            return RedirectToAction("Details", new { id = modelVm.Id });
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View(modelVm);
        }
    }


    #endregion

    #region Detail
    [Authorize(Permissions.Requisition.Detail)]
    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var model = await _iService.GetRequsitionInfoDataAsync(id);
            var vm = _iMapper.Map<RequsitionInfoVm>(model);
            return View(vm);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region PartialLoad

    [HttpPost]

    public async Task<PartialViewResult> GetDetailPartial(long id)
    {
        var model = await _iService.GetRequsitionInfoDataAsync(id);
        var vm = _iMapper.Map<RequsitionInfoVm>(model);
        return model != null ? PartialView("PartialView/RequsitionInfo/_RequsitionInfoDetails", vm) : PartialView("_404");
    }
    public async Task<PartialViewResult> GetDetailPartialAsReviewer(long id)
    {
        var model = await _iService.GetRequsitionInfoDataAsync(id);
        var vm = _iMapper.Map<RequsitionInfoVm>(model);
        return model != null ? PartialView("PartialView/RequsitionInfo/_RequsitionInfoDetailsAsReviewer", vm) : PartialView("_404");
    }
    #endregion

    #region Json Data

    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _iService.GetRequsitionInfoDataAsync(id);
            return Ok(data);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }


    public IActionResult GetItemsByRequsitionId(long id)
    {
        try
        {
            var data = _iService.GetItemsByRequsitionId(id);
            return Ok(data);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }

    public IActionResult GetRequisitionByDptId(long dptId)
    {
        var data = _iService.GetRequsitionByDptId(dptId);
        return Ok(data);
    }

    public IActionResult GetRequisitionByEmpId(long empId)
    {
        var data = _iService.GetRequsitionByEmpId(empId);
        return Ok(data);
    }

    public async Task<IActionResult> GetIssueItemsByReqId(long reqId)
    {
        try
        {
            var data = await _iService.GetIssueItemByReqId(reqId);
            return Ok(data);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }

    #endregion

    #region ApprovalProcess

    [HttpGet]
    public IActionResult ApprovalSearch()
    {
        var vm = new RequsitionInfoSearchVm();
        vm.DepartmentLookUp = _dropdownService.GetDepartmentSelectListItems();
        vm.RequsitionByLookUp = _dropdownService.GetUserSelectListItems();
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> SubmitReview(long reqId)
    {
        try
        {
            var requsition = _iService.GetById(reqId);

            if (requsition == null)
            {
                SaveFailedMsg("Requsition Not Found !");
                return RedirectToAction("Details", new { id = reqId });
            }

            if(!(requsition.DeptId > 0) && !(requsition.ReqById > 0))
            {
                SaveFailedMsg("No Department Or Employee Found For Requsition...!!!");
                return RedirectToAction("Details", new { id = reqId });
            }

            requsition.Status = (short)RequisitionStatusEnum.REVIEW;

            var isUpdate = _iService.Update(requsition);

            if (!isUpdate)
            {
                SaveFailedMsg("Can't Submit For Review");
                return RedirectToAction("Details", new { id = reqId });
            }

            var actionUrl = Url.Action("ReviewRequsition", "RequsitionInfo", new { id = requsition.Id }, Request.Scheme);

            string ntfMsgHtml = @$"<a href='{actionUrl}' target='_blank'><b>Requsition {requsition.ReqNo}-({requsition.ReqDate.ToString("dd/MM/yyyy")}) Submited For Review..</b></a>";

            string emailMsg = $@"Requsition {requsition.ReqNo}-({requsition.ReqDate.ToString("dd/MM/yyyy")}) Submited For Review..";

            var ntfGenerated = await _iNtfNotificationMsgService.GenerateNtf(NotificationEventCode.RequsitionReviewNtf, ntfMsgHtml, emailMsg);

            SaveSuccessMsg("Requsition has been submitted for Review");
            return RedirectToAction("Details", new { id = reqId });

        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return RedirectToAction("Details", new { id = reqId });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ReviewRequsition(long id)
    {
        try
        {
            var modelVm = await _iService.GetRequsitionInfoDataAsync(id);

            modelVm.ReqDateStr = DU.Utility.ConvertDateToStr(modelVm.ReqDate);

            return View(modelVm);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ApproveRequsition(RequsitionApprovalVm modelVm)
    {
        modelVm.Status = (short)RequisitionStatusEnum.APPROVED;
        _iService.CurrentUserId = UserId;
        var actionUrl = Url.Action("Details", "RequsitionInfo", new { id = modelVm.Id }, Request.Scheme);
        var isUpdated = await _iService.ReviewUpdate(modelVm, actionUrl);
        return Ok(isUpdated);
    }

    [HttpPost]
    public IActionResult RejectRequsition(long reqId)
    {
        var requsition = _iService.GetById(reqId);

        if (requsition == null) return Ok(false);

        requsition.Status = (short)RequisitionStatusEnum.REJECTED;

        var isUpdated = _iService.Update(requsition);

        return Ok(isUpdated);
    }

    #endregion

    #region DepartmentReq

    [HttpGet]
    [Authorize(Permissions.Requisition.DepartmentWiseCreate)]
    public async Task<IActionResult> DepartmentWiseCreate()
    {
        var model = new RequsitionInfoVm();
        model.ReqDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.ReqNo = await _iService.GetRequsitionInfoNo();
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        model.PriorityLookUp = _dropdownService.GetReqPrioritySelectListItems(false);

        var employeeInfo = await GetUserEmployeeInfo();
        if (employeeInfo == null)
        {
            SaveFailedMsg("Employee Information Is Not Correct...!");
            return RedirectToAction("Index", "Home");
        }

        model.DeptId = employeeInfo.DepartmentId;
        model.Departement = employeeInfo.Department.Name;
        model.EmployeeLookUp = _dropdownService.GetDptWiseEmployeeSelectListItems((long)model.DeptId);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DepartmentWiseCreate(RequsitionInfoVm modelVm)
    {

        modelVm.DepartmentLookUp = _dropdownService.GetOfficialDepartmentSelectListItems();
        modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        modelVm.PriorityLookUp = _dropdownService.GetReqPrioritySelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("DepartmentWiseCreate", modelVm);
            }

            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddAsync(modelVm);

            if (!isAdded)
            {
                SaveFailedMsg();
                return View("DepartmentWiseCreate", modelVm);
            }

            SaveSuccessMsg();

            return RedirectToAction("DepartmentWiseCreate");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("DepartmentWiseCreate", modelVm);
        }
    }

    [HttpGet]
    [Authorize(Permissions.Requisition.DepartmentWiseList)]
    public async Task<IActionResult> DepartmentWiseSearch()
    {
        var vm = new RequsitionInfoSearchVm();
        vm.RequsitionByLookUp = _dropdownService.GetEmployeeSelectListItems();
        vm.PriorityLookUp = _dropdownService.GetReqPrioritySelectListItems();

        var employeeInfo = await GetUserEmployeeInfo();
        if (employeeInfo == null)
        {
            SaveFailedMsg("Employee Information Is Not Correct...!");
            return RedirectToAction("Index", "Home");
        }

        vm.DeptId = employeeInfo.DepartmentId;
        vm.DeptName = employeeInfo.Department.Name;

        return View(vm);
    }

    #endregion

    private async Task<Employee?> GetUserEmployeeInfo()
    {
        var employee = await _iEmployeeService.GetFirstOrDefaultAsync(x => x.UserId == UserId, x => x.Department);
        if (employee == null)
            return null;

        return employee;
    }

    #region RequsitionPrint

    public async Task<ActionResult> RequsitionPrint(long id)
    {
        string html = await _iService.GetRequsitionByIdAsyncHtml(id);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "";
        reportTitle.IsSignature = true;
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Store Manager");
        reportTitle.SignatureList.Add("Authorized Signature");

        string reportName = "Requsition" + DateTime.Today.ToString("dd_mm_yyy");

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false, withFooter: true), "application/pdf");
    }

    #endregion
}
