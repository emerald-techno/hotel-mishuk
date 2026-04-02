using AutoMapper;
using Domain.Entities.Admin;
using Interface.Repository.Admin;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Admin
{
    public class SetCountryService : BaseService<SetCountry>, ISetCountryService
    {
        #region Config
        private ISetCountryRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public SetCountryService(ISetCountryRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion
    }
}
