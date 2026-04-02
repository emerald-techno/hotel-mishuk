using AutoMapper;
using Domain.Entities.Notification;
using Interface.Repository.Notification;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Notification;

public class NtfNotificationMsgRepository : BaseRepository<NtfNotificationMsg>, INtfNotificationMsgRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public NtfNotificationMsgRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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
