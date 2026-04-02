using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Notification;

public class NtfUserSettingVm
{
    public long Id { get; set; }
    public bool IsEnable { get; set; }
    public bool IsEmail { get; set; }
    public bool IsSms { get; set; }
    public long EventId { get; set; }
    public string EventName { get; set; }
    public string EventCode { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; }
    public IEnumerable<SelectListItem> EventLookUp { get; set; }
    public IEnumerable<SelectListItem> UserLookUp { get; set; }
}

public class NtfUserSettingSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public bool IsEnable { get; set; }
    public bool IsEmail { get; set; }
    public bool IsSms { get; set; }
    public long EventId { get; set; }
    public string EventName { get; set; }
    public string EventCode { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}