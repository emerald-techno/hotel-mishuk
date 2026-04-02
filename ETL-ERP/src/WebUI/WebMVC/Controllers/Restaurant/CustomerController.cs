using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Customer;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HotelManagement;

public class CustomerController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly ICustomerService _iService;
    private readonly DropdownService _dropdownService;
    private readonly IMapper _iMapper;

    public CustomerController(IUnitOfWork iUnitOfWork,
                            ICustomerService iService,
                            DropdownService dropdownService,
                            IMapper iMapper) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _dropdownService = dropdownService;
        _iMapper = iMapper;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.Customers.Create)]
    [Authorize(Permissions.Module.RestaurantModule)]

    public IActionResult Create()
    {
        var model = new CustomerVm();

        model.GenderLookUp = _dropdownService.GetGenderSelectListItems();
        model.SalutationLookUp = _dropdownService.GetSalutationSelectListItems();
        model.GuestLookUp = _dropdownService.GetGuestSelectListItems();
        model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        model.CompanyLookUp = _dropdownService.GetClientCompanySelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CustomerVm modelVm)
    {
        modelVm.GenderLookUp = _dropdownService.GetGenderSelectListItems();
        modelVm.SalutationLookUp = _dropdownService.GetSalutationSelectListItems();
        modelVm.GuestLookUp = _dropdownService.GetGuestSelectListItems();
        modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        modelVm.CompanyLookUp = _dropdownService.GetClientCompanySelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }

            _iService.CurrentUserId = UserId;

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var isAdded = await _iService.CustomerEntry(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            await DU.Utility.UploadFileToFolderAsync(photoFile);

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
    //[Authorize(Permissions.Customers.Edit)]
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
            var model = _iMapper.Map<CustomerVm>(data);

            var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoUrl);
            ViewBag.Photo = photoDoc;

            model.GenderLookUp = _dropdownService.GetGenderSelectListItems();
            model.SalutationLookUp = _dropdownService.GetSalutationSelectListItems();
            model.GuestLookUp = _dropdownService.GetGuestSelectListItems();
            model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
            model.CompanyLookUp = _dropdownService.GetClientCompanySelectListItems();
            model.CustomerCode = data.CustomerCode;
            model.Email = data.Email;
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CustomerVm modelVm)
    {
        modelVm.GenderLookUp = _dropdownService.GetGenderSelectListItems();
        modelVm.SalutationLookUp = _dropdownService.GetSalutationSelectListItems();
        modelVm.GuestLookUp = _dropdownService.GetGuestSelectListItems();
        modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        modelVm.CompanyLookUp = _dropdownService.GetClientCompanySelectListItems();

        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var model = _iMapper.Map<RsCustomer>(modelVm);
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
    //[Authorize(Permissions.Customers.Delete)]
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
    //[Authorize(Permissions.Customers.ListView)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult Search()
    {
        var vm = new CustomerSearchVm()
        {
            CompanyLookUp = _dropdownService.GetClientCompanySelectListItems()
        };
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<CustomerSearchVm, CustomerSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<CustomerSearchVm, CustomerSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new CustomerSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
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

    public async Task<IActionResult> GetCustomerInfoById(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            return BadRequest("Customer Info Not Found..!!!");
        }
        var model = _iMapper.Map<CustomerVm>(data);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult GetCustomerJsonData(string code)
    {
        var dataList = _dropdownService.GetRsCustomerDynamicData(code);
        return Ok(dataList);
    }

    public async Task<IActionResult> GetCustomerInfoByRoomId(long roomId)
    {
        var data = await _iService.GetCurrentGuestCustomerByRoomId(roomId);
        return Ok(data);
    }

    #endregion

    #region API

    [HttpPost]
    public async Task<IActionResult> CustomerEntry(CustomerVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Information is not correct");
            }

            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.CustomerEntry(modelVm);
            if (!isAdded)
            {
                return BadRequest("Customer info entry failed..!");
            }

            return Ok(true);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion
}
