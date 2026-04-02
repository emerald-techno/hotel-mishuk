using AutoMapper;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.SetHoliday;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Admin
{
    public class SetHolidayController : AppBaseController
    {
        #region Config
        private readonly ISetHolidayService _iService;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly DropdownService _dropdownService;
        public SetHolidayController(ISetHolidayService iService, IMapper iMapper, IUnitOfWork iUnitOfWork, DropdownService dropdownService) : base(iUnitOfWork)
        {
            _iService = iService;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
            _dropdownService = dropdownService;
        }
        #endregion

        #region Create

        [HttpGet]
        [Authorize(Permissions.SetHolidays.Create)]
        public IActionResult Create()
        {
            var model = new SetHolidayVm();
            model.TypeLookUp = _dropdownService.GetTypeSelectListItems();
            model.YearLookUp = _dropdownService.GetYearSelectListItems();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(SetHolidayVm modelVm)
        {
            modelVm.TypeLookUp = _dropdownService.GetTypeSelectListItems();
            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }
                _iService.CurrentUserId = UserId;
                var model = _iMapper.Map<SetHoliday>(modelVm);

                model.StartDate = (DateTime)(!string.IsNullOrEmpty(modelVm.StartDateStr) ? DU.Utility.ConvertStrToDate(modelVm.StartDateStr) : modelVm.StartDate);
                model.EndDate = (DateTime)(!string.IsNullOrEmpty(modelVm.EndDateStr) ? DU.Utility.ConvertStrToDate(modelVm.EndDateStr) : null);
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
        [Authorize(Permissions.SetHolidays.ListView)]
        public IActionResult Search()
        {
            var vm = new SetHolidaySearchVm();
            vm.YearLookUp = _dropdownService.GetYearSelectListItems();
            vm.HolidayYear = DateTime.Now.Year;
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<SetHolidaySearchVm, SetHolidaySearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<SetHolidaySearchVm, SetHolidaySearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new SetHolidaySearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        #endregion

        #region Edit

        [HttpGet]
        [Authorize(Permissions.SetHolidays.Edit)]
        public IActionResult Edit(long id)
        {
            try
            {
                var data = _iService.GetById(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<SetHolidayVm>(data);
                model.StartDateStr = model.StartDate.ToString("dd/MM/yyyy");
                model.EndDateStr = model.EndDate.ToString("dd/MM/yyyy");
                model.TypeLookUp = _dropdownService.GetTypeSelectListItems();
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SetHolidayVm modelVm)
        {
            modelVm.TypeLookUp = _dropdownService.GetTypeSelectListItems();
            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<SetHoliday>(modelVm);
                model.StartDate = (DateTime)(!string.IsNullOrEmpty(modelVm.StartDateStr) ? DU.Utility.ConvertStrToDate(modelVm.StartDateStr) : modelVm.StartDate);
                model.EndDate = (DateTime)(!string.IsNullOrEmpty(modelVm.EndDateStr) ? DU.Utility.ConvertStrToDate(modelVm.EndDateStr) : modelVm.EndDate);

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

        #region Delete

        [HttpGet]
        [Authorize(Permissions.SetHolidays.Delete)]
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

        #region EXISTING CHECK

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsNameExist(string holidayName, int holidayYear, string initName)
        {
            if (!string.IsNullOrEmpty(holidayName) && !string.IsNullOrEmpty(initName) && holidayName.ToUpper().Equals(initName.ToUpper()))
            {
                return Json(true);
            }

            var result = await _iService.GetFirstOrDefaultAsync(c => c.HolidayName.Equals(holidayName) && c.HolidayYear == holidayYear && !c.IsDeleted);
            if (result == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Name {holidayName} Is Already Exist..!!");
            }

        }

        #endregion
    }
}
