using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HouseKeeping.TaskAssign;

public class TaskAssignVm
{
    public long Id { get; set; }
    public DateTime AssignDate { get; set; }
    public DateTime? CompleteDate { get; set; }
    public string AssignRemarks { get; set; }
    public string CompleteRemarks { get; set; }
    public string AuditorRemarks { get; set; }
    public DateTime? AuditDate { get; set; }

    // --- Fk ---

    public long AssignId { get; set; }
    public string AssignRoomNo { get; set; }
    public long TaskId { get; set; }
    public string TaskName { get; set; }
    public long? AuditorId { get; set; }
    public string AuditorName { get; set; }
}

public class TaskAssignSaveVm
{
    public string AssignRemarks { get; set; }
    public long AssignId { get; set; }
    public long TaskId { get; set; }
    public long? HouseKeeperId { get; set; }
    public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
    public ICollection<TaskAssignSaveVm> TaskAssignSaveVms { get; set; }
}