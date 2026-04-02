using AutoMapper;
using Domain.Entities.HR;
using Domain.ViewModel.Hr.HrSetting;
using Interface.Services.Hr;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Hr
{
    public class HrSettingController : AppBaseController
    {
        #region Config

        private readonly IMapper _iMapper;
        private readonly IHrSettingService _iService;
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly DropdownService _dropdownService;

        public HrSettingController(IMapper iMapper, IHrSettingService iService, IUnitOfWork iUnitOfWork, DropdownService dropdownService) : base(iUnitOfWork)
        {
            _iMapper = iMapper;
            _iService = iService;
            _iUnitOfWork = iUnitOfWork;
            _dropdownService = dropdownService;
        }

        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Permissions.HrSettings.Edit)]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                var data = _iService.GetById(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<HrSettingVm>(data);
                model.HolidayLookUp = _dropdownService.GetHolidaySelectListItems();
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(HrSettingVm modelVm)
        {
            modelVm.HolidayLookUp = _dropdownService.GetHolidaySelectListItems();
            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<HrSetting>(modelVm);

                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
                var isAdded = await _iService.UpdateAsync(model);
                if (!isAdded)
                {
                    UpdateFailedMsg();
                    return View("Edit", modelVm);
                }
                UpdateSuccessMsg();
                return RedirectToAction("Details");
            }
            catch (Exception ex)
            {
                ExceptionMsg(ex.Message);
                return View(modelVm);
            }
        }
        #endregion

        #region Detail

        [Authorize(Permissions.HrSettings.DetailsView)]
        public async Task<ActionResult> Details()
        {
            try
            {
                var data = await _iService.GetFirstOrDefaultAsync();
                var model = _iMapper.Map<HrSettingVm>(data);
                model.HolidayLookUp = _dropdownService.GetHolidaySelectListItems();
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        #endregion
    }
}
