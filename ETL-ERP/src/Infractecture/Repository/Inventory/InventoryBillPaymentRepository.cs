using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.BillPayment;
using Interface.Repository.Inventory;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Inventory;

public class InventoryBillPaymentRepository : BaseRepository<InventoryBillPayment>, IInventoryBillPaymentRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public InventoryBillPaymentRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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

    #region Search
    public async Task<DataTablePagination<InventoryBillPaymentSearchVm, InventoryBillPaymentSearchVm>> SearchAsync(DataTablePagination<InventoryBillPaymentSearchVm, InventoryBillPaymentSearchVm> vm)
    {

        var searchResult = Context.InventoryBillPayments
            .Include(c => c.OrderMst)
            .Include(c => c.Supplier)
            .Include(c => c.BillBy)
            .AsQueryable().Where(c => !c.IsDeleted);

        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search Inventory Bill Payments not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.BillNo.ToLower().Contains(value));
        }

        if (model.OrderMstId > 0)
        {
            searchResult = searchResult.Where(x => x.OrderMstId == model.OrderMstId);
        }

        if (model.SupplierId > 0)
        {
            searchResult = searchResult.Where(x => x.SupplierId == model.SupplierId);
        }

        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderByDescending(c => c.ActionDate)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<InventoryBillPaymentSearchVm>>(data);


            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.BillDate = filterData.BillDate.ToShortDateString();
                searchDto.BillAmount = filterData.BillAmount;
                searchDto.OrderMstNo = filterData.OrderMst.OrderNo;
                searchDto.OrderDate = filterData.OrderMst.OrderDate.ToShortDateString();
                searchDto.SupplierName = filterData.Supplier.SupplierName;
                searchDto.BillByName = filterData.BillBy.FullName;
            }
        }
        return vm;
    }

    public async Task<InventoryBillPaymentDetails> GetBillPaymentByIdAsync(long id)
    {
        var dataModel = await Context.InventoryBillPayments
            .Include(c => c.OrderMst)
            .Include(c => c.Supplier)
            .Include(c => c.BillBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (dataModel == null)
            throw new Exception("Not Found..!");

        var model = new InventoryBillPaymentDetails();

        model.SupplierName = dataModel.Supplier.SupplierName;
        model.OrderNo = dataModel.OrderMst.OrderNo;
        model.OrderDateStr = dataModel.OrderMst.OrderDate.ToShortDateString();
        model.SupplierName = dataModel.Supplier.SupplierName ?? string.Empty;
        model.SupplierCode = dataModel.Supplier.SupplierCode;
        model.Mobile = dataModel.Supplier.Mobile;
        model.BillDateStr = dataModel.BillDate.ToShortDateString();
        model.BillByName = dataModel.BillBy.FullName;
        model.BillNo = dataModel.BillNo;
        model.BillAmount = dataModel.BillAmount;
        model.PayMode = (dataModel.PayMode == Domain.Enums.AppEnums.PayModeEnum.Cash) ? "Cash" : "Cheque";
        {
            if (dataModel.OrderMst.Status == 0) model.Status = "Fresh";
            else if (dataModel.OrderMst.Status == 1) model.Status = "Review";
            else if (dataModel.OrderMst.Status == 2) model.Status = "Rejected";
            else if (dataModel.OrderMst.Status == 3) model.Status = "Approved";
        }
        {
            if (dataModel.OrderMst.ReceiveStatus == 0) model.ReceiveStatus = "Not Received";
            else if (dataModel.OrderMst.ReceiveStatus == 1) model.ReceiveStatus = "Partial Received";
            else if (dataModel.OrderMst.ReceiveStatus == 2) model.ReceiveStatus = "Full Received";
            else if (dataModel.OrderMst.ReceiveStatus == 3) model.ReceiveStatus = "Fource Full Received";
        }

        return model;

    }
    #endregion
}
