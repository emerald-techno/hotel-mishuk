using Domain.Enums.AppEnums;
using Domain.ModelInterface;

namespace Domain.ViewModel.HouseKeeping.TaskAssign;

public class TaskAssignSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public DateTime AssignDate { get; set; }
    public DateTime? CompleteDate { get; set; }
    public string AssignRemarks { get; set; }
    public string CompleteRemarks { get; set; }
    public string AuditorRemarks { get; set; }
    public DateTime? AuditDate { get; set; }
    public long AssignId { get; set; }
    public RoomAssignEnum AssignStatus { get; set; }
    public string AssignRoomNo { get; set; }
    public string AssignKepperName { get; set; }
    public long TaskId { get; set; }
    public string TaskName { get; set; }
    public long? AuditorId { get; set; }
    public string AuditorName { get; set; }
    public int TaskCount { get; set; }
    public string RoomWiseTaskList { get; set; }

    public long? RoomId { get; set; }
    public long? HouseKeeperId { get; set; }
    public string ActionByName { get; set; }

    //DataTable
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
