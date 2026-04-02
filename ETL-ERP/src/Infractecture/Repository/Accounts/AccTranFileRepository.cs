using AutoMapper;
using Domain.Entities.Accounting;
using Interface.Repository.Accounts;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Accounts
{
    public class AccTranFileRepository : BaseRepository<AccTranFile>, IAccTranFileRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public AccTranFileRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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
}
