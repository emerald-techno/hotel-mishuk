using AutoMapper;
using Domain.Entities.HotelManagement;
using Interface.Repository.Restaurant;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Restaurant;

public class CustomerTypeRepository : BaseRepository<RsCustomerType>, ICustomerTypeRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public CustomerTypeRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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
