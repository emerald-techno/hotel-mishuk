using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.Transaction;
using Interface.Repository.Inventory;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.Inventory;

public class TranRepository : BaseRepository<TranMst>, ITranRepository, IDisposable
{
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public TranRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    public async Task<DataTablePagination<TransactionSearchVm, TransactionSearchVm>>
        SearchAsync(DataTablePagination<TransactionSearchVm, TransactionSearchVm> vm)
    {
        var model = vm.SearchModel;
        var searchResult = Context.TranMsts
            .Include(c => c.Supplier)
            .Include(c => c.Order)
            .Include(c => c.ReqMst)
            .Include(c => c.IssueDept)
            .Include(c => c.IssueEmp)
            .Include(c => c.IssueRoom)
            .Include(c => c.ActionBy)
            .AsQueryable().Where(c => !c.IsDeleted);

        if (model == null) throw new Exception("Search Requsition not found");


        if (!string.IsNullOrEmpty(model.TranNo))
        {
            searchResult = searchResult.Where(c => c.TranNo.Trim() == model.TranNo);
        }

        if (model.SFromDate != null)
        {
            var fromDate = DU.Utility.ConvertStrToDate(model.SFromDate);
            searchResult = searchResult.Where(c => c.TranDate >= fromDate);
        }
        if (model.SToDate != null)
        {
            var toDate = DU.Utility.ConvertStrToDate(model.SToDate);
            searchResult = searchResult.Where(c => c.TranDate <= toDate);
        }

        if (!string.IsNullOrEmpty(model.TranType))
        {
            searchResult = searchResult.Where(c => c.TranType == model.TranType);
        }

        if (model.IssueDeptId > 0)
        {
            searchResult = searchResult.Where(c => c.IssueDeptId == model.IssueDeptId);
        }

        if (model.OrderId > 0)
        {
            searchResult = searchResult.Where(c => c.OrderId == model.OrderId);
        }

        if (model.ReqMstId > 0)
        {
            searchResult = searchResult.Where(c => c.ReqMstId == model.ReqMstId);
        }

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.TranNo.ToLower().Contains(value));
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

            vm.data = _iMapper.Map<List<TransactionSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.SupplierName = filterData?.Supplier?.SupplierName;
                searchDto.IssueRoomNo = filterData?.IssueRoom?.RoomNo;
                searchDto.OrderNo = filterData?.Order?.OrderNo;
                searchDto.OrderDate = filterData?.Order?.OrderDate;
                searchDto.ReqNo = filterData?.ReqMst?.ReqNo;
                searchDto.ReqDate = filterData?.ReqMst?.ReqDate;
                searchDto.IssueDeptName = filterData?.IssueDept?.Name;
                searchDto.IssueEmpName = filterData?.IssueEmp?.Name;
                searchDto.ActionByName = filterData?.ActionBy?.FullName;

