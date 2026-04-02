using AutoMapper;
using Domain.Entities.Notification;
using Domain.Utility;
using Domain.ViewModel.Notification;
using Interface.Repository.Notification;
using Interface.Services;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Microsoft.Extensions.Logging;
using Services.Base;
using DU = Domain.Utility;

namespace Services.Notification;

public class NtfNotificationMsgService : BaseService<NtfNotificationMsg>, INtfNotificationMsgService
{
    #region Config

    private INtfNotificationMsgRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly INtfEventInfoRepository _iNtfEventInfoRepository;
    private readonly INtfUserSettingsRepository _iNtfUserSettingRepository;
    private readonly IAppEmailSender _iAppEmailSender;
    private readonly IApplicationUserService _iAppUser;
    private readonly ILogger<NtfNotificationMsgService> _iLogger;

    public NtfNotificationMsgService(INtfNotificationMsgRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        INtfEventInfoRepository iNtfEventInfoRepository,
        INtfUserSettingsRepository iNtfUserSettingRepository,
        IAppEmailSender iAppEmailSender,
        IApplicationUserService iAppUser,
        ILogger<NtfNotificationMsgService> iLogger) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iNtfEventInfoRepository = iNtfEventInfoRepository;
        _iNtfUserSettingRepository = iNtfUserSettingRepository;
        _iAppEmailSender = iAppEmailSender;
        _iAppUser = iAppUser;
        _iLogger = iLogger;
    }

    #endregion

    #region GenerateNtf

    public async Task<bool> GenerateNtf(string eventCode, string ntfMsg, string emailMsg)
    {
        var eventInfo = await _iNtfEventInfoRepository.GetFirstOrDefaultAsync(x => x.EventCode == eventCode && !x.IsDeleted);
        if (eventInfo == null)
        {
            _iLogger.LogInformation($"No Notification Event Found With Code {eventCode}");
            return false;
        }

        var notificationList = new List<NtfNotificationMsg>();
        var eventUserList = await _iNtfUserSettingRepository.GetAsync(x => x.EventId == eventInfo.Id && x.IsEnable);
        if (eventUserList.Count > 0)
        {
            foreach (var user in eventUserList)
            {
                var model = new NtfNotificationMsg();
                model.EventId = eventInfo.Id;
                model.UserId = user.UserId;
                model.IsSeen = false;
                model.NtfMsg = ntfMsg;
                model.NtfTime = DateTime.Now;
                notificationList.Add(model);

                if (eventInfo.IsEmailNtf)
                {
                    var emailConfig = DU.Utility.AppSettings.SmtpConfig;

                    if (user.IsEmail)
                    {
                        var userInfo = await _iAppUser.GetByIdAsync(user.UserId);
                        if (userInfo != null)
                        {
                            try
                            {
                                (bool Success, string ErrorMsg) isSend = await _iAppEmailSender.SendEmailAsync(emailConfig.SmtpEmailSenderName, emailConfig.SmtpEmailAddress, userInfo.FullName, userInfo.Email, eventInfo.EventName, emailMsg, emailConfig, true);
                            }
                            catch { }
                        }
                    }
                }
            }
        }
        if (notificationList.Count > 0)
        {
            await Repository.AddRangeAsync(notificationList);
            var isAdded = await _iUnitOfWork.CompleteAsync();
            if (!isAdded) { return false; }
            return true;
        }
        else
        {
            return true;
        }
    }

    #endregion

    #region GetUserNotification

    public async Task<List<NtfNotificationMsgVm>> GetUserNotification(long userId)
    {
        if (userId == 0)
            throw new Exception("user information not found...");
        var userNotificationList = await Repository.GetAsync(x => x.UserId == userId, e => e.Event);
        var dataResult = _iMapper.Map<List<NtfNotificationMsgVm>>(userNotificationList);

        if (dataResult.Count > 0)
        {
            foreach (var model in dataResult)
            {
                var filterData = userNotificationList.FirstOrDefault(x => x.Id == model.Id);

                model.EventName = filterData.Event.EventName;
            }
        }

        dataResult = dataResult.OrderByDescending(x => x.NtfTime).ToList();

        return dataResult;
    }

    #endregion

    #region NotificationHtml

    public string GetNotificationMsgHtml(string msgText, string link)
    {
        var html = $@"<li class='noti-primary'>
                    <div class='media'><span class='notification-bg bg-light-primary'><i data-feather='check-circle'> </i></span>
                      <div class='media-body'>
                        <p>{msgText}</p><span>10 minutes ago</span>
                      </div>
                    </div>
                  </li>";

        return html;
    }

    #endregion

    #region MarkAllAsRead

    public async Task<bool> MarkAllAsRead(long userId)
    {
        if (userId == 0)
            throw new Exception("user information not found...");
        var userNotificationList = await Repository.GetAsync(x => x.UserId == userId);

        if (userNotificationList.Count == 0)
            return false;

        if (userNotificationList.Count > 0)
        {
            foreach (var model in userNotificationList)
            {
                model.IsSeen = true;
            }
        }

        await Repository.UpdateRangeAsync(userNotificationList);
        var isUpdated = await _iUnitOfWork.CompleteAsync();
        if (!isUpdated) { return false; }
        return true;
    }

    #endregion
}
