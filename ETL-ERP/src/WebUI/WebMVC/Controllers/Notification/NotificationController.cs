using AutoMapper;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Controllers.Base;

namespace WebMVC.Controllers.Notification;

public class NotificationController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly INtfNotificationMsgService _iService;
    private readonly IMapper _iMapper;

    public NotificationController(IUnitOfWork iUnitOfWork,
                            INtfNotificationMsgService iService,
                            IMapper iMapper) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
    }
    #endregion

    #region ViewAll

    [HttpGet]
    public async Task<IActionResult> ViewAll()
    {
        var allNotification = await _iService.GetUserNotification(UserId);
        return View(allNotification);
    }

    #endregion

    #region Json Data

    public async Task<IActionResult> GetUserNotification()
    {
        try
        {
            var data = await _iService.GetUserNotification(UserId);
            return Ok(data);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }

    public async Task<IActionResult> GetUserUnSeenNotification()
    {
        try
        {
            var data = await _iService.GetUserNotification(UserId);
            data = data.Where(x => !x.IsSeen).ToList();
            return Ok(data);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }

    public async Task<IActionResult> GetUserLastNotification()
    {
        try
        {
            var data = await _iService.GetUserNotification(UserId);
            data = data.OrderByDescending(x => x.NtfTime).Take(10).ToList();
            return Ok(data);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }

    #endregion

    #region MarkAllAsRead
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.MarkAllAsRead(UserId);
            if (!isAdded)
            {
                SaveFailedMsg("Mark All As Read Failed...");
                return RedirectToAction("ViewAll");
            }

            SaveSuccessMsg("Mark All As Readed");
            return RedirectToAction("ViewAll");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("ViewAll");
        }
    }

    public async Task<IActionResult> MarkSeen()
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.MarkAllAsRead(UserId);
            return Ok(isAdded);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion
}
