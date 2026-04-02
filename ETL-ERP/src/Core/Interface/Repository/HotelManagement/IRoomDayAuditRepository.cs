using Domain.Entities.HotelManagement;
using Domain.ViewModel.HotelManagement.RoomDayAudit;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IRoomDayAuditRepository : IRepository<HtRoomDayAudit>
{
    Task<List<RoomDayAuditVm>> GetRoomAuditByBusinessDate(DateTime businessDate);
    Task<AuditStatusVm> IsAudited(DateTime businessDate);
}
