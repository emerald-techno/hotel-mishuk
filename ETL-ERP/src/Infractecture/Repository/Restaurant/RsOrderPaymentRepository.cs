using AutoMapper;
using Domain.Entities.HotelManagement;
using Interface.Repository.Restaurant;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Restaurant;

public class RsOrderPaymentRepository : BaseRepository<RsOrderPayments>, IRsOrderPaymentRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public RsOrderPaymentRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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
