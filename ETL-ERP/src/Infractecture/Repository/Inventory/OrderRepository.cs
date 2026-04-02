using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.BillPayment;
using Domain.ViewModel.Inventory.Order;
using Domain.ViewModel.Inventory.Receive;
using Domain.ViewModel.Report;
using Interface.Repository.Inventory;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.Inventory;

public class OrderRepository : BaseRepository<OrderMst>, IOrderRepository, IDisposable
{
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;
    private readonly IInventoryReportRepository _reportRepository;

    public OrderRepository(ApplicationDbContext db, IMapper iMapper, IInventoryReportRepository reportRepository) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
        _reportRepository = reportRepository;
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    public async Task<OrderVm> GetOrderByIdAsync(long id)
    {
        var dataModel = await Context.OrderMsts
            .Include(c => c.OrderDtls)
                .ThenInclude(c => c.Item)
            .Include(c => c.OrderDtls)
                .ThenInclude(c => c.ItemUnit)
            .Include(c => c.Supplier)
            .Include(c => c.Req)
            .Include(c => c.ActionBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (dataModel == null) throw new Exception("No Order Found..!");

        var model = _iMapper.Map<OrderVm>(dataModel);

        model.SupplierName = dataModel.Supplier?.SupplierName;
        model.SupplierMobile = dataModel.Supplier?.Mobile;
        model.SupplierAddress = dataModel.Supplier?.Address;
        model.RequsitionNo = dataModel.Req?.ReqNo;

        foreach (var data in model.OrderDtls)
        {
            var reqDtl = dataModel.OrderDtls.FirstOrDefault(c => c.Id == data.Id);
            if (reqDtl == null) continue;

            data.ItemName = reqDtl.Item?.ItemName;
            data.ItemUnitName = reqDtl.ItemUnit?.UnitName;
        }

        return model;
    }

    public async Task<List<ReceiveDtlVm>> GetOrderItemForReceive(long orderId)
    {
        var dataModel = await Context.OrderMsts
            .Include(c => c.OrderDtls)
                .ThenInclude(c => c.Item)
            .Include(c => c.OrderDtls)
                .ThenInclude(c => c.ItemUnit)
            .Include(c => c.Supplier)
            .Include(c => c.Req)
            .Include(c => c.ActionBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == orderId && !c.IsDeleted);

        if (dataModel == null) throw new Exception("No Order Found..!");

        var orderItems = dataModel.OrderDtls;

        var receivedItems = Context
                    .TranMsts
                    .Include(x => x.TranDtls)
                    .AsNoTracking()
                    .Where(c => c.OrderId == orderId && !c.IsDeleted)
                    .SelectMany(d => d.TranDtls);

        var receiveItems = new List<ReceiveDtlVm>();

        if (orderItems.Count > 0)
        {
            foreach (var item in orderItems)
            {
                var model = new ReceiveDtlVm();

                model.ItemId = item.ItemId;
                model.ItemName = item.Item.ItemName;
                model.ItemUnitId = item.ItemUnit.Id;
                model.ItemUnitName = item.ItemUnit.UnitName;
                model.OrderQty = item.AprOrderQty;
                model.UnitPrice = item.Rate;

                var currentStock = await _reportRepository.GetInventoryStockInfo(new StockVm { ItemId = item.ItemId });

                model.Stock = currentStock != null && currentStock.Count > 0 ? currentStock.FirstOrDefault().Stock : 0;
                model.ActualRcvQty = receivedItems.Where(c => c.ItemId == item.ItemId).Sum(x => x.ItemQty);

                receiveItems.Add(model);
            }
        }

        return receiveItems;
    }

    public async Task<DataTablePagination<OrderSearchVm, OrderSearchVm>>
        SearchAsync(DataTablePagination<OrderSearchVm, OrderSearchVm> vm)
    {
        var searchResult = Context.OrderMsts
            .Include(c => c.OrderDtls)
            .Include(c => c.Req)
            .Include(c => c.Supplier)
            .AsQueryable().Where(c => !c.IsDeleted);

        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search Requsition not found");

        if (model.SFromDate != null)
        {
            var fromDate = DU.Utility.ConvertStrToDate(model.SFromDate);
            searchResult = searchResult.Where(c => c.OrderDate >= fromDate);
        }
        if (model.SToDate != null)
        {
            var toDate = DU.Utility.ConvertStrToDate(model.SToDate);
            searchResult = searchResult.Where(c => c.OrderDate <= toDate);
        }

        if (model.SupplierId > 0)
        {
            searchResult = searchResult.Where(c => c.SupplierId == model.SupplierId);
        }

        if (model.ReqId > 0)
        {
            searchResult = searchResult.Where(c => c.ReqId == model.ReqId);
        }
        if (!string.IsNullOrEmpty(model.OrderType))
        {
            searchResult = searchResult.Where(c => c.OrderType == model.OrderType);
        }

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.OrderNo.ToLower().Contains(value));
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

            vm.data = _iMapper.Map<List<OrderSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.ReqNo = filterData.Req?.ReqNo;
                searchDto.ReqDate = filterData.Req?.ReqDate;
                searchDto.DeliveryDate = filterData?.DeliveryDeadline;
                searchDto.SupplierName = filterData.Supplier?.SupplierName;
            }
        }
        return vm;
    }

    public async Task<InvBillSaveVm> GenerateBill(long orderId)
    {
        var dataModel = await Context.OrderMsts
            .Include(c => c.Supplier)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == orderId && !c.IsDeleted);

        if (dataModel == null) throw new Exception("No Order Found..!");

        var receivedList = Context
                    .TranMsts
                    .Include(x => x.TranDtls)
                    .AsNoTracking()
                    .Where(c => c.OrderId == orderId && !c.IsDeleted)
                    .ToList();

        var bill = new InvBillSaveVm();

        bill.OrderMstId = dataModel.Id;
        bill.OrderMstNo = dataModel.OrderNo;
        bill.OrderDate = dataModel.OrderDate;
        bill.SupplierId = dataModel.SupplierId;
        bill.SupplierName = dataModel.Supplier.SupplierName;
        bill.SupplierMobile = dataModel.Supplier.Mobile;
        bill.SupplierAddress = dataModel.Supplier.Address;

        var rcvList = new List<BillSaveReceiveVm>();

        if (receivedList.Count > 0)
        {
            foreach (var rcv in receivedList)
            {
                var model = new BillSaveReceiveVm();

                model.RcvId = rcv.Id;
                model.RcvTranNo = rcv.TranNo;
                model.RcvTranDate = rcv.TranDate;

                if (rcv.TranDtls == null || !(rcv.TranDtls.Count > 0))
                {
                    continue;
                }

                var amount = rcv.TranDtls.Sum(x => x.ItemQty * x.UnitPrice);

                model.RcvAmount = amount;

                rcvList.Add(model);
            }

            bill.BillSaveReceiveVms = rcvList;
        }

        var paidList = Context.InventoryBillPayments.Where(x => x.OrderMstId == bill.OrderMstId && !x.IsDeleted).ToList();

        bill.PaidAmount = paidList.Sum(x => x.BillAmount);

        var totalAmount = bill.BillSaveReceiveVms.Sum(x => x.RcvAmount);

        bill.TotalAmount = totalAmount;
        bill.DueAmount = totalAmount - bill.PaidAmount;

        return bill;
    }
}
