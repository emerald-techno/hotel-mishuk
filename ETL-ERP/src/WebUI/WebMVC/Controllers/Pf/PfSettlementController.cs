using AutoMapper;
using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfSettlement;
using Interface.Services.Pf;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Pf
{
    public class PfSettlementController : AppBaseController
    {
        #region Config
        private readonly IUnitOfWork _iUnitWork;
        private readonly IPfSettlementService _iService;
        private readonly IMapper _iMapper;
        private readonly DropdownService _dropdownService;

        public PfSettlementController(IUnitOfWork iUnitOfWork,
                                IPfSettlementService iService,
                                IMapper iMapper, DropdownService dropdownService) : base(iUnitOfWork)
        {
            _iUnitWork = iUnitOfWork;
            _iService = iService;
            _iMapper = iMapper;
            _dropdownService = dropdownService;
        }
        #endregion

        #region Create

        [HttpGet]
        [Authorize(Permissions.PfSettlements.Create)]
        public ActionResult Create()
        {
            var model = new PfSettlementVm();
            model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
            model.PfSettlementStatusLookUp = _dropdownService.PfSettlementStatus();
            model.CompanyContributionTypeLookUp = _dropdownService.CompanyContributionType();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(PfSettlementVm modelVm)
        {
            modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
            modelVm.PfSettlementStatusLookUp = _dropdownService.PfSettlementStatus();
            modelVm.CompanyContributionTypeLookUp = _dropdownService.CompanyContributionType();
            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information is not correct");
                    return View("Report", modelVm);
                }

                _iService.CurrentUserId = UserId;
                var model = _iMapper.Map<PfSettlement>(modelVm);

                model.SettlementDate = (DateTime)(!string.IsNullOrEmpty(modelVm.SettlementDateStr) ? DU.Utility.ConvertStrToDate(modelVm.SettlementDateStr) : modelVm.SettlementDate);
                model.StatusDate = (DateTime)(!string.IsNullOrEmpty(modelVm.StatusDateStr) ? DU.Utility.ConvertStrToDate(modelVm.StatusDateStr) : modelVm.StatusDate);
                model.PfStartDate = (DateTime)(!string.IsNullOrEmpty(modelVm.PfStartDateStr) ? DU.Utility.ConvertStrToDate(modelVm.PfStartDateStr) : modelVm.PfStartDate);
                model.PfEndDate = (DateTime)(!string.IsNullOrEmpty(modelVm.PfEndDateStr) ? DU.Utility.ConvertStrToDate(modelVm.PfEndDateStr) : modelVm.PfEndDate);
                model.ApproveDate = (DateTime)(!string.IsNullOrEmpty(modelVm.ApproveDateStr) ? DU.Utility.ConvertStrToDate(modelVm.ApproveDateStr) : modelVm.ApproveDate);

                model.ActionById = UserId;
                model.ActionDate = DU.Utility.GetBdDateTimeNow();

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
        [Authorize(Permissions.PfSettlements.ListView)]
        public IActionResult Search()
        {
            var vm = new PfSettlementSearchVm();
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<PfSettlementSearchVm, PfSettlementSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<PfSettlementSearchVm, PfSettlementSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new PfSettlementSearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Permissions.PfSettlements.Edit)]
        public IActionResult Edit(long id)
        {
            try
            {
                var data = _iService.GetById(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<PfSettlementVm>(data);
                model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
                model.PfSettlementStatusLookUp = _dropdownService.PfSettlementStatus();
                model.CompanyContributionTypeLookUp = _dropdownService.CompanyContributionType();
                model.SettlementDateStr = model.SettlementDate.ToString("d");
                model.StatusDateStr = model.StatusDate.ToString("d");
                model.PfStartDateStr = model.PfStartDate.ToString("d");
                model.PfEndDateStr = model.PfEndDate.ToString("d");
                model.ApproveDateStr = model.ApproveDate.ToString("d");

                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PfSettlementVm modelVm)
        {
            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<PfSettlement>(modelVm);

                model.SettlementDate = (DateTime)(!string.IsNullOrEmpty(modelVm.SettlementDateStr) ? DU.Utility.ConvertStrToDate(modelVm.SettlementDateStr) : modelVm.SettlementDate);
                model.StatusDate = (DateTime)(!string.IsNullOrEmpty(modelVm.StatusDateStr) ? DU.Utility.ConvertStrToDate(modelVm.StatusDateStr) : modelVm.StatusDate);
                model.PfStartDate = (DateTime)(!string.IsNullOrEmpty(modelVm.PfStartDateStr) ? DU.Utility.ConvertStrToDate(modelVm.PfStartDateStr) : modelVm.PfStartDate);
                model.PfEndDate = (DateTime)(!string.IsNullOrEmpty(modelVm.PfEndDateStr) ? DU.Utility.ConvertStrToDate(modelVm.PfEndDateStr) : modelVm.PfEndDate);
                model.ApproveDate = (DateTime)(!string.IsNullOrEmpty(modelVm.ApproveDateStr) ? DU.Utility.ConvertStrToDate(modelVm.ApproveDateStr) : modelVm.ApproveDate);

                modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
                modelVm.PfSettlementStatusLookUp = _dropdownService.PfSettlementStatus();
                modelVm.CompanyContributionTypeLookUp = _dropdownService.CompanyContributionType();

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
    }
}