                if(searchDto.ActionDate.Date == DateTime.Now.Date)
                    searchDto.CanUpdate = true;
                else
                    searchDto.CanUpdate = false;
            }
        }
        return vm;
    }

    public async Task<DataTablePagination<TransactionSearchVm, TransactionSearchVm>>
        SearchLOAsync(DataTablePagination<TransactionSearchVm, TransactionSearchVm> vm)
    {
        var model = vm.SearchModel;
        var searchResult = Context.TranMsts
            .Include(c => c.Supplier)
            .Include(c => c.Order)
            .Include(c => c.ReqMst)
            .Include(c => c.IssueDept)
            .Include(c => c.IssueEmp)
            .Include(c => c.IssueRoom)
            .Include(c => c.ActionBy)
            .AsQueryable().Where(c => !c.IsDeleted);

        if (model == null) throw new Exception("Search Requsition not found");


        if (!string.IsNullOrEmpty(model.TranNo))
        {
            searchResult = searchResult.Where(c => c.TranNo.Trim() == model.TranNo);
        }

        if (model.SFromDate != null)
        {
            var fromDate = DU.Utility.ConvertStrToDate(model.SFromDate);
            searchResult = searchResult.Where(c => c.TranDate >= fromDate);
        }
        if (model.SToDate != null)
        {
            var toDate = DU.Utility.ConvertStrToDate(model.SToDate);
            searchResult = searchResult.Where(c => c.TranDate <= toDate);
        }

        if (!string.IsNullOrEmpty(model.TranType))
        {
            searchResult = searchResult.Where(c => c.TranType == model.TranType);
        }

        if (model.IssueDeptId > 0)
        {
            searchResult = searchResult.Where(c => c.IssueDeptId == model.IssueDeptId);
        }

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.TranNo.ToLower().Contains(value));
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

            vm.data = _iMapper.Map<List<TransactionSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.SupplierName = filterData?.Supplier?.SupplierName;
                searchDto.IssueRoomNo = filterData?.IssueRoom?.RoomNo;
                searchDto.OrderNo = filterData?.Order?.OrderNo;
                searchDto.OrderDate = filterData?.Order?.OrderDate;
                searchDto.ReqNo = filterData?.ReqMst?.ReqNo;
                searchDto.ReqDate = filterData?.ReqMst?.ReqDate;
                searchDto.IssueDeptName = filterData?.IssueDept?.Name;
                searchDto.IssueEmpName = filterData?.IssueEmp?.Name;
                searchDto.ActionByName = filterData?.ActionBy?.FullName;
            }
        }
        return vm;
    }

    public async Task<TransactionVm> GetTransByIdAsync(long id)
    {
        var dataModel = await Context.TranMsts
            .Include(c => c.RefTran)
            .Include(c => c.TranDtls)
                .ThenInclude(c => c.Item)
                    .ThenInclude(cr => cr.Category)
            .Include(c => c.TranDtls)
                .ThenInclude(c => c.ItemUnit)
            .Include(c => c.TranDtls)
                .ThenInclude(c => c.OrderDtl)
            .Include(c => c.TranDtls)
                .ThenInclude(c => c.ReqDtl)
            .Include(c => c.TranBy)
            .Include(c => c.IssueDept)
            .Include(c => c.IssueEmp)
            .Include(c => c.Supplier)
            .Include(c => c.Order)
            .Include(c => c.ReqMst)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (dataModel == null)
            throw new Exception("Not Found..!");

        var model = _iMapper.Map<TransactionVm>(dataModel);

        model.IssueDeptName = dataModel.IssueDept?.Name;
        model.IssueDeptCode = dataModel.IssueDept?.Code;
        model.IssueEmpName = dataModel.IssueEmp?.Name;
        model.SupplierName = dataModel.Supplier?.SupplierName;
        model.ReqNo = dataModel.ReqMst?.ReqNo;
        model.ReqDate = dataModel.ReqMst?.ReqDate;
        model.OrderNo = dataModel.Order?.OrderNo;
        model.OrderDate = dataModel.Order?.OrderDate;
        model.ReceiveNo = dataModel.RefTran?.TranNo;
        model.TranByName = dataModel.TranBy.FullName;
        model.LedgerId = dataModel.LedgerId;

        foreach (var data in model.TranDtls)
        {
            var reqDtl = dataModel.TranDtls.FirstOrDefault(c => c.Id == data.Id);
            if (reqDtl == null) continue;

            data.ItemName = reqDtl.Item?.ItemName;
            data.ItemUnitName = reqDtl.ItemUnit?.UnitName;
            data.CategoryId = reqDtl.Item?.CategoryId;
            data.LedgerId = reqDtl.Item?.Category?.LedgerId;
        }

        return model;
    }

    #region OpeningDetails

    public async Task<TransactionVm?> GetOpeningData()
    {

        var data = await Context.TranMsts
                                .Include(d => d.TranDtls)
                                    .ThenInclude(l => l.Item)
                                .Include(d => d.TranDtls)
                                    .ThenInclude(l => l.ItemUnit)
                                .FirstOrDefaultAsync(x => x.TranType == TransType.Opening && !x.IsDeleted);

        if (data == null)
            return null;

        var model = _iMapper.Map<TransactionVm>(data);

        if (model.TranDtls.Count > 0)
        {
            foreach (var item in model.TranDtls)
            {
                var filterData = data.TranDtls.FirstOrDefault(x => x.Id == item.Id);

                item.ItemName = filterData.Item?.ItemName;
                item.ItemUnitName = filterData.ItemUnit?.UnitName;
            }
        }

        return model;
    }

    public async Task<TransactionVm?> GetOpeningDataByDept(long id)
    {

        var data = await Context.TranMsts
                                .Include(d => d.TranDtls)
                                    .ThenInclude(l => l.Item)
                                .Include(d => d.TranDtls)
                                    .ThenInclude(l => l.ItemUnit)
                                .FirstOrDefaultAsync(x => x.TranType == TransType.Opening && x.IssueDeptId == id && !x.IsDeleted);

        if (data == null)
            return null;

        var model = _iMapper.Map<TransactionVm>(data);

        if (model.TranDtls.Count > 0)
        {
            foreach (var item in model.TranDtls)
            {
                var filterData = data.TranDtls.FirstOrDefault(x => x.Id == item.Id);

                item.ItemName = filterData.Item?.ItemName;
                item.ItemUnitName = filterData.ItemUnit?.UnitName;
            }
        }

        return model;
    }

    #endregion

    #region DirectReceiveReportData

    public async Task<List<DirectReceiveReportVm>> GetDirectReceiveReportData(DirectReceiveReportVm vm)
    {
        var directReceiveList = Context.TranMsts
            .Include(s => s.Supplier)
            .Include(t => t.TranBy)
            .Include(d => d.TranDtls)
            .AsQueryable()
            .Where(x => x.TranType == TransType.Receive && x.OrderId == null && !x.IsDeleted);

        if (vm.StrFromDate != null)
        {
            var fromDate = DU.Utility.ConvertStrToDate(vm.StrFromDate);
            directReceiveList = directReceiveList.Where(c => c.TranDate >= fromDate);
        }

        if (vm.StrToDate != null)
        {
            var toDate = DU.Utility.ConvertStrToDate(vm.StrToDate);
            directReceiveList = directReceiveList.Where(c => c.TranDate <= toDate);
        }

        if (vm.SupplierId > 0)
        {
            directReceiveList = directReceiveList.Where(c => c.SupplierId == vm.SupplierId);
        }

        var data = await directReceiveList.OrderByDescending(c => c.TranDate)
                                         .ToListAsync();

        var dataList = new List<DirectReceiveReportVm>();

        if (data.Count > 0)
        {
            foreach (var receive in data)
            {
                var model = new DirectReceiveReportVm();

                model.TranId = receive.Id;
                model.TranNo = receive.TranNo;
                model.TranDate = receive.TranDate;
                model.TranById = receive.TranById;
                model.TranByName = receive.TranBy?.FullName;
                model.SupplierId = receive.SupplierId;
                model.SupplierName = receive.Supplier?.SupplierName;
                model.SupplierMobile = receive.Supplier?.Mobile;

                model.ReceiveAmount = receive.TranDtls.Sum(x => x.ItemQty * x.UnitPrice);

                var paidAmount = Context.InventoryBillPayments.Where(x => x.ReceiveId == model.TranId && !x.IsDeleted).Sum(x => x.BillAmount);

                model.PaidAmount = paidAmount;
                model.DueAmount = model.ReceiveAmount > model.PaidAmount ? (model.ReceiveAmount - model.PaidAmount) : 0;

                dataList.Add(model);
            }
        }

        return dataList;
    }

    #endregion
}
