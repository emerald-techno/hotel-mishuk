using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Billing;
using Interface.Repository.HotelManagement;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.HotelManagement;

public class BillingRepository : BaseRepository<HtBilling>, IBillingRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public BillingRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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

    public async Task<DataTablePagination<BillingSearchVm, BillingSearchVm>> SearchAsync(DataTablePagination<BillingSearchVm, BillingSearchVm> vm)
    {
        var searchResult = Context.HtBillings
            .Include(d => d.BillingDetails)
                .ThenInclude(br => br.BookingRoom)
                    .ThenInclude(r => r.Room)
            .Include(b => b.Booking)
            .Include(b => b.BillBy)
            .AsQueryable()
            .Where(c => !c.IsDeleted);

        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search bed type not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.BillNumber.ToLower().Contains(value) || c.BillingDetails.Any(x => x.BookingRoom.Room.RoomNo.Contains(value)));
        }

        if (!string.IsNullOrEmpty(model.BillNo))
        {
            searchResult = searchResult.Where(c => c.BillNumber.ToLower().Contains(model.BillNo.ToLower()));
        }

        if (!string.IsNullOrEmpty(model.BookingNo))
        {
            searchResult = searchResult.Where(c => c.Booking.BookingNo.ToLower().Contains(model.BookingNo.ToLower()));
        }

        if (model.BillStatus != null)
        {
            searchResult = searchResult.Where(c => c.BillStatus == model.BillStatus);
        }

        if (!string.IsNullOrEmpty(model.FormDateStr))
        {
            var formDate = (DateTime)(!string.IsNullOrEmpty(model.FormDateStr) ? Utility.ConvertStrToDate(model.FormDateStr) : model.BillDate);
            searchResult = searchResult.Where(c => c.BillDate.Date >= formDate.Date);
        }

        if (!string.IsNullOrEmpty(model.ToDateStr))
        {
            var toDate = (DateTime)(!string.IsNullOrEmpty(model.ToDateStr) ? Utility.ConvertStrToDate(model.ToDateStr) : model.BillDate);
            searchResult = searchResult.Where(c => c.BillDate.Date <= toDate.Date);
        }

        if (model.IsOnlyCheckOutBill)
        {
            searchResult = searchResult.Where(c => c.Booking.BookingStatus != BookingServiceStatusEnum.Booked 
            && c.Booking.BookingStatus != BookingServiceStatusEnum.CheckIn);
        }

        var totalRecords = searchResult.Count();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderByDescending(c => c.BillDate)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<BillingSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.BookingNo = filterData.Booking.BookingNo;
                searchDto.BookingDate = filterData.Booking.BookingDate;
                searchDto.BillByFullName = filterData.BillBy.FullName;
                searchDto.BillNo = filterData.BillNumber;
                searchDto.TotalAmount = filterData.TotalAmount;

                var roomList = filterData.BillingDetails.Select(x => x.BookingRoom?.Room?.RoomNo).ToList();
                searchDto.RoomList = string.Join(", ", roomList);
            }
        }
        return vm;
    }

    #endregion

    #region GetBillInfo

    public async Task<HtBilling> GetBillByIdAsync(long id)
    {
        var data = await Context.HtBillings
            .Include(b => b.Booking)
            .Include(b => b.BillBy)
            .Include(c => c.BillingDetails)
                .ThenInclude(e => e.BookingRoom)
                    .ThenInclude(d => d.Room)
                        .ThenInclude(d => d.RoomCategory)
            .Include(c => c.BillingDetails)
                .ThenInclude(e => e.Service)
                    .ThenInclude(s => s.Ledger)
            .Include(c => c.BillingDetails)
                .ThenInclude(e => e.BookingHall)
                    .ThenInclude(d => d.Hall)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (data == null) throw new Exception("No Bill Data Found");

        return data;
    }

    #endregion

    #region BillDetailInfo
    private static List<DateTime> GetDateList(DateTime startDate, DateTime endDate)
    {
        startDate = startDate.Date;
        endDate = endDate.Date;
        List<DateTime> dateList = new List<DateTime>();

        if (startDate <= endDate)
        {
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                dateList.Add(date);
            }
        }
        return dateList;
    }
    public async Task<string> GetBillDetailHtmlById(BillingVm vm)
    {
        try
        {
            string leftAlign = "text-align:left;";
            string rightAlign = "text-align:right;";
            string noBorder = "border:0px;";
            string boldText = "font-weight:bold;";
            string fullHtml = "";
            List<DateTime> dateList = GetDateList(Convert.ToDateTime(vm.BillingDetails.Min(o => o.BookingRoomCheckInTime)), Convert.ToDateTime(vm.BillingDetails.Max(o => o.BookingRoomCheckOutTime)));

            //var paidList = Context.HtBookingPayments.Where(x => x.BookingId == vm.BookingId && !x.IsDeleted).ToList();
            var paidList = vm.BillingPayments;
            //var netPayableAmount = vm.NetAmount > vm.PaidAmount ? (vm.NetAmount - vm.PaidAmount) : 0;

            var netPayableAmount = vm.NetAmount > vm.AdvanceAmount ? (vm.NetAmount - vm.AdvanceAmount) : 0;

            var totalPayment = paidList.Sum(x => x.PaidAmount);
            var remainAmount = vm.NetAmount >= totalPayment ? (vm.NetAmount - totalPayment) : 0;
            if (remainAmount > 0)
            {
                netPayableAmount = (double)remainAmount;
            }

            var checkInTime = vm.BillingDetails.Min(o => o.BookingRoomCheckInTime);
            var checkOutTime = vm.BillingDetails.Min(o => o.BookingRoomCheckOutTime);

            var checkInTimeStr = checkInTime != null ? checkInTime?.ToString("dd/MM/yyyy hh:mm tt") : "";
            var checkOutTimeStr = checkOutTime != null ? checkOutTime?.ToString("dd/MM/yyyy hh:mm tt") : "";

            fullHtml += $@"<table class='table table-striped table-bordered table-bill' id='print_table' style='width:100%;margin-bottom:4px;repeat-header:yes;' >
            <tr style='height:30px;'>
                  <td colspan='5' style='border:0px;'>
                       <table style='width:100%;border:0px;' class='table-bill' border='0' >
                            <tr style='height:17px;'><td style='width:15%;{leftAlign + noBorder + boldText}'> Bill No</td><td style='{leftAlign + noBorder + boldText}'>: {vm.BillNumber}</td></tr>
                            <tr style='height:17px;'><td style='width:15%;{leftAlign + noBorder + boldText}'> Bill Date </td><td style='{leftAlign + noBorder}'>: {vm.BillDate}</td></tr>
                            <tr style='height:17px;'><td style='width:15%;{leftAlign + noBorder + boldText}'> Guest Name</td><td style='{leftAlign + noBorder + boldText}'>: {vm.BookingGuestName}</td></tr>
                            <tr style='height:17px;'><td style='width:15%;{leftAlign + noBorder + boldText}'> Mobile</td><td style='{leftAlign + noBorder}'>: {vm.BookingGuestMobile}</td></tr>
                            <tr style='height:17px;'><td style='width:15%;{leftAlign + noBorder + boldText}'> Address</td><td style='{leftAlign + noBorder}'>: {vm.BookingGuestAddress}</td></tr>
                            <tr style='height:17px;'><td style='width:15%;{leftAlign + noBorder + boldText}'> Booking No</td><td style='{leftAlign + noBorder}'>: {vm.BookingNo}</td></tr>
                      </table>
                  </td>
                  <td colspan='3' style='width:35%;border:0px;' class='table-bill' > 
                         <table style='width:100%'>
                            <tr style='height:17px;'><td style='width:30%;{leftAlign + noBorder + boldText}'> Check In</td><td style='{leftAlign + noBorder}'>: {checkInTimeStr}</td></tr>
                            <tr style='height:17px;'><td style='width:30%;{leftAlign + noBorder + boldText}'> Check Out </td><td style='{leftAlign + noBorder}'>: {checkOutTimeStr}</td></tr>
                            <tr style='height:17px;'><td style='width:30%;{leftAlign + noBorder + boldText}'> Pax</td><td style='{leftAlign + noBorder}'>: {vm.TotalGuest}</td></tr>
                            <tr style='height:17px;'><td style='width:30%;{leftAlign + noBorder + boldText}'> Total Stay</td><td style='{leftAlign + noBorder}'>: {vm.DurationText}</td></tr>
                            <tr style='height:17px;'><td style='width:30%;{leftAlign + noBorder + boldText}'> Bill Status</td><td style='{leftAlign + noBorder}'><div class='{vm.BillStatusClass}'> : {vm.BillStatusText} </div> </td></tr>
                            <tr style='height:17px;'><td style='width:30%;{leftAlign + noBorder + boldText}'> Bill By</td><td style='{leftAlign + noBorder}'>: {vm.BillByName}</td></tr>
                        </table>
                  </td>
            </tr>
            <tr style='height:35px;'>
                <td style='width:10%!important;{leftAlign + boldText}'>Rooms ({vm.BillingDetails?.Where(o => o.ServiceId == 1).ToList().Count})</td>
                <td colspan='7' style='padding:7px;{leftAlign};width:90%'> : {string.Join(",", vm.BillingDetails.Select(o => o.BookingRoomNo))}</td>
            </tr>
            <tr style='height:35px;'>
                <td style='{leftAlign + boldText}'>Note </td>
                <td colspan='7' style='padding:7px;{leftAlign}'> : {vm.Remarks}</td>
            </tr>
            </table>
            ";

            fullHtml += $@"<table class='table table-striped table-bordered table-bill' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;'>";
            fullHtml += $@" <tr style='height:30px;background-color:#F2F3F2;'>
            <th style='width:6%;'>#</th>
            <th colspan='4' style='text-align:left;padding:3px;width:60%'>Particulars</th>
            <th class='text-center' style='width:8%;'>Quantity</th>
            <th class='text-center' style='width:14%;'>Rate</th>
            <th class='text-center' style='width:12%;'>Amount</th>
            </tr>";
            for (int i = 0; i < dateList.Count; i++)
            {
                var dateDataList = vm.BillingDetails.Where(o => Convert.ToDateTime(o.BookingRoomCheckInTime).Date <= dateList[i].Date && dateList[i].Date < Convert.ToDateTime(o.BookingRoomCheckOutTime).Date && o.ServiceId == 1).ToList();
                var dateHalfDayList = vm.BillingDetails.Where(o => dateList[i].Date == Convert.ToDateTime(o.BookingRoomCheckOutTime).Date && o.ServiceId == 1 && o.BookingDayStatus == 1).ToList();
                var dateDayUseList = vm.BillingDetails.Where(o => dateList[i].Date == Convert.ToDateTime(o.BookingRoomCheckOutTime).Date && o.ServiceId == 1 && o.BookingDayStatus == 2).ToList();

                if (dateDataList.Count > 0 || dateHalfDayList.Count > 0 || dateDayUseList.Count > 0)
                {
                    fullHtml += $@"<tr style='height:35px;'><td colspan='8' style='padding:7px;{leftAlign + boldText}'> Day {i + 1} | {DU.Utility.ConvertDateToStr(dateList[i])}  </td></tr>";
                    var CategorySummaryList = dateDataList.Concat(dateHalfDayList).Concat(dateDayUseList).Select(o => new { o.BookingRoomCategoryId, o.BookingRoomCategoryName, o.Rate }).Distinct().ToList();
                    decimal dayTotalAmount = 0;
                    for (int j = 0; j < CategorySummaryList.Count; j++)
                    {
                        decimal roomQuantity = dateDataList.Where(o => o.BookingRoomCategoryId == CategorySummaryList[j].BookingRoomCategoryId && o.Rate == CategorySummaryList[j].Rate).ToList().Count;
                        decimal halfDayQuantity = dateHalfDayList.Where(o => o.BookingRoomCategoryId == CategorySummaryList[j].BookingRoomCategoryId && o.Rate == CategorySummaryList[j].Rate).ToList().Count;
                        decimal dayUseQuantity = dateDayUseList.Where(o => o.BookingRoomCategoryId == CategorySummaryList[j].BookingRoomCategoryId && o.Rate == CategorySummaryList[j].Rate).ToList().Count;

                        roomQuantity = roomQuantity + dayUseQuantity;
                        roomQuantity = (halfDayQuantity > 0) ? roomQuantity + (halfDayQuantity * Convert.ToDecimal(0.5)) : roomQuantity;

                        decimal roomRent = Convert.ToDecimal(CategorySummaryList[j].Rate);
                        decimal roomAmount = roomQuantity * roomRent;
                        dayTotalAmount += roomAmount;
                        fullHtml += $@"<tr style='height:32px;'>
                        <td style=''>{j + 1}</td>
                        <td colspan='4' style='text-align:left;'>{CategorySummaryList[j].BookingRoomCategoryName}</td>
                        <td style='text-align:center;'>{roomQuantity}</td>
                        <td style='text-align:right;'>{roomRent:N2}</td>
                        <td style='text-align:right;'>{roomAmount:N2}</td>
                        </tr>";
                    }
                    ////day wise extra bed charge
                    decimal extraBedCharge = 0;
                    //decimal extraBedCharge = (decimal)dateDataList.Concat(dateDayUseList).Concat(dateHalfDayList).Sum(o => o.ExtraBedCharge);
                    //if (extraBedCharge > 0)
                    //{
                    //    decimal extraBedCount = dateDataList.Concat(dateDayUseList).Where(o => o.ExtraBedCharge > 0).ToList().Count;
                    //    decimal halfDayExtraBedCount = dateHalfDayList.Where(o => o.ExtraBedCharge > 0).ToList().Count;
                    //    extraBedCount = (halfDayExtraBedCount > 0) ? extraBedCount + (halfDayExtraBedCount * (decimal)0.5) : extraBedCount;
                    //    decimal extraBedRate = 500;
                    //    extraBedCharge = extraBedCount * extraBedRate;
                    //    fullHtml += $@"<tr style='height:32px;'>
                    //    <td style='width:50px;'>{CategorySummaryList.Count}</th>
                    //    <td colspan='4' style='text-align:left;'>Extra Bed Charge</td>
                    //    <td style='text-align:right;'>{extraBedCount}</td>
                    //    <td style='text-align:right;'>{extraBedRate:N2}</td>
                    //    <td style='text-align:right;'>{extraBedCharge:N2}</td>
                    //    </tr>";
                    //}

                    fullHtml += $@"<tr style='height:32px;'>
                    <td colspan='5'></td>
                    <td colspan='2' style='text-align:right;font-weight:bold;'>Day Total : </td>
                    <td style='text-align:right;'>{dayTotalAmount + extraBedCharge:N2}</td>
                    </tr>";
                }

            }
            //Other Service Bills
            if (vm.BillingDetails.Where(o => o.ServiceId != 1).ToList().Count > 0 || vm.BillingDetails.Sum(o => o.ExtraBedCharge) > 0)
            {
                fullHtml += $@"<tr style='height:35px;'><td colspan='8' style='padding:7px;'> Other Services </td></tr>";
                var otherServiceList = vm.BillingDetails.Where(o => o.ServiceId != 1).ToList();
                decimal totalExtraBedCharge = (decimal)vm.BillingDetails.Sum(o => o.ExtraBedCharge);
                decimal totalExtraBedCount = (decimal)vm.BillingDetails.Where(o => o.ExtraBedCharge > 0).Sum(o => o.Days);
                for (int s = 0; s < otherServiceList.Count; s++)
                {
                    fullHtml += $@"<tr style='height:32px;'>
                    <td style='width:50px;'>{s + 1}</td>
                    <td colspan='4' style='text-align:left;'>{otherServiceList[s].ServiceName}</td>
                    <td style='text-align:center;'>{otherServiceList[s].Quantity}</td>
                    <td style='text-align:right;'>{otherServiceList[s].Rate:N2}</td>
                    <td style='text-align:right;'>{otherServiceList[s].NetAmount:N2}</td>
                    </tr>";
                }
                if (totalExtraBedCharge > 0)
                {
                    fullHtml += $@"<tr style='height:32px;'>
                    <td style='width:50px;'>{otherServiceList.Count + 1}</td>
                    <td colspan='4' style='text-align:left;'>Extra Bed Charge</td>
                    <td style='text-align:right;'></td>
                    <td style='text-align:right;'></td>
                    <td style='text-align:right;'>{totalExtraBedCharge:N2}</td>
                    </tr>";
                }

                fullHtml += $@"<tr style='height:32px;'>
                    <td colspan='5'></td><td colspan='2' style='text-align:right;font-weight:bold;'>Other Service Total : </td>
                    <td style='text-align:right;'>{otherServiceList.Sum(s => s.NetAmount) + (double)totalExtraBedCharge:N2}</td>
                    </tr>";
            }

            var paymentHtml = "";

            if (paidList?.Count > 0)
            {
                paymentHtml += $@"<table style='width:100%'><tbody>";

                foreach (var payment in paidList)
                {
                    var payMode = payment.PayMode.GetDescription();

                    var mr = !string.IsNullOrEmpty(payment.TransactionNo) ? $"({payment.TransactionNo})" : "";

                    paymentHtml += $@"<tr>
                                      <td style='width:15%;{leftAlign + noBorder + boldText}'>{payment.PaidDate?.ToString("dd-MMM-yyyy")}</td>
                                      <td style='width:15%;{leftAlign + noBorder}'>{payMode}</td>
                                      <td style='width:50%;{leftAlign + noBorder}'>{payment.PaidAmount?.ToString("N2")}  {mr}</td>
                                    </tr>";
                }

                paymentHtml += $@"</tbody></table>";
            }

            var grandTotal = vm.BillingDetails.Where(c => c.ServiceId == 1).Sum(x => x.NetAmount + x.Discount) + vm.BillingDetails.Where(c => c.ServiceId != 1).Sum(x => x.NetAmount);

            fullHtml += $@"<tr style='height:32px;'>
                    <td colspan='5' rowspan='8'>{paymentHtml}</td>
                    <td colspan='2' style='text-align:right;'>Grand Total : </td>
                    <td style='text-align:right;font-weight:bold;'>{grandTotal:N2}</td>
                    </tr>";
            fullHtml += $@"<tr style='height:32px;'>
                    <td colspan='2' style='text-align:right;'>(+)VAT : </td>
                    <td style='text-align:right;font-weight:bold;'>{vm.Vat:N2}</td>
                    </tr>";
            //fullHtml += $@"<tr style='height:32px;'>
            //        <td colspan='2' style='text-align:right;'>(+) Tax : </td>
            //        <td style='text-align:right;font-weight:bold;'>{vm.Tax:N2}</td>
            //        </tr>";

            var roomServiceCharge = vm.BillingDetails.Where(c => c.ServiceId == 1 || c.ServiceId == 4).Sum(o => o.ServiceCharge);

            fullHtml += $@"<tr style='height:32px;'>
                    <td colspan='2' style='text-align:right;'>(+) Service Charge : </td>
                    <td style='text-align:right;font-weight:bold;'>{roomServiceCharge:N2}</td>
                    </tr>
            ";

            var subTotal = vm.BillingDetails.Sum(x => x.Amount + x.ExtraBedCharge + x.ServiceCharge);

            var detailDiscount = vm.BillingDetails.Sum(x => x.Discount);

            var roomTotal = vm.BillingDetails.Where(v => v.ServiceId == 1 || v.ServiceId == 4).Sum(x => x.Amount + x.ExtraBedCharge + x.ServiceCharge);

            var otherServiceDiscount = vm.BillingDetails.Where(v => v.ServiceId != 1 || v.ServiceId != 4).Sum(x => x.Discount);
            var masterRoomDiscount = !(vm.Id > 0) ? (vm.Discount - otherServiceDiscount) : vm.Discount;
            var detailRoomDiscount = vm.BillingDetails.Where(v => v.ServiceId == 1 || v.ServiceId == 4).Sum(x => x.Discount);


            var totalDiscount = masterRoomDiscount > 0 ? masterRoomDiscount : detailRoomDiscount > 0 ? detailRoomDiscount : 0;

            double discountPercent = AppUtility.CalculatePercentage(totalDiscount, roomTotal);

            //if (roomTotal > 0 && detailRoomDiscount > 0)
            //{
            //    discountPercent = AppUtility.CalculatePercentage(detailRoomDiscount, roomTotal);
            //}

            string discountPercentText = $"({discountPercent.ToString("F2")} %)";

            fullHtml += $@"<tr style='height:32px;'>
                    <td colspan='2' style='text-align:right;'>{discountPercentText} (-) Room Discount : </td>
                    <td style='text-align:right;font-weight:bold;'>{(masterRoomDiscount > 0 ? masterRoomDiscount : detailRoomDiscount):N2}</td>
                    </tr>";
            fullHtml += $@"<tr style='height:32px;'>
                    <td colspan='2' style='text-align:right;'>(-) Adjustment Discount : </td>
                    <td style='text-align:right;font-weight:bold;'>{(vm.SpecialDiscount):N2}</td>
                    </tr>";
            fullHtml += $@"<tr style='height:32px;'>
                    <td colspan='2' style='text-align:right;'>Net Payable Amount : </td>
                    <td  style='text-align:right;font-weight:bold;'>{vm.NetAmount:N2}</td>
                    </tr>";
            fullHtml += $@"<tr style='height:32px;'>
                    <td colspan='2' style='text-align:right;'>(-) Deposit Paid :<br /> {vm.MrList} </td>
                    <td  style='text-align:right;font-weight:bold;'>{vm.AdvanceAmount:N2}</td>
                    </tr>";
            fullHtml += $@"<tr style='height:32px;'>
                    <td colspan='2' style='text-align:right;'>(-) Total Paid :<br /> </td>
                    <td  style='text-align:right;font-weight:bold;'>{vm.PaidAmount:N2}</td>
                    </tr>";
            fullHtml += $@"<tr style='height:40px;background-color:#F2F3F2;'>
                    <td colspan='5' rowspan='1' style='vertical-align:middle;font-weight:bold;font-size:12px;text-align:left;padding:6px;'>
                        Taka  {Utility.ConvertToWordInt(netPayableAmount)}.
                    </td>
                    <td colspan='2' style='text-align:right;'> Balance to be paid in Full : </td>
                    <td style='text-align:right;font-weight:bold;'>{netPayableAmount:N2}</td>
                    </tr>";
            //fullHtml += $@"<tr style='height:32px;'>
            //        <td colspan='2' style='text-align:right;font-weight:bold;'> Total Paid Amount : </td>
            //        <td style='text-align:right;'>{vm.PaidAmount:N2}</td>
            //        </tr>";


            fullHtml += "</table>";

            return fullHtml;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    #endregion
}