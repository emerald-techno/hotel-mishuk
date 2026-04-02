using AutoMapper;
using Dapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Report;
using Domain.ViewModel.Restaurant.FoodOrder;
using Interface.Repository.Restaurant;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Persistence.DapperModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.Restaurant;

public class FoodOrderRepository : BaseRepository<RsFoodOrder>, IFoodOrderRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;
    private readonly IApplicationReadDbConnection _iReadDbConnection;

    public FoodOrderRepository(ApplicationDbContext db, IMapper iMapper, IApplicationReadDbConnection iReadDbConnection) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
        _iReadDbConnection = iReadDbConnection;
    }
    #endregion

    #region Search
    public async Task<DataTablePagination<FoodOrderSearchVm, FoodOrderSearchVm>> SearchAsync(DataTablePagination<FoodOrderSearchVm, FoodOrderSearchVm> vm)
    {
        var searchResult = Context.RsFoodOrders
            .Include(x => x.Customer)
            .Include(x => x.CustomerType)
            .Include(x => x.Room)
            .Include(x => x.Waiter)
            .Include(x => x.Table)
            .Include(x => x.ActionBy)
            .AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search Food Order not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.OrderNo.ToLower().Contains(value));
        }

        if (model.OrderType != null)
        {
            searchResult = searchResult.Where(c => c.OrderType == model.OrderType);
        }

        if (model.OrderStatus != null)
        {
            searchResult = searchResult.Where(c => c.OrderStatus == model.OrderStatus);
        }

        if (model.PaymentStatus != null)
        {
            searchResult = searchResult.Where(c => c.PaymentStatus == model.PaymentStatus);
        }

        if (!string.IsNullOrEmpty(model.FormDateStr))
        {
            var formDate = (DateTime)(!string.IsNullOrEmpty(model.FormDateStr) ? Utility.ConvertStrToDate(model.FormDateStr) : model.OrderDate);
            searchResult = searchResult.Where(c => c.OrderDate.Date >= formDate.Date);
        }

        if (!string.IsNullOrEmpty(model.ToDateStr))
        {
            var toDate = (DateTime)(!string.IsNullOrEmpty(model.ToDateStr) ? Utility.ConvertStrToDate(model.ToDateStr) : model.OrderDate);
            searchResult = searchResult.Where(c => c.OrderDate.Date <= toDate.Date);
        }

        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderByDescending(c => c.OrderDate)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<FoodOrderSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.CustomerName = $"{filterData.Customer.Salutation} {filterData.Customer.FirstName} {filterData.Customer.LastName}";
                searchDto.CustomerTypeName = filterData.CustomerType.TypeName;
                searchDto.SaleByFullName = filterData.ActionBy.FullName;
                searchDto.RoomNo = filterData.Room?.RoomNo;
                searchDto.TableNo = filterData.Table?.TableNo;
                searchDto.WaiterName = filterData.Waiter?.Name;
            }
        }
        return vm;
    }
    #endregion

    #region GetFoodOrderInfo

    public async Task<RsFoodOrder> GetFoodOrderByIdAsync(long id)
    {
        var data = await Context.RsFoodOrders
            .Include(b => b.Customer)
            .Include(b => b.CustomerType)
            .Include(b => b.Room)
            .Include(b => b.Booking)
            .Include(b => b.Table)
            .Include(b => b.Waiter)
            .Include(b => b.ActionBy)
            .Include(c => c.RsFoodOrderItems)
                .ThenInclude(e => e.Food)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (data == null) throw new Exception("No Food Order Data Found");

        return data;
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }


    #endregion

    #region RsDailySales

    //public async Task<List<RsDailySalesReportVm>> RsMonthlySalesDataAsync(RsDailySalesReportVm vm)
    //{
    //    var foodOrders = Context.RsFoodOrders
    //        .Include(x => x.Customer)
    //        .Include(r => r.Room)
    //        .Include(b => b.Booking)
    //        .Where(x => !x.IsDeleted)
    //        .AsNoTracking();

    //    var rsPayments = Context.RsOrderPayments.AsNoTracking();

    //    vm.StrQueryDate = (string.IsNullOrEmpty(vm.StrQueryDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrQueryDate;
    //    var queryDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrQueryDate));

    //    var foodOrderList = await foodOrders.Where(x => x.OrderDate.Date == queryDate.Date && x.OrderStatus != RsOrderStatusEnum.Canceled).ToListAsync();

    //    var dataResult = new List<RsDailySalesReportVm>();

    //    if (foodOrderList.Count > 0)
    //    {
    //        foreach (var order in foodOrderList)
    //        {
    //            var model = new RsDailySalesReportVm();

    //            model.OrderId = order.Id;
    //            model.OrderNo = order.OrderNo;
    //            model.OrderDate = order.OrderDate;
    //            model.CustomerId = order.CustomerId;
    //            model.CustomerName = $"{order.Customer.Salutation} {order.Customer.FirstName} {order.Customer.LastName}";
    //            model.RoomId = order.RoomId;
    //            model.RoomNo = order.Room?.RoomNo;
    //            model.BookingId = order.BookingId;
    //            model.BookingNo = order.Booking?.BookingNo;
    //            model.PaymentStatus = order.PaymentStatus;
    //            model.OrderAmount = order.NetAmount;

    //            var paidAmount = rsPayments.Where(x => x.OrderId == model.OrderId).Sum(c => c.PaidAmount);
    //            model.PaidAmount = paidAmount;
    //            model.DueAmount = (model.OrderAmount - model.PaidAmount);

    //            dataResult.Add(model);
    //        }
    //    }

    //    return dataResult;
    //}

    public async Task<List<RsDailySalesReportVm>> RsMonthlySalesDataAsync(RsDailySalesReportVm vm)
    {

        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrToDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");
        string fromDateFilter = (!string.IsNullOrEmpty(vm.StrFromDate)) ? $" and convert(date,o.OrderDate) >= '{fromDate}'" : "";
        string toDateFilter = (!string.IsNullOrEmpty(vm.StrToDate)) ? $" and convert(date,o.OrderDate) <= '{toDate}'" : "";
        string customerFilter = (vm.CustomerId > 0) ? $" and c.Id = {vm.CustomerId}" : "";
        string customerTypeFilter = (vm.CustomerTypeId > 0) ? $" and ct.Id = {vm.CustomerTypeId}" : "";

        string query = $@"select c.Id CustomerId,c.FirstName+' '+isnull(c.LastName,'') CustomerName,c.Mobile,ct.TypeName CustomerType,ct.Id CustomerTypeId, ri.RoomNo, o.BookingId, bk.BookingNo,bk.BookingDate
                    ,sum(o.OrderAmount)OrderAmount,sum(o.VAT) Vat,sum(o.TAX) Tax,sum(o.ServiceCharge)ServiceCharge,sum(o.Discount)Discount,sum(o.NetAmount)NetAmount
                    ,sum(isnull(P.PaidAmount,0))PaidAmount,sum(o.NetAmount - isnull(p.PaidAmount,0)) DueAmount,sum(o.OrderAmount + o.ServiceCharge+o.VAT+o.TAX) NetOrderAmount,
                    o.id OrderId,o.OrderNo,cast(o.OrderDate as date) OrderDate, o.ReservationDate, o.OrderDesc, w.Name WaiterName, t.TableNo, o.AuditDate, o.AuditById, o.AuditRemarks
                    from RsFoodOrders o 
                    inner join RsCustomers c on c.Id = o.CustomerId
                    left join RsCustomerTypes ct on ct.Id = o.CustomerTypeId
                    left join HtBookingServices bk on bk.Id = o.BookingId
                    left join HtRoomInfos ri on ri.Id = o.RoomId
                    left join RsWaiters w on w.Id = o.WaiterId
                    left join RsTables t on t.Id = o.TableId
                    left join (select OrderId,sum(PaidAmount) PaidAmount from RsOrderPayments where IsDeleted = 0 and PaymentType = 0 group by OrderId) p on p.OrderId = o.Id
                    where o.IsDeleted = 0 and o.OrderStatus != 2 {customerFilter} {customerTypeFilter} {fromDateFilter} {toDateFilter}
                    group by c.Id,c.FirstName+' '+isnull(c.LastName,''),c.Email,c.Mobile,ct.TypeName,ct.Id ,o.id,o.OrderNo,cast(o.OrderDate as date),o.OrderDesc, ri.RoomNo,o.BookingId,bk.BookingNo,o.ReservationDate,bk.BookingDate,w.Name, t.TableNo, o.AuditDate, o.AuditById, o.AuditRemarks
                    order by c.FirstName+' '+isnull(c.LastName,'')";

        var data = await _iReadDbConnection.QueryAsync<RsDailySalesReportVm>(query);
        return data.ToList();

        //try
        //{
        //}
        //catch (Exception e)
        //{
        //    throw new Exception(e.Message);
        //}
    }


    #endregion

    #region GetCustomerWiseDueReportData

    public async Task<List<RsCustomerWiseDueReportVm>> GetCustomerWiseDueReportData(RsCustomerWiseDueReportVm vm)
    {
        string customerFilter = (vm.CustomerId > 0) ? $" and c.Id = {vm.CustomerId}" : "";

        string customerTypeFilter = (vm.CustomerTypeId > 0) && !vm.ShowWithoutEmployee ? $" and ct.Id = {vm.CustomerTypeId}" :
            vm.ShowWithoutEmployee ? $"and ct.Id != 6" : "";

        string dateFilter = !string.IsNullOrEmpty(vm.StrQueryDate) ? $" and CONVERT(date, o.OrderDate) >= '{vm.StrQueryDate}'" : "";
        string orderInfoColumn = (vm.GroupBy == "O") ? ",o.id OrderId,o.OrderNo,cast(o.OrderDate as date) OrderDate" : "";
        string groupByColumn = (vm.GroupBy == "O") ? ",o.id,o.OrderNo,cast(o.OrderDate as date)" : "";

        string query = $@"select c.Id CustomerId,c.FirstName+' '+isnull(c.LastName,'') CustomerName,c.Email,c.Mobile,ct.TypeName CustomerType,ct.Id CustomerTypeId
        ,sum(o.OrderAmount)OrderAmount,sum(o.VAT)vat,sum(o.TAX)tax,sum(o.ServiceCharge)ServiceCharge,sum(o.Discount)Discount,sum(o.NetAmount)NetAmount
        ,sum(isnull(P.PaidAmount,0))PaidAmount,sum(o.NetAmount - isnull(p.PaidAmount,0)) DueAmount,sum(o.OrderAmount + o.ServiceCharge+o.VAT+o.TAX) NetOrderAmount
        {orderInfoColumn} 
        from RsFoodOrders o 
        inner join RsCustomers c on c.Id = o.CustomerId
        left join RsCustomerTypes ct on ct.Id = o.CustomerTypeId
        left join HtBookingServices bk on bk.Id = o.BookingId
        left join HtRoomInfos ri on ri.Id = o.RoomId
        left join (select OrderId,sum(PaidAmount) PaidAmount from RsOrderPayments where IsDeleted = 0 group by OrderId) p on p.OrderId = o.Id
        where o.IsDeleted = 0 and o.OrderStatus != {(int)RsOrderStatusEnum.Canceled} {customerFilter} {customerTypeFilter} {dateFilter}
        group by c.Id,c.FirstName+' '+isnull(c.LastName,''),c.Email,c.Mobile,ct.TypeName,ct.Id {groupByColumn}
        having sum(o.NetAmount - isnull(p.PaidAmount,0)) > 0 
        order by c.FirstName+' '+isnull(c.LastName,'')";

        var data = await _iReadDbConnection.QueryAsync<RsCustomerWiseDueReportVm>(query);
        return data.ToList();
    }

    #endregion

    #region RsCustomerWiseDueReportHtml

    public async Task<string> RsCustomerWiseDueReportHtml(RsCustomerWiseDueReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await GetCustomerWiseDueReportData(vm);
        int colNumber = (vm.GroupBy == "O") ? 11 : 9;
        int footerColNumber = (vm.GroupBy == "O") ? 6 : 4;

        if (vm.GroupBy == "O")
        {
            fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";
            fullHtml += $@"<tr style='height:30px;'><td colspan='10' class='text-center'>Date: {DateTime.Today:dd/MMM/yyyy}</td></tr>";
            fullHtml += "<tr style='height:30px;' class='text-center'>";
            fullHtml += $@"
                            <th style='width:80px;'> Customer Name</th> 
                            <th style='width:80px;'> Mobile</th> 
                            <th style='width:200px;'>Customer Type</th>
                            <th style='width:80px;'> Order No</th>
                            <th style='width:130px;'> Order Date</th>
                            <th style='width:130px;'> Order Amount</th>
                            <th style='width:80px;'> Discount</th>
                            <th style='width:100px;'> Net Amount</th>
                            <th style='width:100px;'> Paid Amount</th>
                            <th style='width:100px;'> Due Amount</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            var customerList = data.Select(o => new { o.CustomerId, o.CustomerName, o.Mobile }).Distinct().ToList();
            int sl = 1;
            foreach (var customer in customerList)
            {
                List<RsCustomerWiseDueReportVm> customerWiseDataList = data.Where(o => o.CustomerId == customer.CustomerId).ToList();
                for (int i = 0; i < customerWiseDataList.Count; i++)
                {
                    RsCustomerWiseDueReportVm objDue = customerWiseDataList[i];
                    string orderNo = !isPrint ? $"<a target='_blank' href='../FoodOrder/Details/{objDue.OrderId}'>{objDue.OrderNo}</a>" : $"{objDue.OrderNo}";

                    if (i == 0)
                    {
                        fullHtml += "<tr>";
                        fullHtml += $@"<td style='text-align:center;' rowspan='{customerWiseDataList.Count}'><b>{objDue.CustomerName}</b></td>                              
                                <td style='text-align:center;' rowspan='{customerWiseDataList.Count}'>{objDue.Mobile}</td>
                                <td style='text-align:center;' rowspan='{customerWiseDataList.Count}'>{objDue.CustomerType}</td>
                                <td style='text-align:center;' >{orderNo}</td>
                                <td style='text-align:center;' >{objDue.OrderDate:dd/MMM/yy}</td>
                                <td style='text-align:center;'>{objDue.NetOrderAmount:F2}</td>
                                <td style='text-align:center;'>{objDue.Discount:F2}</td>
                                <td style='text-align:center;'>{objDue.NetAmount:F2}</td>
                                <td style='text-align:center;'>{objDue.PaidAmount:F2}</td>
                                <td style='text-align:center;'>{objDue.DueAmount:F2}</td>";
                        fullHtml += "</tr>";
                    }
                    else
                    {
                        fullHtml += "<tr>";
                        fullHtml += $@"<td style='text-align:center;' >{orderNo}</td>
                                <td style='text-align:center;' >{objDue.OrderDate:dd/MMM/yy}</td>
                                <td style='text-align:center;'>{objDue.NetOrderAmount:F2}</td>
                                <td style='text-align:center;'>{objDue.Discount:F2}</td>
                                <td style='text-align:center;'>{objDue.NetAmount:F2}</td>
                                <td style='text-align:center;'>{objDue.PaidAmount:F2}</td>
                                <td style='text-align:center;'>{objDue.DueAmount:F2}</td>";
                        fullHtml += "</tr>";
                    }
                }
                //customer wise total
                fullHtml += $@"<tr style='font-weight:bold;'><td colspan='5' style='text-align:right;font-weight:bold;'><b>{customer.CustomerName} Total : </b></td>                              
                                <td style='text-align:center;'>{customerWiseDataList.Sum(o => o.NetOrderAmount):F2}</td>
                                <td style='text-align:center;'>{customerWiseDataList.Sum(o => o.Discount):F2}</td>
                                <td style='text-align:center;'>{customerWiseDataList.Sum(o => o.NetAmount):F2}</td>
                                <td style='text-align:center;'>{customerWiseDataList.Sum(o => o.PaidAmount):F2}</td>
                                <td style='text-align:center;'>{customerWiseDataList.Sum(o => o.DueAmount):F2}</td>";
                fullHtml += "</tr>";
            }
            fullHtml += "</tbody>";
            fullHtml += "<tfoot>";
            fullHtml += $@"<tr style='font-weight:bold;'><td colspan='5' style='text-align:right;'><b>Grand Total</b></td>                              
                                <td style='text-align:center;'>{data.Sum(o => o.NetOrderAmount):F2}</td>
                                <td style='text-align:center;'>{data.Sum(o => o.Discount):F2}</td>
                                <td style='text-align:center;'>{data.Sum(o => o.NetAmount):F2}</td>
                                <td style='text-align:center;'>{data.Sum(o => o.PaidAmount):F2}</td>
                                <td style='text-align:center;'>{data.Sum(o => o.DueAmount):F2}</td>";
            fullHtml += "</tr>";
            fullHtml += "</tfoot>";


            fullHtml += "</table>";
        }
        else
        {
            if (data != null && data.Count > 0)
            {
                fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
                fullHtml += "<thead>";

                fullHtml += $@"<tr style='height:30px;'><td colspan='9' class='text-center'>Date: {DateTime.Today:dd/MMM/yyyy}</td></tr>";

                fullHtml += "<tr style='height:30px;' class='text-center'>";

                fullHtml += $@"<th style='width:50px;' >SL No</th>
                            <th style='width:200px;' >Customer Name</th>
                            <th style='width:80px;' >Mobile</th>
                            <th style='width:130px;' >Customer  Type</th>
                            <th style='width:130px;' >Order Amount</th>
                            <th style='width:80px;' >Discount</th>
                            <th style='width:100px;' >Net Amount</th>
                            <th style='width:100px;' >Paid Amount</th>
                            <th style='width:100px;' >Due Amount</th>";

                fullHtml += "</tr>";

                fullHtml += "</thead>";
                fullHtml += "<tbody>";

                for (int i = 0; i < data.Count; i++)
                {
                    RsCustomerWiseDueReportVm objDue = data[i];
                    fullHtml += "<tr>";

                    fullHtml += $@"<td style='text-align:left;'>{i + 1}</td>
                                <td style='text-align:center;'><b>{objDue.CustomerName}</b></td>                              
                                <td style='text-align:center;'>{objDue.Mobile}</td>
                                <td style='text-align:center;'>{objDue.CustomerType}</td>
                                <td style='text-align:center;'>{objDue.NetOrderAmount:F2}</td>
                                <td style='text-align:center;'>{objDue.Discount:F2}</td>
                                <td style='text-align:center;'>{objDue.NetAmount:F2}</td>
                                <td style='text-align:center;'>{objDue.PaidAmount:F2}</td>
                                <td style='text-align:center;'>{objDue.DueAmount:F2}</td>";
                    fullHtml += "</tr>";

                }

                fullHtml += "</tbody>";

                fullHtml += "<tfoot>";
                fullHtml += $@"<tr><td colspan='4' style='text-align:right;'><b>TOTAL</b></td>                              
                                <td style='text-align:center;'>{data.Sum(o => o.NetOrderAmount):F2}</td>
                                <td style='text-align:center;'>{data.Sum(o => o.Discount):F2}</td>
                                <td style='text-align:center;'>{data.Sum(o => o.NetAmount):F2}</td>
                                <td style='text-align:center;'>{data.Sum(o => o.PaidAmount):F2}</td>
                                <td style='text-align:center;'>{data.Sum(o => o.DueAmount):F2}</td>";
                fullHtml += "</tr>";
                fullHtml += "</tfoot>";


                fullHtml += "</table>";

            }
        }

        return fullHtml;
    }

    #endregion

    #region GetRsSalesReportData

    public async Task<List<RsSalesReportVm>> GetRsSalesReportData(RsSalesReportVm vm)
    {
        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");
        string fromDateFilter = (!string.IsNullOrEmpty(vm.StrFromDate)) ? $" and convert(date,o.OrderDate) >= '{fromDate}'" : "";
        string toDateFilter = (!string.IsNullOrEmpty(vm.StrToDate)) ? $" and convert(date,o.OrderDate) <= '{toDate}'" : "";
        string categoryFilter = (vm.CategoryId > 0) ? $" and fc.Id = {vm.CategoryId}" : "";
        string itemFilter = (vm.ItemId > 0) ? $" and fi.Id = {vm.ItemId}" : "";

        string query = $@"select o.Id OrderId,o.OrderNo,convert(Date,o.OrderDate)OrderDate,o.OrderDesc,c.Id CustomerId,c.FirstName+' '+isnull(c.LastName,'') CustomerName,ct.TypeName CustomerType,ct.Id CustomerTypeId,bk.Id BookingI,bk.BookingNo,ri.Id RoomId,ri.RoomNo
        ,o.OrderAmount,o.VAT,o.TAX,o.ServiceCharge,o.Discount,o.NetAmount,o.OrderStatus,o.PaymentStatus,fc.Id CategoryId,fc.CategoryName,fi.Id ItemId,fi.ItemName,fi.ItemCode,d.Quantity,d.Rate,d.TotalAmount ItemAmount
        ,P.PaidAmount,o.NetAmount - isnull(p.PaidAmount,0) DueAmount
        from RsFoodOrderItems d
        inner join RsFoodItems fi on fi.Id = d.FoodId
        inner join RsFoodCategories fc on fc.Id = fi.CategoryId
        inner join RsFoodOrders o on o.Id = d.OrderId
        inner join RsCustomers c on c.Id = o.CustomerId
        left join RsCustomerTypes ct on ct.Id = o.CustomerTypeId
        left join HtBookingServices bk on bk.Id = o.BookingId
        left join HtRoomInfos ri on ri.Id = o.RoomId
        left join (select OrderId,sum(PaidAmount) PaidAmount from RsOrderPayments where IsDeleted = 0 group by OrderId) p on p.OrderId = o.Id
        where d.IsDeleted = 0 and o.IsDeleted = 0 and o.OrderStatus != {(int)RsOrderStatusEnum.Canceled} {fromDateFilter} {toDateFilter} {categoryFilter} {itemFilter}
        order by o.OrderDate,o.OrderNo";

        if (vm.GroupBy == "I")//ONLY ITEM WISE SALES, ITEM COUNT
        {
            query = $@"select fc.Id CategoryId,fc.CategoryName,fi.Id ItemId,fi.ItemName,fi.ItemCode,sum(d.Quantity) Quantity,round(avg(d.Rate),2) Rate,round(sum(Quantity *d.Rate),2) ItemAmount
            from RsFoodOrderItems d
            inner join RsFoodItems fi on fi.Id = d.FoodId
            inner join RsFoodCategories fc on fc.Id = fi.CategoryId
            inner join RsFoodOrders o on o.Id = d.OrderId
            where d.IsDeleted = 0 and o.IsDeleted = 0 
            and o.OrderStatus != {(int)RsOrderStatusEnum.Canceled} {fromDateFilter} {toDateFilter} {categoryFilter} {itemFilter}
            group by fc.Id,fc.CategoryName,fi.Id,fi.ItemName,fi.ItemCode 
            order by fc.CategoryName,fi.ItemName";
        }

        var data = await _iReadDbConnection.QueryAsync<RsSalesReportVm>(query);
        return data.ToList();
    }

    #endregion

    #region RsSalesReportHtml

    public async Task<string> RsSalesReportHtml(RsSalesReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await GetRsSalesReportData(vm);

        if (vm.GroupBy == "I")
        {
            if (data != null && data.Count > 0)
            {
                fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
                fullHtml += "<thead>";

                fullHtml += $@"<tr style='height:30px;'><td colspan='5' class='text-center'>Date: </td></tr>";

                fullHtml += "<tr style='height:30px;font-size:10px;' class='text-center'>";
                fullHtml += $@"<th rowspan='2' style='width:200px;' >Category</th>
                            <th rowspan='2' style='width:200px;' >Item</th>
                            <th style='width:180px;' colspan='3' >Order Info</th>
                            </tr>";

                fullHtml += $@"<tr style='font-size:9px;'><th style='width:80px;text-align:center;'>Quantity</th>
                            <th style='width:80px;text-align:center;'>Rate</th>
                            <th style='width:80px;text-align:center;'>Amount</th></tr>";

                fullHtml += "</thead>";
                fullHtml += "<tbody>";
                for (int i = 0; i < data.Count; i++)
                {
                    var objItem = data[i];
                    string itemLink = $"<a target='_blank' href='../FoodItem/Details/{objItem.ItemId}'>{objItem.ItemName}</a>";

                    fullHtml += "<tr>";
                    fullHtml += $@"<td style='text-align:left;'>{objItem.CategoryName}</td>
                                <td style='text-align:left;'>{itemLink}</td>
                                <td style='text-align:center;'>{objItem.Quantity:F2}</td>
                                <td style='text-align:center;'>{objItem.Rate:F2}</td>
                                <td style='text-align:center;'>{objItem.ItemAmount:F2}</td>
                    </tr>";

                }
                fullHtml += "</tbody>";

                fullHtml += "<tfoot>";
                fullHtml += $@"<tr style='font-weight:bold;'>
                        <td colspan='2' style='text-align:right;'><b>TOTAL</b></td>                              
                        <td style='text-align:center;'>{data.Sum(o => o.Quantity):F2}</td>
                        <td style='text-align:center;'></td>
                        <td style='text-align:center;'>{data.Sum(o => o.ItemAmount):F2}</td>";
                fullHtml += "</tr>";
                fullHtml += "</tfoot>";
                fullHtml += "</table>";
            }
        }
        else
        {

            var order = data.Select(o => new { o.OrderId, o.OrderNo, o.OrderDate, o.CustomerName, o.VAT, o.Tax, o.Discount, o.NetAmount, o.PaidAmount, o.DueAmount }).Distinct().ToList();
            if (data != null && data.Count > 0)
            {
                fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
                fullHtml += "<thead>";

                fullHtml += $@"<tr style='height:30px;'><td colspan='13' class='text-center'>Date: </td></tr>";

                fullHtml += "<tr style='height:30px;font-size:10px;' class='text-center'>";
                fullHtml += $@"<th rowspan='2' style='width:100px;' >Order No</th>
                            <th rowspan='2' style='width:150px;' >Customer</th>
                            <th rowspan='2' style='width:200px;' >Category</th>
                            <th rowspan='2' style='width:200px;' >Item</th>
                            <th style='width:180px;' colspan='3' >Order Info</th>
                            <th rowspan='2' style='width:50px;' >VAT</th>
                            <th rowspan='2' style='width:50px;' >TAX</th>
                            <th rowspan='2' style='width:50px;' >Discount</th>
                            <th rowspan='2' style='width:50px;' >Net Amount</th>
                            <th rowspan='2' style='width:50px;' >Paid Amount</th>
                            <th rowspan='2' style='width:50px;' >Due Amount</th> </tr>";

                fullHtml += $@"<tr style='font-size:9px;'><th style='width:60px;'>Qty</th>
                            <th style='width:60px;'>Rate</th>
                            <th style='width:60px;'>Amount</th></tr>";

                fullHtml += "</thead>";
                fullHtml += "<tbody>";

                for (int j = 0; j < order.Count; j++)
                {
                    var objOrder = order[j];
                    var orderItem = data.Where(o => o.OrderId == objOrder.OrderId).ToList();
                    for (int i = 0; i < orderItem.Count; i++)
                    {
                        RsSalesReportVm objItem = orderItem[i];
                        if (i == 0)
                        {
                            fullHtml += "<tr>";
                            fullHtml += $@"<td rowspan='{orderItem.Count}' style='text-align:center;'>{objItem.OrderNo} ({objItem.OrderDate:dd/MMM/yy})</td>                              
                                <td  rowspan='{orderItem.Count}' style='text-align:center;'>{objItem.CustomerName}</td>
                                <td style='text-align:center;'>{objItem.CategoryName}</td>
                                <td style='text-align:center;'>{objItem.ItemName}</td>
                                <td style='text-align:center;'>{objItem.Quantity}</td>
                                <td style='text-align:center;'>{objItem.Rate}</td>
                                <td style='text-align:center;'>{objItem.ItemAmount}</td>
                                <td rowspan='{orderItem.Count}' style='text-align:center;'>{objItem.VAT}</td>
                                <td rowspan='{orderItem.Count}' style='text-align:center;'>{objItem.Tax}</td>
                                <td rowspan='{orderItem.Count}' style='text-align:center;'>{objItem.Discount}</td>
                                <td rowspan='{orderItem.Count}' style='text-align:center;'>{objItem.NetAmount}</td>
                                <td rowspan='{orderItem.Count}' style='text-align:center;'>{objItem.PaidAmount}</td>
                                <td rowspan='{orderItem.Count}' style='text-align:center;'>{objItem.DueAmount}</td>";
                            fullHtml += "</tr>";
                        }
                        else
                        {
                            fullHtml += $@"<tr>
                                <td style='text-align:center;'>{objItem.CategoryName}</td>
                                <td style='text-align:center;'>{objItem.ItemName}</td>
                                <td style='text-align:center;'>{objItem.Quantity}</td>
                                <td style='text-align:center;'>{objItem.Rate}</td>
                                <td style='text-align:center;'>{objItem.ItemAmount}</td>";
                            fullHtml += "</tr>";
                        }
                    }

                }

                fullHtml += "</tbody>";

                fullHtml += "<tfoot>";
                fullHtml += $@"<tr><td colspan='4' style='text-align:right;'><b>TOTAL</b></td>                              
                        <td style='text-align:center;'>{data.Sum(o => o.Quantity):F2}</td>
                        <td style='text-align:center;'></td>
                        <td style='text-align:center;'>{data.Sum(o => o.ItemAmount):F2}</td>
                        <td style='text-align:center;'>{order.Sum(o => o.VAT):F2}</td>
                        <td style='text-align:center;'>{order.Sum(o => o.Tax):F2}</td>
                        <td style='text-align:center;'>{order.Sum(o => o.Discount):F2}</td>
                        <td style='text-align:center;'>{order.Sum(o => o.NetAmount):F2}</td>
                        <td style='text-align:center;'>{order.Sum(o => o.PaidAmount):F2}</td>
                        <td style='text-align:center;'>{order.Sum(o => o.DueAmount):F2}</td>";
                fullHtml += "</tr>";
                fullHtml += "</tfoot>";
                fullHtml += "</table>";
            }
        }

        return fullHtml;
    }

    #endregion

    #region GetRsDailySalesSummaryReportData

    public async Task<List<RsDailySalesSummaryVm>> GetRsDailySalesSummaryReportData(RsDailySalesSummaryVm vm)
    {
        vm.StrQueryDate = (string.IsNullOrEmpty(vm.StrQueryDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrQueryDate;
        var queryDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrQueryDate)).ToString("dd/MMM/yyyy");

        //for consider reservation system : 23/Mar/2024 Masuk // updated : 16/May/2025 for add table no, Masuk
        string query = $@"/*Same day order payment*/
        select convert(date,o.OrderDate) TranDate, o.Id OrderId,convert(date,o.OrderDate)OrderDate,o.OrderNo,ct.TypeName CustomerType,ct.Id CustomerTypeId,bk.Id BookingId,bk.BookingNo,ri.Id RoomId,ri.RoomNo
        ,o.OrderAmount,o.VAT,o.TAX,o.ServiceCharge,o.Discount,o.NetAmount,isnull(P.PaidAmount,0)PaidAmount,
        /*o.NetAmount - isnull(p.PaidAmount,0) - isnull(hp.PaidAmount,0) DueAmount*/
        
        /*for showing due amount on due date and not to show DueAmount = 0 on the day it was due, Updated DueAmount : 4/Nov/2025, Rifat*/

        /*case when fbs.BillStatus != 2 or fbs.BillStatus is null then o.NetAmount - isnull(p.PaidAmount,0) - isnull(hp.PaidAmount,0) else 0 end DueAmount*/
        case when fbs.BillStatus != 2 or fbs.BillStatus is null then o.NetAmount - isnull(p.PaidAmount,0) - isnull(hp.PaidAmount,0) else o.NetAmount end DueAmount

        ,0 DueCollection,0 AdvanceAmount,0 AdvanceRefund,isnull(c.Salutation,'')+' '+c.FirstName+' '+isnull(c.LastName,'') CustomerName,t.TableNo,t.Id TableId 
        from RsFoodOrders o
        inner join RsCustomers c on c.Id = o.CustomerId
        left join RsCustomerTypes ct on ct.Id = o.CustomerTypeId
        left join HtBookingServices bk on bk.Id = o.BookingId
        left join HtRoomInfos ri on ri.Id = o.RoomId
        left join RsTables t on t.Id = o.TableId 
        /*left join (select OrderId,sum(PaidAmount) PaidAmount from RsOrderPayments where IsDeleted = 0 and convert(date,PaidDate) = '{queryDate}' group by OrderId) p on p.OrderId = o.Id*/
        left join (select OrderId,sum(PaidAmount) PaidAmount from RsOrderPayments where IsDeleted = 0 and BillDtlId is null and convert(date,PaidDate) = '{queryDate}' group by OrderId) p on p.OrderId = o.Id
        left join (select OrderId,sum(PaidAmount) PaidAmount from RsOrderPayments where IsDeleted = 0 and BillDtlId is not null and convert(date,PaidDate) = '{queryDate}' group by OrderId) hp on hp.OrderId = o.Id
        left join (select b.BookingId, b.BillStatus
        from HtBillings  b
        inner join HtBillingDetails bd on bd.BillId = b.Id 
        where bd.ServiceId = 2
        group by b.BookingId, b.BillStatus) fbs on fbs.BookingId = o.BookingId
        where o.IsDeleted = 0 and convert(date,o.OrderDate) = '{queryDate}' and o.OrderType = 0
        union all
        /*Due Collection*/
        select convert(date,dp.PaidDate) TranDate,o.Id OrderId,convert(date,o.OrderDate)OrderDate,o.OrderNo,ct.TypeName CustomerType,ct.Id CustomerTypeId,bk.Id BookingI,bk.BookingNo,ri.Id RoomId,ri.RoomNo
        ,o.OrderAmount,o.VAT,o.TAX,o.ServiceCharge,o.Discount,o.NetAmount,0 PaidAmount,o.NetAmount - isnull(ap.PaidAmount,0) DueAmount,sum(dp.PaidAmount) DueCollection,0 AdvanceAmount,0 AdvanceRefund,
        isnull(c.Salutation,'')+' '+c.FirstName+' '+isnull(c.LastName,'') CustomerName,t.TableNo,t.Id TableId 
        from RsOrderPayments dp
        inner join RsFoodOrders o on o.Id = dp.OrderId
        inner join RsCustomers c on c.Id = o.CustomerId
        left join RsCustomerTypes ct on ct.Id = o.CustomerTypeId
        left join HtBookingServices bk on bk.Id = o.BookingId
        left join HtRoomInfos ri on ri.Id = o.RoomId
        left join RsTables t on t.Id = o.TableId 
        left join (select OrderId,sum(PaidAmount) PaidAmount from RsOrderPayments where IsDeleted = 0 and convert(date,PaidDate) <= '{queryDate}' and PaymentType = 0  group by OrderId) ap on ap.OrderId = o.Id
        left join (select OrderId,sum(PaidAmount) PaidAmount from RsOrderPayments where IsDeleted = 0 and convert(date,PaidDate) < '{queryDate}' and PaymentType = 0  group by OrderId) p on p.OrderId = o.Id
        where o.IsDeleted = 0 and dp.IsDeleted = 0 and convert(date,dp.PaidDate) > convert(date,o.OrderDate)
        and convert(date,dp.PaidDate) = '{queryDate}' and  dp.PaymentType = 0 
        group by convert(date,dp.PaidDate),o.Id,o.OrderNo,ct.TypeName,ct.Id,bk.Id,bk.BookingNo,ri.Id,ri.RoomNo,o.OrderAmount,o.VAT,o.TAX,o.ServiceCharge,o.Discount,o.NetAmount
        ,convert(date,o.OrderDate),isnull(p.PaidAmount,0),isnull(c.Salutation,''),c.FirstName,isnull(c.LastName,''),t.TableNo,t.Id,isnull(ap.PaidAmount,0) 
        /*reservation advance*/
        union all
        select convert(date,dp.PaidDate) TranDate,o.Id OrderId,convert(date,o.OrderDate)OrderDate,o.OrderNo,ct.TypeName CustomerType,ct.Id CustomerTypeId,bk.Id BookingI,bk.BookingNo,ri.Id RoomId,ri.RoomNo
        ,o.OrderAmount,o.VAT,o.TAX,o.ServiceCharge,o.Discount,o.NetAmount,0 PaidAmount,0 DueAmount,0 DueCollection,sum(dp.PaidAmount) AdvanceAmount,0 AdvanceRefund,
        isnull(c.Salutation,'')+' '+c.FirstName+' '+isnull(c.LastName,'') CustomerName,t.TableNo,t.Id TableId 
        from RsOrderPayments dp
        inner join RsFoodOrders o on o.Id = dp.OrderId
        inner join RsCustomers c on c.Id = o.CustomerId
        left join RsCustomerTypes ct on ct.Id = o.CustomerTypeId
        left join HtBookingServices bk on bk.Id = o.BookingId
        left join HtRoomInfos ri on ri.Id = o.RoomId
        left join RsTables t on t.Id = o.TableId 
        where o.IsDeleted = 0 and dp.IsDeleted = 0 and convert(date,dp.PaidDate) = '{queryDate}' and o.OrderType = 1 and convert(date,o.OrderDate) > '{queryDate}'
        group by convert(date,dp.PaidDate),o.Id,o.OrderNo,ct.TypeName,ct.Id,bk.Id,bk.BookingNo,ri.Id,ri.RoomNo,o.OrderAmount,o.VAT,o.TAX,o.ServiceCharge,o.Discount,o.NetAmount
        ,convert(date,o.OrderDate),isnull(c.Salutation,''),c.FirstName,isnull(c.LastName,''),t.TableNo,t.Id 
        /*reservation order*/
        union all
        select convert(date,dp.PaidDate) TranDate,o.Id OrderId,convert(date,o.OrderDate)OrderDate,o.OrderNo,ct.TypeName CustomerType,ct.Id CustomerTypeId,bk.Id BookingI,bk.BookingNo,ri.Id RoomId,ri.RoomNo
        ,o.OrderAmount,o.VAT,o.TAX,o.ServiceCharge,o.Discount,o.NetAmount,sum(dp.PaidAmount) + isnull(p.PaidAmount,0) PaidAmount,o.NetAmount - isnull(sum(dp.PaidAmount),0)  - isnull(p.PaidAmount,0) DueAmount,0 DueCollection,0 AdvanceAmount,isnull(p.PaidAmount,0) AdvanceRefund,isnull(c.Salutation,'')+' '+c.FirstName+' '+isnull(c.LastName,'') CustomerName
        ,t.TableNo,t.Id TableId 
        from RsOrderPayments dp
        inner join RsFoodOrders o on o.Id = dp.OrderId
        inner join RsCustomers c on c.Id = o.CustomerId
        left join RsCustomerTypes ct on ct.Id = o.CustomerTypeId
        left join HtBookingServices bk on bk.Id = o.BookingId
        left join HtRoomInfos ri on ri.Id = o.RoomId
        left join RsTables t on t.Id = o.TableId 
        left join (select OrderId,sum(PaidAmount) PaidAmount from RsOrderPayments where IsDeleted = 0 and PaymentType = 1 and convert(date,PaidDate) = '{queryDate}' group by OrderId) p on p.OrderId = o.Id -- advance refund
        where o.IsDeleted = 0 and dp.IsDeleted = 0 and convert(date,dp.PaidDate) = '{queryDate}' and o.OrderType = 1 and dp.PaymentType = 0 and convert(date,o.OrderDate) = '{queryDate}'
        group by convert(date,dp.PaidDate),o.Id,o.OrderNo,ct.TypeName,ct.Id,bk.Id,bk.BookingNo,ri.Id,ri.RoomNo,o.OrderAmount,o.VAT,o.TAX,o.ServiceCharge,o.Discount,o.NetAmount
        ,convert(date,o.OrderDate),isnull(p.PaidAmount,0),isnull(c.Salutation,''),c.FirstName,isnull(c.LastName,''),t.TableNo,t.Id 
        union all
        select convert(date,o.OrderDate) TranDate,o.Id OrderId,convert(date,o.OrderDate)OrderDate,o.OrderNo,ct.TypeName CustomerType,ct.Id CustomerTypeId,bk.Id BookingI,bk.BookingNo,ri.Id RoomId,ri.RoomNo
        ,o.OrderAmount,o.VAT,o.TAX,o.ServiceCharge,o.Discount,o.NetAmount,0 PaidAmount,o.NetAmount - isnull(dp.TotalPaidAmount,0) DueAmount,0 DueCollection,0 AdvanceAmount,isnull(p.PaidAmount,0) AdvanceRefund,
        isnull(c.Salutation,'')+' '+c.FirstName+' '+isnull(c.LastName,'') CustomerName,t.TableNo,t.Id TableId 
        from RsFoodOrders o
        inner join RsCustomers c on c.Id = o.CustomerId
        left join RsCustomerTypes ct on ct.Id = o.CustomerTypeId
        left join HtBookingServices bk on bk.Id = o.BookingId
        left join HtRoomInfos ri on ri.Id = o.RoomId
        left join RsTables t on t.Id = o.TableId 
        left join (
            select OrderId, sum(PaidAmount) TotalPaidAmount
            from RsOrderPayments
            where IsDeleted = 0 and PaymentType = 0 and convert(date,PaidDate) <= '{queryDate}'
            group by OrderId
        ) dp on dp.OrderId = o.Id
        left join (
            select OrderId,sum(PaidAmount) PaidAmount 
            from RsOrderPayments 
            where IsDeleted = 0 and PaymentType = 1 and convert(date,PaidDate) = '{queryDate}' 
            group by OrderId
        ) p on p.OrderId = o.Id
        where o.IsDeleted = 0 
          and o.OrderStatus = 1 
          and o.OrderType = 1 
          and convert(date,o.OrderDate) = '{queryDate}'
          and not exists (
                select 1 
                from RsOrderPayments px
                where px.OrderId = o.Id 
                  and px.IsDeleted = 0 
                  and px.PaymentType = 0 
                  and convert(date,px.PaidDate) = convert(date,o.OrderDate)
          )";

        var data = await _iReadDbConnection.QueryAsync<RsDailySalesSummaryVm>(query);
        return data.ToList();
    }
    public async Task<List<PaymentModeReceive>> GetRsDailySalesPaymentModeData(RsDailySalesSummaryVm vm)
    {
        vm.StrQueryDate = (string.IsNullOrEmpty(vm.StrQueryDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrQueryDate;
        var queryDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrQueryDate)).ToString("dd/MMM/yyyy");
        string query = $@"select convert(date,PaidDate) PaidDate,sum(case when PayMode = 0 then PaidAmount else 0 end) CashPayment,sum(case when PayMode = 1 then PaidAmount else 0 end) BankPayment
        ,sum(case when PayMode = 2 then PaidAmount else 0 end) BkashPayment,sum(case when PayMode = 3 then PaidAmount else 0 end) CardPayment
        ,isnull(sum(PaidAmount),0) TotalPayment
        from RsOrderPayments where convert(date,PaidDate) = '{queryDate}' and IsDeleted = 0 and PaymentType = 0  and BillDtlId is null 
        group by convert(date,PaidDate)";

        var data = await _iReadDbConnection.QueryAsync<PaymentModeReceive>(query);
        return data.ToList();
    }
    #endregion

    #region RsDailySalesSummaryReportHtml

    public async Task<string> RsDailySalesSummaryReportHtml(RsDailySalesSummaryVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await GetRsDailySalesSummaryReportData(vm);

        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";
        fullHtml += $@"<tr style='height:30px;'><td colspan='17' class='text-center'>Date: {vm.StrQueryDate} </td></tr>";
        fullHtml += "<tr style='height:30px;font-size:10px;' class='text-center'>";
        fullHtml += $@"<th rowspan='2' style='width:100px;'>Bill No</th>
                       <th colspan='5' style='text-align:center;'>Cash Sales + Collection</th>
                       <th colspan='8' style='text-align:center;'>Bill Due For Collection & Complimentary</th>
                       <th rowspan='2' style='width:100px;'>Grand Total</th>
                       <th rowspan='2' style='width:100px;'>Customer Type</th>
                       <th rowspan='2' style='width:200px;'>Customer Name</th></tr>";

        fullHtml += $@"<tr style='height:30px;font-size:11px;'><th style='width:80px;'>Cash</th><th style='width:80px;'>Due Collection</th><th style='width:80px;'>Advance</th><th style='width:80px;'>Service Charge</th><th style='width:80px;'>Total</th>";
        fullHtml += $@"<th style='width:80px;'>Adv. Refund</th><th style='width:80px;'>Room No</th><th style='width:80px;'>Due</th>
            <th style='width:80px;'>Compli B/F</th><th style='width:80px;'>MD Sir</th><th style='width:80px;'>GM Sir</th><th style='width:80px;'>Official</th><th style='width:80px;'>Total</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        if (data != null && data.Count > 0)
        {
            fullHtml += "<tbody>";
            for (int j = 0; j < data.Count; j++)
            {
                RsDailySalesSummaryVm objOrder = data[j];
                string dueAmount = (objOrder.CustomerType != "Complimentary" && objOrder.CustomerType != "MD Sir" && objOrder.CustomerType != "GM Sir" && objOrder.CustomerType != "Official") ? objOrder.DueAmount.ToString() : "";
                string complimentary = (objOrder.CustomerType == "Complimentary") ? objOrder.DueAmount.ToString() : "";
                string mdAmount = (objOrder.CustomerType == "MD Sir") ? objOrder.DueAmount.ToString() : "";
                string gmAmount = (objOrder.CustomerType == "GM Sir") ? objOrder.DueAmount.ToString() : "";
                string empAmount = (objOrder.CustomerType == "Official") ? objOrder.DueAmount.ToString() : "";

                if (objOrder.DueCollection > 0 && objOrder.RoomId > 0 && objOrder.BookingId > 0)
                    continue;

                string orderNoHtml = !isPrint ? $"<a target='_blank' href='../FoodOrder/Details/{objOrder.OrderId}'>{objOrder.OrderNo}</a>" : $"{objOrder.OrderNo}";

                fullHtml += "<tr>";
                fullHtml += $@"<td style='text-align:center;'>{orderNoHtml} <br/> {objOrder.TableNo} </td>                              
                                <td style='text-align:right;'>{objOrder.PaidAmount}</td>
                                <td style='text-align:right;'>{objOrder.DueCollection}</td>
                                <td style='text-align:center;'>{objOrder.AdvanceAmount}</td>
                                <td style='text-align:center;'>{objOrder.ServiceCharge}</td>
                                <td style='text-align:right;'>{objOrder.PaidAmount + objOrder.DueCollection + objOrder.AdvanceAmount}</td>
                                <td style='text-align:center;'>{objOrder.AdvanceRefund}</td>
                                <td style='text-align:center;'>{objOrder.RoomNo}</td>
                                <td style='text-align:right;'>{dueAmount}</td>
                                <td style='text-align:right;'>{complimentary}</td>
                                <td style='text-align:right;'>{mdAmount}</td>
                                <td style='text-align:right;'>{gmAmount}</td>
                                <td style='text-align:right;'>{empAmount}</td>
                                <td style='text-align:right;'>{objOrder.DueAmount}</td>
                                <td style='text-align:right;'>{objOrder.PaidAmount + objOrder.DueCollection + objOrder.DueAmount + objOrder.AdvanceAmount - objOrder.AdvanceRefund}</td>
                                <td style='text-align:center;'>{objOrder.CustomerType}</td>
                                <td style='text-align:center;'>{objOrder.CustomerName}</td>";
                fullHtml += "</tr>";

            }

            fullHtml += "</tbody>";
        }

        var dueCollection = data.Where(x => x.RoomId > 0 is false && x.BookingId > 0 is false).Sum(o => o.DueCollection);
        var paidAmount = data.Sum(o => o.PaidAmount);
        var advanceAmount = data.Sum(o => o.AdvanceAmount);
        var collectionTotal = paidAmount + dueCollection + advanceAmount;

        fullHtml += "<tfoot><tr style='height:35px;'>";
        fullHtml += $@"<td style='text-align:right;font-weight:bold;'>Total : </td>                              
                    <td style='text-align:right;'>{data.Sum(o => o.PaidAmount).ToString("N2")}</td>
                    <td style='text-align:right;'>{dueCollection.ToString("N2")}</td>
                    <td style='text-align:center;'></td>
                    <td style='text-align:center;'></td>
                    <td style='text-align:right;'>{collectionTotal.ToString("N2")}</td>
                    <td style='text-align:center;'></td>
                    <td style='text-align:center;'></td>
                    <td style='text-align:right;'>{data.Where(o => o.CustomerType != "Complimentary" && o.CustomerType != "MD Sir" && o.CustomerType != "GM Sir" && o.CustomerType != "Official").Sum(o => o.DueAmount).ToString("N2")}</td>
                    <td style='text-align:right;'>{data.Where(o => o.CustomerType == "Complimentary").Sum(o => o.DueAmount).ToString("N2")}</td>
                    <td style='text-align:right;'>{data.Where(o => o.CustomerType == "MD Sir").Sum(o => o.DueAmount).ToString("N2")}</td>
                    <td style='text-align:right;'>{data.Where(o => o.CustomerType == "GM Sir").Sum(o => o.DueAmount).ToString("N2")}</td>
                    <td style='text-align:right;'>{data.Where(o => o.CustomerType == "Official").Sum(o => o.DueAmount).ToString("N2")}</td>
                    <td style='text-align:right;'>{data.Sum(o => o.DueAmount).ToString("N2")}</td>
                    <td style='text-align:right;'>{(data.Sum(o => o.PaidAmount + o.DueAmount + o.AdvanceAmount - o.AdvanceRefund) + dueCollection).ToString("N2")}</td>";
        fullHtml += "</tr>";
        fullHtml += "</tfoot>";
        fullHtml += "</table>";


        var modeData = await GetRsDailySalesPaymentModeData(vm);
        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:50%;margin-top:50px;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";
        fullHtml += $@"<tr style='height:30px;'><td colspan='5' class='text-center'>Date: {vm.StrQueryDate} </td></tr>";
        fullHtml += "<tr style='height:30px;font-size:10px;' class='text-center'>";
        fullHtml += $@"<th style='width:100px;'>Cash</th><th style='width:100px;'>Card</th><th style='width:100px;'>Bkash</th>
                        <th style='width:100px;'>Bank</th><th style='width:100px;'>Total</th></tr>";
        fullHtml += "</thead>";

        if (modeData.Count > 0)
        {
            fullHtml += "<tbody>";
            fullHtml += "<tr style='height:35px;font-weight:bold;'>";
            fullHtml += $@"<td style='text-align:center;'>{modeData[0].CashPayment.ToString("N2")}</td>                              
                <td style='text-align:center;'>{modeData[0].CardPayment.ToString("N2")}</td>
                <td style='text-align:center;'>{modeData[0].BkashPayment.ToString("N2")}</td>
                <td style='text-align:center;'>{modeData[0].BankPayment.ToString("N2")}</td>
                <td style='text-align:center;'>{modeData[0].TotalPayment.ToString("N2")}</td>";
            fullHtml += "</tr>";
            fullHtml += "</tbody>";
        }
        fullHtml += "</table>";


        return fullHtml;
    }

    #endregion

    #region RsPaymentTransectionReportData
    public async Task<List<RsTransectionReportVm>> RsPaymentTransectionReportAsync(RsTransectionReportVm vm)
    {
        vm.StrFromDate = string.IsNullOrEmpty(vm.StrFromDate) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = string.IsNullOrEmpty(vm.StrToDate) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;

        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");

        string fromDateFilter = !string.IsNullOrEmpty(vm.StrFromDate) ? $" and convert(date, op.PaidDate) >= '{fromDate}'" : "";
        string toDateFilter = !string.IsNullOrEmpty(vm.StrToDate) ? $" and convert(date, op.PaidDate) <= '{toDate}'" : "";
        string customerFilter = (vm.CustomerId > 0) ? $" and rc.Id = {vm.CustomerId}" : "";
        string customerTypeFilter = (vm.CustomerTypeId > 0) ? $" and ct.Id = {vm.CustomerTypeId}" : "";

        string query = $@"select 
                            op.Id RsPaymentId,
                            fo.Id OrderId,
                            fo.OrderNo,
                            fo.OrderDate,
                            op.PaidDate,
                            ISNULL(rc.Salutation,'') + ' ' + ISNULL(rc.FirstName,'') + ' ' + ISNULL(rc.LastName,'') CustomerName,
                            rc.Mobile,
                            ct.TypeName as CustomerType,
                            rw.Name as WaiterName,
                            fo.RoomId,
                            ri.RoomNo,
                            rt.TableNo,
                            fo.PaymentStatus,
                            op.PayMode,
                            op.Remarks,
                            fo.OrderAmount,
                            fo.Discount,
                            fo.ServiceCharge,
                            fo.VAT,
                            fo.NetAmount,
                            op.PaidAmount, 
                            op.BillDtlId,
                            op.AuditDate,
                            op.AuditById,
                            u.FullName AuditBy,
                            op.AuditRemarks

                        from RsOrderPayments op
                        left join RsFoodOrders fo on fo.id = op.OrderId
                        left join AspNetUsers u on u.Id = op.AuditById 
                        left join RsCustomers rc on rc.id = fo.CustomerId 
                        left join RsCustomerTypes ct on ct.id = fo.CustomerTypeId
                        left join HtRoomInfos ri on ri.id = fo.RoomId
                        left join RsWaiters rw on rw.id = fo.WaiterId
                        left join RsTables rt on rt.id = fo.TableId 
                        where op.IsDeleted = 0 
                            {customerFilter} 
                            {customerTypeFilter} 
                            {fromDateFilter} 
                            {toDateFilter}
                        order by op.PaidDate desc";

        var data = await _iReadDbConnection.QueryAsync<RsTransectionReportVm>(query);
        return data.ToList();
    }

    #endregion 

}
