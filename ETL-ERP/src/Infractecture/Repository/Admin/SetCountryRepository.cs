using AutoMapper;
using Domain.Entities.Admin;
using Interface.Repository.Admin;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Admin
{
    public class SetCountryRepository : BaseRepository<SetCountry>, ISetCountryRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public SetCountryRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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
