using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.GuestInfo;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HotelManagement;

public class GuestInfoController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IGuestInfoService _iService;
    private readonly DropdownService _dropdownService;
    private readonly IMapper _iMapper;
    private readonly IHttpContextAccessor _iHttpContextAccessor;

    public GuestInfoController(IUnitOfWork iUnitOfWork,
                            IGuestInfoService iService,
                            DropdownService dropdownService,
                            IMapper iMapper,
                            IHttpContextAccessor iHttpContextAccessor) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _dropdownService = dropdownService;
        _iMapper = iMapper;
        _iHttpContextAccessor = iHttpContextAccessor;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.GuestInfos.Create)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Create()
    {
        var model = new HtGuestInfoVm();

        model.GenderLookUp = _dropdownService.GetGenderSelectListItems();
        model.SalutationLookUp = _dropdownService.GetSalutationSelectListItems();
        model.IdentityTypeLookUp = _dropdownService.GetIdentityTypeSelectListItems();
        model.CountryLookUp = _dropdownService.GetCountrySelectListItems();
        model.DistrictLookUp = _dropdownService.GetDistrictSelectListItems();
        model.ClientCompanyLookUp = _dropdownService.GetClientCompanySelectListItems();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(HtGuestInfoVm modelVm)
    {
        modelVm.GenderLookUp = _dropdownService.GetGenderSelectListItems();
        modelVm.SalutationLookUp = _dropdownService.GetSalutationSelectListItems();
        modelVm.IdentityTypeLookUp = _dropdownService.GetIdentityTypeSelectListItems();
        modelVm.CountryLookUp = _dropdownService.GetCountrySelectListItems();
        modelVm.DistrictLookUp = _dropdownService.GetDistrictSelectListItems();
        modelVm.ClientCompanyLookUp = _dropdownService.GetClientCompanySelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }

            _iService.CurrentUserId = UserId;

            var photoFile = modelVm.GetAppFileToUploadFolder();
            var identityPhotoFile = modelVm.GetIdentityFileToUploadFolder();

            var (isAdded, guestId) = await _iService.GuestEntry(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            await DU.Utility.UploadFileToFolderAsync(photoFile);
            await DU.Utility.UploadFileToFolderAsync(identityPhotoFile);

            SaveSuccessMsg();

            return View("Create", modelVm);
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
    //[Authorize(Permissions.GuestInfos.Edit)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<HtGuestInfoVm>(data);

            var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoUrl);
            ViewBag.Photo = photoDoc;

            model.GenderLookUp = _dropdownService.GetGenderSelectListItems();
            model.SalutationLookUp = _dropdownService.GetSalutationSelectListItems();
            model.IdentityTypeLookUp = _dropdownService.GetIdentityTypeSelectListItems();
            model.CountryLookUp = _dropdownService.GetCountrySelectListItems();
            model.DistrictLookUp = _dropdownService.GetDistrictSelectListItems();
            model.ClientCompanyLookUp = _dropdownService.GetClientCompanySelectListItems();

            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HtGuestInfoVm modelVm)
    {
        modelVm.GenderLookUp = _dropdownService.GetGenderSelectListItems();
        modelVm.SalutationLookUp = _dropdownService.GetSalutationSelectListItems();
        modelVm.IdentityTypeLookUp = _dropdownService.GetIdentityTypeSelectListItems();
        modelVm.CountryLookUp = _dropdownService.GetCountrySelectListItems();
        modelVm.DistrictLookUp = _dropdownService.GetDistrictSelectListItems();
        modelVm.ClientCompanyLookUp = _dropdownService.GetClientCompanySelectListItems();

        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var photoFile = modelVm.GetAppFileToUploadFolder();
            var identityPhotoFile = modelVm.GetIdentityFileToUploadFolder();

            var model = _iMapper.Map<HtGuestInfo>(modelVm);
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

            await DU.Utility.UploadFileToFolderAsync(photoFile);
            await DU.Utility.UploadFileToFolderAsync(identityPhotoFile);

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

    #region GuestUpdate WithCompany/ Edit V2
    [HttpPost]
    public async Task<IActionResult> GuestUpdate(HtGuestInfoVm modelVm)
    {
        modelVm.GenderLookUp = _dropdownService.GetGenderSelectListItems();
        modelVm.IdentityTypeLookUp = _dropdownService.GetIdentityTypeSelectListItems();
        modelVm.CountryLookUp = _dropdownService.GetCountrySelectListItems();
        modelVm.DistrictLookUp = _dropdownService.GetDistrictSelectListItems();
        modelVm.ClientCompanyLookUp = _dropdownService.GetClientCompanySelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Guest Update Failled.Please Provide Valid Information..!!");
            }

            _iService.CurrentUserId = UserId;

            var photoFile = modelVm.GetAppFileToUploadFolder();
            var identityPhotoFile = modelVm.GetIdentityFileToUploadFolder();

            var (isUpdated, guestId) = await _iService.GuestUpdateAsync(modelVm);
            if (!isUpdated)
            {
                return BadRequest(isUpdated);
            }

            await DU.Utility.UploadFileToFolderAsync(photoFile);
            await DU.Utility.UploadFileToFolderAsync(identityPhotoFile);

            return Ok(isUpdated);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion
    #region Delete
    [HttpGet]
    //[Authorize(Permissions.GuestInfos.Delete)]
    [Authorize(Permissions.Module.HotelManagementModule)]
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
    //[Authorize(Permissions.GuestInfos.ListView)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Search()
    {
        var vm = new HtGuestInfoSearchVm();

        vm.GenderLookUp = _dropdownService.GetGenderSelectListItems();
        vm.SalutationLookUp = _dropdownService.GetSalutationSelectListItems();
        vm.IdentityTypeLookUp = _dropdownService.GetIdentityTypeSelectListItems();
        vm.CountryLookUp = _dropdownService.GetCountrySelectListItems();
        vm.DistrictLookUp = _dropdownService.GetDistrictSelectListItems();

        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<HtGuestInfoSearchVm, HtGuestInfoSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<HtGuestInfoSearchVm, HtGuestInfoSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new HtGuestInfoSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Details
    //[Authorize(Permissions.GuestInfos.ListView)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> Details(long id)
    {
        var model = await _iService.GetGuestDetails(id);
        return View(model);
    }
    #endregion

    #region EXISTING CHECK

    [AcceptVerbs("Get", "Post")]
    public async Task<IActionResult> IsNumberExist(string number, string initName)
    {
        if (!string.IsNullOrEmpty(number) && !string.IsNullOrEmpty(initName) && number.ToUpper().Equals(initName.ToUpper()))
        {
            return Json(true);
        }

        var result = await _iService.GetFirstOrDefaultAsync(c => c.Mobile.Equals(number) && !c.IsDeleted);
        if (result == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Number {number} Is Already Exist..!!");
        }

    }
    #endregion

    #region JsonData

    public async Task<IActionResult> GetGuestInfoById(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            return BadRequest("Guest Info Not Found..!!!");
        }
        var model = _iMapper.Map<HtGuestInfoVm>(data);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult GetGuestJsonData()
    {
        var dataList = _dropdownService.GetGuestDynamicData();
        return Ok(dataList);
    }

    #endregion

    #region API

    [HttpPost]
    public async Task<IActionResult> GuestEntry(HtGuestInfoVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Information is not correct");
            }

            _iService.CurrentUserId = UserId;

            var (isAdded, guestId) = await _iService.GuestEntry(modelVm);
            if (!isAdded)
            {
                return BadRequest("Guest info entry failed..!");
            }

            return Ok(guestId);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region GuestPrint
    public async Task<ActionResult> GuestPrint(long id)
    {
        string html = await _iService.GuestPrintHtml(id);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Guest Information";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Guest Information Details" + DateTime.Now.ToString("dd/MM/yyyy");


        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");

    }
    #endregion
}
