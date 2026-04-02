using AutoMapper;
using Domain.Entities.HotelManagement;
using Interface.Repository.HotelManagement;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class RoomFacilityMapRepository : BaseRepository<HtRoomFacilityMap>, IRoomFacilityMapRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public RoomFacilityMapRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }
    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion
}