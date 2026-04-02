using AutoMapper;
using Domain.Entities.Pf;
using Interface.Repository.Pf;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Pf
{
    public class EmpLoanDtlRepository : BaseRepository<EmpLoanDtl>, IEmpLoanDtlRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public EmpLoanDtlRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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
