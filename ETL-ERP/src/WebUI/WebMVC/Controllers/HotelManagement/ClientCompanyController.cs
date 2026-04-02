using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.ClientCompany;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HotelManagement;

public class ClientCompanyController : BaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IClientCompanyService _iService;
    private readonly IMapper _iMapper;

    public ClientCompanyController(IUnitOfWork iUnitOfWork,
                                    IClientCompanyService iService,
                                    IMapper iMapper) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;

    }
    #endregion

    #region Create
    [HttpGet]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Create()
    {
        var model = new ClientCompanyVm();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ClientCompanyVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            var existingCompany = await _iService.GetAsync(x=>x.Name.ToLower() ==modelVm.Name.ToLower() && x.IsDeleted==false);

            if (existingCompany.Count()>0)
            {
                SaveFailedMsg("Client Company already Exists..!! ");
                return View("Create", modelVm);
            }

            var logoFile = modelVm.GetAppFileToUploadFolder();

            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddAsync(modelVm);

            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            await DU.Utility.UploadFileToFolderAsync(logoFile);

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
            var model = _iMapper.Map<ClientCompanyVm>(data);

            //var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoUrl);
            //ViewBag.Photo = photoDoc;

            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ClientCompanyVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);
            var existingCompany = await _iService.GetAsync(x=>x.Name.ToLower() == modelVm.Name.ToLower() && x.IsDeleted==false);

            if (existingCompany.Count()>0)
            {
                SaveFailedMsg("Client Company with the same name already exists...!!");
                return View("Create",modelVm);
            }

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var model = _iMapper.Map<ClientCompany>(modelVm);

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

    #region Search
    [HttpGet]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Search()
    {
        var vm = new ClientCompanySearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<ClientCompanySearchVm, ClientCompanySearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<ClientCompanySearchVm, ClientCompanySearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new ClientCompanySearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }
    #endregion

    #region Delete
    [HttpGet]
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
}
