using AutoMapper;
using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfFundOpenning;
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
    [Authorize]
    public class PfFundOpenningController : AppBaseController
    {
        #region Config

        private readonly IUnitOfWork _iUnitWork;
        private readonly IPfFundOpenningService _iService;
        private readonly IMapper _iMapper;
        private readonly DropdownService _dropdownService;

        public PfFundOpenningController(IUnitOfWork iUnitOfWork,
                                IPfFundOpenningService iService,
                                IMapper iMapper,
                                DropdownService dropdownService) : base(iUnitOfWork)
        {
            _iUnitWork = iUnitOfWork;
            _iService = iService;
            _iMapper = iMapper;
            _dropdownService = dropdownService;
        }

        #endregion

        #region Create

        [HttpGet]
        [Authorize(Permissions.PfFundOpennings.Create)]
        public IActionResult Create()
        {
            var model = new PfFundOpenningVm();
            model.EmployeeLookup = _dropdownService.GetEmployeeSelectListItems();

            return View(model);
        }
        [HttpPost]
        public IActionResult Create(PfFundOpenningVm modelVm)
        {
            modelVm.EmployeeLookup = _dropdownService.GetEmployeeSelectListItems();

            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }
                _iService.CurrentUserId = UserId;
                var model = _iMapper.Map<PfFundOpenning>(modelVm);
                model.PfStartDate = (DateTime)(!string.IsNullOrEmpty(modelVm.PfStartDateStr) ? DU.Utility.ConvertStrToDate(modelVm.PfStartDateStr) : model.PfStartDate);
                model.OpenningDate = DU.Utility.GetBdDateTimeNow();
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
        [Authorize(Permissions.PfFundOpennings.ListView)]
        public IActionResult Search()
        {
            var vm = new PfFundOpenningSearchVm();
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<PfFundOpenningSearchVm, PfFundOpenningSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<PfFundOpenningSearchVm, PfFundOpenningSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new PfFundOpenningSearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }

        #endregion
    }
}
