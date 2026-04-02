using Domain.Entities.Identity;

namespace Domain.Entities.Notification;

public class NtfUserSettings
{
    public long Id { get; set; }
    public DateTime SettingDate { get; set; }
    public bool IsEnable { get; set; }
    public bool IsEmail { get; set; }
    public bool IsSms { get; set; }
    //------------------------------------
    public long EventId { get; set; }
    public NtfEventInfo Event { get; set; }

    public long UserId { get; set; }
    public ApplicationUser User { get; set; }
}
