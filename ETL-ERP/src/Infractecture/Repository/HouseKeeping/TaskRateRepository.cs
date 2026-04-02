using AutoMapper;
using Domain.Entities.HouseKeeping;
using Interface.Repository.HouseKeeping;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HouseKeeping;

public class TaskRateRepository : BaseRepository<HkTaskRate>, ITaskRateRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public TaskRateRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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
