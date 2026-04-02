using System.ComponentModel.DataAnnotations;
using Domain.Entities.Identity;

namespace Domain.Entities.Notification;

public class NtfNotificationMsg
{
    public long Id { get; set; }

    [Required]
    [StringLength(1500)]
    public string NtfMsg { get; set; }
    public DateTime NtfTime { get; set; }
    public bool IsSeen { get; set; }
    public DateTime? SeenTime { get; set; }

    //--------------FK---------------------
    public long EventId { get; set; }
    public NtfEventInfo Event { get; set; }

    public long UserId { get; set; }
    public ApplicationUser User { get; set; }

    public bool IsDeleted { get; set; }
}
