namespace Domain.ViewModel.Notification;

public class NtfNotificationMsgVm
{
    public long Id { get; set; }
    public string NtfMsg { get; set; }
    public DateTime NtfTime { get; set; }
    public bool IsSeen { get; set; }
    public DateTime? SeenTime { get; set; }
    public long EventId { get; set; }
    public string EventName { get; set; }
    public string EventCode { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; }
    public bool IsDeleted { get; set; }
}
