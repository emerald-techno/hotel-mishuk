using AutoMapper;
using Domain.Entities.Inventory;
using Interface.Repository.Inventory;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Inventory
{
    public class OrderDtlService : BaseService<OrderDtl>, IOrderDtlService
    {
        #region Config

        private IOrderDtlRepository Repository { get; }
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public OrderDtlService(IOrderDtlRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }

        #endregion
    }
}
