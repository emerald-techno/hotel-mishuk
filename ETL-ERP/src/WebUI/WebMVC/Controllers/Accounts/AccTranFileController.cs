using AutoMapper;
using Domain.Entities.Accounting;
using Domain.ViewModel.Accounting.AccTranFiles;
using Interface.Services.Accounts;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Accounts
{
    public class AccTranFileController : AppBaseController
    {
        #region Config

        private readonly IUnitOfWork _iUnitWork;
        private readonly IMapper _iMapper;
        private readonly IAccTranFileService _iService;
        private readonly DropdownService _dropdownService;
        private readonly IWebHostEnvironment _iWebHostEnvironment;

        public AccTranFileController(IAccTranFileService iService,
                                IMapper iMapper,
                                IUnitOfWork iUnitOfWork,
                                DropdownService dropdownService,
                                IWebHostEnvironment iWebHostEnvironment) : base(iUnitOfWork)
        {
            _iService = iService;
            _iMapper = iMapper;
            _iUnitWork = iUnitOfWork;
            _dropdownService = dropdownService;
            _iWebHostEnvironment = iWebHostEnvironment;
        }

        #endregion

        #region Create

        [HttpGet]
        public ActionResult Create()
        {
            var model = new AccTranFileVm();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AccTranFileVm modelVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (modelVm.IsAjaxPost)
                    {
                        return Ok(false);
                    }

                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                };

                var photoFile = modelVm.GetAppFileToUploadFolder();

                var model = _iMapper.Map<AccTranFile>(modelVm);
                model.UploadById = UserId;
                model.UploadDate = DU.Utility.GetBdDateTimeNow();
                model.ActionById = UserId;
                model.ActionDate = DU.Utility.GetBdDateTimeNow();

                var isAdded = await _iService.AddAsync(model);

                if (modelVm.IsAjaxPost)
                {
                    await DU.Utility.UploadFileToFolderAsync(photoFile);
                    return Ok(isAdded);
                }

                if (!isAdded)
                {
                    SaveFailedMsg();
                    return View("Create", modelVm);
                }

                await DU.Utility.UploadFileToFolderAsync(photoFile);
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

        #region GetVoucher

        [HttpGet]
        public async Task<ActionResult> GetVcFileByVcId(long vcId)
        {
            try
            {
                var vcFiles = await _iService.GetAsync(c => c.TranMstId == vcId && !c.IsDeleted);
                return Ok(vcFiles.ToList());
            }
            catch (Exception ex)
            {
                return Ok(SetError(ex.Message));
            }
        }

        #endregion

        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var data = await _iService.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            data.IsDeleted = true;
            var isRemove = _iService.Update(data);
            return Ok(isRemove);
        }
        #endregion
    }
}
