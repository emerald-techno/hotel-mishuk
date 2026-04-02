using AutoMapper;
using Domain.Entities.HR;
using Interface.Repository.Hr;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Hr
{
    public class EmpReferenceRepository : BaseRepository<EmpReference>, 
        IEmpReferenceRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public EmpReferenceRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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
