using AutoMapper;
using Domain.Entities.Inventory;
using Interface.Repository.Inventory;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Inventory
{
    public class RequsitionInfoDtlRepository : BaseRepository<RequsitionInfoDtl>, IRequsitionInfoDtlRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public RequsitionInfoDtlRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }
        #endregion

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
