using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.Transaction;
using Interface.Services.Inventory;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Inventory;

public class IssueController : AppBaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitWork;
    private readonly ITranService _iService;
    private readonly DropdownService _dropdownService;
    private readonly IMapper _iMapper;
    private IHttpContextAccessor _iHttpContextAccessor;
    private readonly INtfNotificationMsgService _iNtfMsgService;

    public IssueController(IUnitOfWork iUnitOfWork,
                            ITranService iService,
                            DropdownService dropdownService,
                            IMapper iMapper,
                            IHttpContextAccessor iHttpContextAccessor,
                            INtfNotificationMsgService iNtfMsgService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _dropdownService = dropdownService;
        _iMapper = iMapper;
        _iHttpContextAccessor = iHttpContextAccessor;
        _iNtfMsgService = iNtfMsgService;
    }

    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.Issue.Create)]
    public async Task<IActionResult> Create()
    {
        var vm = new TransactionVm();
        vm.ReqLookUp = _dropdownService.GetDefaultSelectListItem();
        vm.IssueDptLookUp = _dropdownService.GetOfficialDepartmentSelectListItems();
        vm.IssueEmpLookUp = _dropdownService.GetEmployeeSelectListItems();
        vm.TranType = TransType.Issue;
        vm.TranDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        vm.TranNo = await _iService.GetTransAutoCode(vm.TranType);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TransactionVm modelVm)
    {
        try
        {
            modelVm.ReqLookUp = _dropdownService.GetDefaultSelectListItem();
            modelVm.IssueDptLookUp = _dropdownService.GetOfficialDepartmentSelectListItems();
            modelVm.IssueEmpLookUp = _dropdownService.GetEmployeeSelectListItems();

            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.IssueEntryAsync(modelVm);

            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            var actionUrl = Url.Action("Details", "Issue", new { id = modelVm.Id }, Request.Scheme);

            string ntfMsgHtml = @$"<a href='{actionUrl}' target='_blank'><b>{modelVm.TranNo}-({modelVm.TranDate.ToString("dd/MM/yyyy")}) Receive Items</b></a>";
            string emailMsg = $@"{modelVm.TranNo}-({modelVm.TranDate.ToString("dd/MM/yyyy")}) Receive Items";

            var ntfGenerated = await _iNtfMsgService.GenerateNtf(NotificationEventCode.ReceiveItemNtf, ntfMsgHtml, emailMsg);

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
    [Authorize(Permissions.Issue.ListView)]
    public IActionResult Search()
    {
        var vm = new TransactionSearchVm()
        {
            IssueDptLookup = _dropdownService.GetOfficialDepartmentSelectListItems(),
            IssueEmpLookup = _dropdownService.GetEmployeeIsEnableSelectListItems(),
            ReqLookup = _dropdownService.GetApproveRequsitionSeletListItems(),
            TranType = TransType.Issue,
        };
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<TransactionSearchVm, TransactionSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<TransactionSearchVm, TransactionSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new TransactionSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Detail
    [Authorize(Permissions.Issue.Detail)]
    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var model = await _iService.GetTransInfoDataAsync(id);
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }

    }

    #endregion

    #region IssuePrint

    public async Task<ActionResult> IssuePrint(long id)
    {
        string html = await _iService.GetIssueByIdAsyncHtml(id);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "";
        reportTitle.IsSignature = true;
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Store Manager");
        reportTitle.SignatureList.Add("Authorized Signature");

        string reportName = "Receive" + DateTime.Today.ToString("dd_mm_yyy");

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false, withFooter: true), "application/pdf");
    }

    #endregion
}
