using AutoMapper;
using Domain.Entities.Pf;
using Domain.ViewModel.Pf.PfSetting;
using Interface.Services.Pf;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Pf
{
    public class PfSettingController : AppBaseController
    {
        #region Config

        private readonly IMapper _iMapper;
        private readonly IPfSettingService _iPfSettingService;
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly DropdownService _dropdownService;

        public PfSettingController(IMapper iMapper, IPfSettingService iPfSettingService, IUnitOfWork iUnitOfWork, DropdownService dropdownService) : base(iUnitOfWork)
        {
            _iMapper = iMapper;
            _iPfSettingService = iPfSettingService;
            _iUnitOfWork = iUnitOfWork;
            _dropdownService = dropdownService;
        }

        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Permissions.PfSettings.Edit)]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                var data = await _iPfSettingService.GetByIdAsync(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<PfSettingVm>(data);
                model.PfSourceLookup = _dropdownService.GetPfSourceSelectListItems();

                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PfSettingVm modelVm)
        {
            modelVm.PfSourceLookup = _dropdownService.GetPfSourceSelectListItems();

            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<PfSetting>(modelVm);


                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
                var isAdded = await _iPfSettingService.UpdateAsync(model);
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

        [Authorize(Permissions.PfSettings.DetailsView)]
        public async Task<ActionResult> Details()
        {
            try
            {
                var data = await _iPfSettingService.GetFirstOrDefaultAsync();
                var model = _iMapper.Map<PfSettingVm>(data);
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
