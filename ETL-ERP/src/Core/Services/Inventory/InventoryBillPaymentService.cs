using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.BillPayment;
using Interface.Repository.Inventory;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Inventory;

public class InventoryBillPaymentService : BaseService<InventoryBillPayment>, IInventoryBillPaymentService
{
    #region Config
    private IInventoryBillPaymentRepository _iRepository { get; }
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public InventoryBillPaymentService(IInventoryBillPaymentRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    public async Task<DataTablePagination<InventoryBillPaymentSearchVm, InventoryBillPaymentSearchVm>> SearchAsync(DataTablePagination<InventoryBillPaymentSearchVm, InventoryBillPaymentSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    public async Task<InventoryBillPaymentDetails> GetBillPaymentDataAsync(long id)
    {
        var data = await _iRepository.GetBillPaymentByIdAsync(id);
        return data;
    }
}
