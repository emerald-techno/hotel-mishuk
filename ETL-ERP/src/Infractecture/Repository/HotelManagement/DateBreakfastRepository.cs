using AutoMapper;
using Domain.Entities.HotelManagement;
using Interface.Repository.HotelManagement;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class DateBreakfastRepository : BaseRepository<HtDateBreakfast>, IDateBreakfastRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public DateBreakfastRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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