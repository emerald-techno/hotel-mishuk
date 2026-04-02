using AutoMapper;
using Domain.Entities.HotelManagement;
using Interface.Repository.Restaurant;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Restaurant;

public class FoodOrderItemRepository : BaseRepository<RsFoodOrderItem>, IFoodOrderItemRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public FoodOrderItemRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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