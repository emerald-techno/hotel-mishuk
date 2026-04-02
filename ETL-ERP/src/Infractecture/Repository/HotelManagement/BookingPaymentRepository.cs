using AutoMapper;
using Domain.Entities.HotelManagement;
using Interface.Repository.HotelManagement;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class BookingPaymentRepository : BaseRepository<HtBookingPayment>, IBookingPaymentRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public BookingPaymentRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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
