using AutoMapper;
using Domain.Entities.Accounting;
using Interface.Repository.Accounts;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Accounts
{
    public class AccTranNoteRepository : BaseRepository<AccTranNote>, IAccTranNoteRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public AccTranNoteRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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
