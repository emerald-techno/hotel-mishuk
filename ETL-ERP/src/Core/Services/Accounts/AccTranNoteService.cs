using AutoMapper;
using Domain.Entities.Accounting;
using Interface.Repository.Accounts;
using Interface.Services.Accounts;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Accounts
{
    public class AccTranNoteService : BaseService<AccTranNote>, IAccTranNoteService
    {
        #region Config
        private IAccTranNoteRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public AccTranNoteService(IAccTranNoteRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion
    }
}
