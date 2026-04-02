using Domain.Entities.Notification;
using Domain.ViewModel.Notification;
using Interface.Base;

namespace Interface.Services.Notification;

public interface INtfNotificationMsgService : IService<NtfNotificationMsg>
{
    Task<bool> GenerateNtf(string eventCode, string ntfMsg, string emailMsg);
    Task<List<NtfNotificationMsgVm>> GetUserNotification(long userId);
    Task<bool> MarkAllAsRead(long userId);
}
