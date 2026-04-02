using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.ViewModel.HotelManagement.HotelReport;
using Interface.Repository.HotelManagement;
using Persistence.DapperModel;
using System.Text;
using DU = Domain.Utility;

namespace Repository.HotelManagement;

public class HotelManagementRepository : IHotelManagementRepository
{
    #region Config
    private readonly IMapper _iMapper;
    private readonly IApplicationReadDbConnection _iReadDbConnection;
    public HotelManagementRepository(IMapper iMapper, IApplicationReadDbConnection iReadDbConnection)
    {
        _iMapper = iMapper;
        _iReadDbConnection = iReadDbConnection;
    }
    #endregion

    #region GetBookingServiceData

    public async Task<List<BookingServiceReportVm>> GetBookingServiceData(BookingServiceReportVm vm)
    {
        string categoryFilter = (vm.RoomCategoryId > 0) ? $" and rc.Id = {vm.RoomCategoryId}" : "";
        string roomFilter = (vm.RoomId > 0) ? $" and ri.Id = {vm.RoomId}" : "";
        string floorFilter = (vm.FloorId > 0) ? $" and fi.Id = {vm.FloorId}" : "";
        string bookingStatusFilter = (vm.BookingStatus > 0) ? $" and hb.BookingStatus = {vm.BookingStatus}" : "";
        //var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        //var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");

        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");

        string query = $@"select rc.Id RoomCategoryId,rc.CategoryName RoomCategory,ri.Id RoomId,ri.RoomNo,fi.Id FloorId,fi.FloorName,hr.CheckInTime,hr.CheckOutTime,hr.ActualCheckInTime,hr.ActualCheckOutTime,hr.Rent,hr.ServiceCharge,
            hr.NetRent,hr.Adult,hr.Child,hb.Id BookingId,hb.BookingDate,hb.BookingNo,hb.BookingStatus,hb.BookingType,hp.PaidAmount,
            case when hb.BookingStatus = 1 then 'Pending' when hb.BookingStatus = 2 then 'Approved' when hb.BookingStatus = 3 then 'Checkin' when hb.BookingStatus = 4 then 'Checkout' when hb.BookingStatus = 9 then 'Cenceled' else '' end 
            BookingStatusName,hb.PaymentStatus,case when hb.PaymentStatus = 0 then 'Pending' when hb.PaymentStatus = 1 then 'Partial Payment' when hb.PaymentStatus = 2 then 'Full Payment' else '' end PaymentStatusName
            ,ui.id BookingById,ui.FullName BookingBy,case when hr.ActualCheckInTime is not null then hr.ActualCheckInTime else hr.CheckInTime end DisplayCheckInTime
            ,case when hr.ActualCheckOutTime is not null then hr.ActualCheckOutTime else hr.CheckOutTime end DisplayCheckOutTime
            from HtBookingRooms hr
            inner join HtBookingServices hb on hb.Id = hr.BookingId
            inner join HtRoomCategories rc on rc.Id = hr.RoomCategoryId
            left join HtRoomInfos ri on ri.Id = hr.RoomId
            left join HtFloorInfos fi on fi.Id = ri.FloorId
            left join (select BookingId,sum(PaidAmount) PaidAmount from HtBookingPayments  group by BookingId) hp on hp.BookingId = hb.Id
            left join AspNetUsers ui on ui.Id = hb.ActionById 
            where hr.IsDeleted =0 {categoryFilter} {roomFilter} {floorFilter} {bookingStatusFilter} and hr.CheckInTime >= '{fromDate}' and hr.CheckInTime <= '{toDate}'
            order by ri.RoomNo";

        var data = await _iReadDbConnection.QueryAsync<BookingServiceReportVm>(query);
        return data.ToList();
    }

    #endregion

    #region GetBookingReportHtml

    public async Task<string> GetBookingReportHtml(BookingServiceReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await GetBookingServiceData(vm);
        if (data != null && data.Count > 0)
        {
            // === Header Section ===
            if (isPrint)
            {
                fullHtml += $@"<div style='text-align:center;margin-bottom:20px;'>
                                    <p style='margin:4px 0;color:#555;font-size:14px;'>
                                        <b>From:</b> {vm.StrFromDate}  |  <b>To:</b> {vm.StrToDate}
                                    </p>
                                    <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                                </div>";
            }

            fullHtml += "<table class='table table-bordered report-table' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += "<tr style='height:30px;'>";

            fullHtml += $@"<th style='width:50px;'>SL No</th>
                            <th style='width:250px;'>Room Info</th>
                            <th style='width:150px;'>Booking Info</th>
                            <th style='width:150px;'>Entry By</th>
                            <th style='width:140px;'>Booked From</th>
                            <th style='width:140px;'>Booked To</th>
                            <th style='width:140px;'>Check In</th>
                            <th style='width:140px;'>Check Out</th>
                            <th style='width:200px;'>Price</th>
                            <th style='width:150px;'>Booking Status</th>
                            <th style='width:150px;'>Payment Status</th>";

            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            for (int i = 0; i < data.Count; i++)
            {
                BookingServiceReportVm objBooking = data[i];

                string rowBookingNo = !isPrint ? $"<a target='_blank' href='../BookingService/Details/{objBooking.BookingId}'>{objBooking.BookingNo}</a>" : $"{objBooking.BookingNo}";

                fullHtml += "<tr>";

                fullHtml += $@"<td style='text-align:left;'>{i + 1}</td>
                                <td style='text-align:left;'><b>{objBooking.RoomNo}</b><br/>{objBooking.RoomCategory}<br/>{objBooking.FloorName}</td>
                                <td style='text-align:left;padding:5px;'><b>{rowBookingNo}</b><br />{DU.Utility.ConvertDateToStr(objBooking.BookingDate)}</td>
                                <td style='text-align:center;'>{objBooking.BookingBy}</td>                                
                                <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckInTime)}</td>
                                <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckOutTime)}</td>
                                <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.ActualCheckInTime)}</td>
                                <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.ActualCheckOutTime)}</td>
                                <td style='text-align:left;'>Rent: {objBooking.Rent}<br/>Service: {objBooking.ServiceCharge}<br/>Net Rent: {objBooking.NetRent} </td>
                                <td style='text-align:center;'>{objBooking.BookingStatusName}</td>
                                <td style='text-align:center;'>{objBooking.PaymentStatusName}</td>";

                fullHtml += "</tr>";

            }
        }
        fullHtml += "</tbody>";

        fullHtml += "</table>";

        // === Footer Section ===
        fullHtml += @"
            <div class='report-footer'>
                <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
                <p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

        fullHtml += @"
            </div>";

        return fullHtml;
    }

    #endregion

    #region GetBookingServiceData

    public async Task<List<BookingDailySalesReportVm>> GetBookingDailySalesData(BookingDailySalesReportVm vm)
    {
        vm.StrQueryDate = (string.IsNullOrEmpty(vm.StrQueryDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrQueryDate;
        var queryDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrQueryDate)).ToString("dd/MMM/yyyy");

        #region v1
        //string query = $@"select bs.Id BookingId, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        //,hr.ActualCheckOutTime CheckOutTime,hr.TotalGuest,r.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,hr.ExtraBedCharge,bs.Discount,hr.NetRent
        //,ab.Amount AdvanceAmount,b.BillNumber,datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) TotalStay,b.FoodBill
        //,isnull(af.RefundAmount,0) AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,e.ExtraCharge
        //FROM HtBookingServices bs
        //inner join HtBookingRooms hr on hr.BookingId = bs.Id
        //inner join HtRoomInfos r on r.Id = hr.RoomId
        //inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000001' then d.NetAmount else 0 end) RoomBill
        //,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode in ('SR000001','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(d.NetAmount) ExtraCharge
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode = 'SR000003' group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) e on e.BookingId = bs.Id 
        //left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        //left join HtGuestInfos g on g.Id= bg.GuestId
        //left join (select sum(PaidAmount) Amount,BookingId from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
        //left join (select BookingId,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId) af on af.BookingId = bs.Id
        //left join (select p.BookingId,sum(isnull(ReceiveAmount,0) - isnull(RefundAmount,0)) NetReceive
        //from (
        //select BookingId,sum(PaidAmount) ReceiveAmount,0 RefundAmount from HtBookingPayments group by BookingId
        //union all
        //select BookingId,0 ReceiveAmount,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId
        //)p group by p.BookingId
        //)p on p.BookingId = bs.Id
        //where bs.IsDeleted = 0 and convert(date,hr.ActualCheckOutTime) = '{queryDate}'";
        #endregion

        #region v2
        //string query = $@"select bs.Id BookingId, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        //    ,hr.ActualCheckOutTime CheckOutTime,hr.TotalGuest,r.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,hr.ExtraBedCharge,bs.Discount,hr.NetRent
        //    ,ab.Amount AdvanceAmount,b.BillNumber,datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) TotalStay,b.FoodBill
        //    ,isnull(af.RefundAmount,0) AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,e.ExtraCharge
        //    ,(select convert(date,max(ActualCheckOutTime)) from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0) MaxCheckOutTime
        //    FROM HtBookingServices bs
        //    inner join HtBookingRooms hr on hr.BookingId = bs.Id
        //    inner join HtRoomInfos r on r.Id = hr.RoomId
        //    inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        //    left join (
        //    select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000001' then d.NetAmount else 0 end) RoomBill
        //    ,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill
        //    from HtBillingDetails d
        //    inner join HtBillings b on b.Id = d.BillId
        //    inner join HtServices s on s.Id = d.ServiceId
        //    where b.BookingId > 0 and s.ServiceCode in ('SR000001','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //    ) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        //    left join (
        //    select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(d.NetAmount) ExtraCharge
        //    from HtBillingDetails d
        //    inner join HtBillings b on b.Id = d.BillId
        //    inner join HtServices s on s.Id = d.ServiceId
        //    where b.BookingId > 0 and s.ServiceCode = 'SR000003' group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //    ) e on e.BookingId = bs.Id 
        //    left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        //    left join HtGuestInfos g on g.Id= bg.GuestId
        //    left join (select sum(PaidAmount) Amount,BookingId from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
        //    left join (select BookingId,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId) af on af.BookingId = bs.Id
        //    left join (select p.BookingId,sum(isnull(ReceiveAmount,0) - isnull(RefundAmount,0)) NetReceive
        //    from (
        //    select BookingId,sum(PaidAmount) ReceiveAmount,0 RefundAmount from HtBookingPayments group by BookingId
        //    union all
        //    select BookingId,0 ReceiveAmount,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId
        //    )p group by p.BookingId
        //    )p on p.BookingId = bs.Id
        //    where bs.IsDeleted = 0 and (select convert(date,max(ActualCheckOutTime)) from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0) = '{queryDate}'";
        #endregion

        #region v3
        //string query = $@"select bs.Id BookingId, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        //,hr.ActualCheckOutTime CheckOutTime,hr.TotalGuest,r.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,hr.ExtraBedCharge,bs.Discount,hr.NetRent
        //,ab.Amount AdvanceAmount,b.BillNumber
        //,case when datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) = 0 then case when hr.IsHalfDay = 1 then 0.5 else 1 end 
        //else case when hr.IsHalfDay = 1 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 0.5 else datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) end
        //end TotalStay
        //,b.FoodBill,isnull(af.RefundAmount,0) AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,e.ExtraCharge
        //,(select convert(date,max(ActualCheckOutTime)) from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0) MaxCheckOutTime
        //FROM HtBookingServices bs
        //inner join HtBookingRooms hr on hr.BookingId = bs.Id
        //inner join HtRoomInfos r on r.Id = hr.RoomId
        //inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000001' then d.NetAmount else 0 end) RoomBill
        //,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode in ('SR000001','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(d.NetAmount) ExtraCharge
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode = 'SR000003' group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) e on e.BookingId = bs.Id 
        //left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        //left join HtGuestInfos g on g.Id= bg.GuestId
        //left join (select sum(PaidAmount) Amount,BookingId from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
        //left join (select BookingId,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId) af on af.BookingId = bs.Id
        //left join (select p.BookingId,sum(isnull(ReceiveAmount,0) - isnull(RefundAmount,0)) NetReceive
        //from (
        //select BookingId,sum(PaidAmount) ReceiveAmount,0 RefundAmount from HtBookingPayments group by BookingId
        //union all
        //select BookingId,0 ReceiveAmount,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId
        //)p group by p.BookingId
        //)p on p.BookingId = bs.Id
        //where bs.IsDeleted = 0 and (select convert(date,max(ActualCheckOutTime)) from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0) = '{queryDate}'";
        #endregion

        #region Main
        //string query = $@"select bs.Id BookingId, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        //,hr.ActualCheckOutTime CheckOutTime,hr.TotalGuest,hr.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,hr.ExtraBedCharge,bs.Discount,hr.NetRent
        //,ab.Amount AdvanceAmount,b.BillNumber,
        //case when datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) = 0 then 
        //case when hr.BookingDayStatus = 1 then 0.5  else 1 end 
        //else 
        //case when BookingDayStatus = 1 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 0.5 
        //when BookingDayStatus = 2 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 1 
        //else datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) end
        //end TotalStay
        //,b.FoodBill,isnull(af.RefundAmount,0) AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,e.ExtraCharge
        //,(select convert(date,max(ActualCheckOutTime)) from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0) MaxCheckOutTime
        //FROM HtBookingServices bs
        //inner join HtBookingRooms hr on hr.BookingId = bs.Id
        //inner join HtRoomInfos r on r.Id = hr.RoomId
        //inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000001' then d.NetAmount else 0 end) RoomBill
        //,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode in ('SR000001','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(d.NetAmount) ExtraCharge
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode = 'SR000003' group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) e on e.BookingId = bs.Id 
        //left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        //left join HtGuestInfos g on g.Id= bg.GuestId
        //left join (select sum(PaidAmount) Amount,BookingId from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
        //left join (select BookingId,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId) af on af.BookingId = bs.Id
        //left join (select p.BookingId,sum(isnull(ReceiveAmount,0) - isnull(RefundAmount,0)) NetReceive
        //from (
        //select BookingId,sum(PaidAmount) ReceiveAmount,0 RefundAmount from HtBookingPayments group by BookingId
        //union all
        //select BookingId,0 ReceiveAmount,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId
        //)p group by p.BookingId
        //)p on p.BookingId = bs.Id
        //where bs.IsDeleted = 0 and (select convert(date,max(ActualCheckOutTime)) from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0) = '{queryDate}'";
        #endregion

        #region with due receive
        //string query = $@"select bs.Id BookingId, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        //,hr.ActualCheckOutTime CheckOutTime,hr.TotalGuest,hr.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,hr.ExtraBedCharge,bs.Discount,hr.NetRent
        //,ab.Amount AdvanceAmount,b.BillNumber,
        // case when datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) = 0 then 
        //case when hr.BookingDayStatus = 1 then 0.5  else 1 end 
        //else 
        //case when BookingDayStatus = 1 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 0.5 
        //when BookingDayStatus = 2 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 1 
        //else datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) end
        //end TotalStay,b.FoodBill,isnull(af.RefundAmount,0) AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax
        //,bs.ServiceCharge,e.ExtraCharge,0 PreReceive ,af.RefundTranNo
        //FROM HtBookingServices bs
        //inner join HtBookingRooms hr on hr.BookingId = bs.Id
        //inner join HtRoomInfos r on r.Id = hr.RoomId
        //inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000001' then d.NetAmount else 0 end) RoomBill
        //,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode in ('SR000001','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(d.NetAmount) ExtraCharge
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode = 'SR000003' group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) e on e.BookingId = bs.Id 
        //left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        //left join HtGuestInfos g on g.Id= bg.GuestId
        //left join (select sum(PaidAmount) Amount,BookingId from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
        //left join (select BookingId,sum(RefundAmount) RefundAmount,max(TransactionNo) RefundTranNo from HtAdvanceRefunds group by BookingId) af on af.BookingId = bs.Id
        //left join (select p.BookingId,sum(isnull(ReceiveAmount,0) - isnull(RefundAmount,0)) NetReceive
        //from (
        //select BookingId,sum(PaidAmount) ReceiveAmount,0 RefundAmount from HtBookingPayments where convert(date,PaidDate) <= '{queryDate}' group by BookingId
        //union all
        //select BookingId,0 ReceiveAmount,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId
        //)p group by p.BookingId
        //)p on p.BookingId = bs.Id
        //where bs.IsDeleted = 0 and (select convert(date,max(ActualCheckOutTime)) from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0) = '{queryDate}'
        //and bs.BookingStatus = 3 and bs.BookingType = 'R' 
        //union all
        //select bs.Id BookingId, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        //,hr.ActualCheckOutTime CheckOutTime,hr.TotalGuest,hr.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,hr.ExtraBedCharge,bs.Discount,hr.NetRent
        //,0 AdvanceAmount,b.BillNumber,
        // case when datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) = 0 then 
        //case when hr.BookingDayStatus = 1 then 0.5  else 1 end 
        //else 
        //case when BookingDayStatus = 1 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 0.5 
        //when BookingDayStatus = 2 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 1 
        //else datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) end
        //end TotalStay,b.FoodBill,0 AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,e.ExtraCharge,pre.NetReceive PreReceive,null RefundTranNo
        //FROM HtBookingServices bs
        //inner join HtBookingRooms hr on hr.BookingId = bs.Id
        //inner join HtRoomInfos r on r.Id = hr.RoomId
        //inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000001' then d.NetAmount else 0 end) RoomBill
        //,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode in ('SR000001','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(d.NetAmount) ExtraCharge
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode = 'SR000003' group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) e on e.BookingId = bs.Id 
        //left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        //left join HtGuestInfos g on g.Id= bg.GuestId
        //inner join (
        //select convert(date,PaidDate) ReceiveDate, BookingId,sum(PaidAmount) NetReceive from HtBookingPayments where convert(date,PaidDate) = '{queryDate}' group by BookingId,convert(date,PaidDate)
        //) p on p.BookingId = bs.Id
        //left join (
        //select BookingId,sum(PaidAmount) NetReceive from HtBookingPayments where convert(date,PaidDate) < '{queryDate}' group by BookingId
        //) pre on pre.BookingId = bs.Id
        //where bs.IsDeleted = 0 and bs.BookingType = 'R' and p.ReceiveDate = '{queryDate}' and (select convert(date,max(ActualCheckOutTime)) from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0) < '{queryDate}'";
        #endregion

        #region with due receive wtih complimentary
        string query = $@"select bs.Id BookingId, g.Id GuestId,isnull(g.Salutation,'')+' '+g.FirstName+' '+isnull(g.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        ,hr.ActualCheckOutTime CheckOutTime,hr.TotalGuest,hr.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,
        case 
            when isnull(e.ExtraBedServiceCharge,0) > 0 
            then e.ExtraBedServiceCharge
            else isnull(hr.ExtraBedCharge,0)
        end as ExtraBedCharge
        ,bs.Discount,ISNULL(b.BillSpecialDiscount,0) AS SpecialDiscount,hr.NetRent
        ,ab.Amount AdvanceAmount,b.BillNumber,
         case when datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) = 0 then 
        case when hr.BookingDayStatus = 1 then 0.5  else 1 end 
        else 
        case when BookingDayStatus = 1 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 0.5 
        when BookingDayStatus = 2 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 1 
        else datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) end
        end TotalStay,b.FoodBill,isnull(af.RefundAmount,0) AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax
        ,bs.ServiceCharge,e.ExtraCharge,0 PreReceive ,af.RefundTranNo,b.IsComplimentary,b.CmpRemarks,e.ConfBill,bs.BookingStatus  
        FROM HtBookingServices bs
        inner join HtBookingRooms hr on hr.BookingId = bs.Id
        inner join HtRoomInfos r on r.Id = hr.RoomId
        inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        left join (
        select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000001' then d.NetAmount else 0 end) RoomBill
        ,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill,
        MAX(ISNULL(b.SpecialDiscount,0)) AS BillSpecialDiscount,d.IsComplimentary,b.CmpRemarks
        from HtBillingDetails d
        inner join HtBillings b on b.Id = d.BillId
        inner join HtServices s on s.Id = d.ServiceId
        where b.BookingId > 0 and s.ServiceCode in ('SR000001','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId,d.IsComplimentary,b.CmpRemarks
        ) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        left join (
        select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000003' then d.NetAmount else 0 end) ExtraCharge,
        sum(case when s.ServiceCode = 'SR000009' then d.NetAmount else 0 end) ExtraBedServiceCharge,
        sum(case when s.ServiceCode in ('SR000005','SR000006','SR000007')  then d.NetAmount else 0 end) ConfBill
        from HtBillingDetails d
        inner join HtBillings b on b.Id = d.BillId
        inner join HtServices s on s.Id = d.ServiceId
        where b.BookingId > 0 and s.ServiceCode in ( 'SR000003','SR000005','SR000006','SR000007','SR000009') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        -- updated by Rifat 19/12/2025.summery was showing extra-bed charge for every room of a booking
        --) e on e.BookingId = bs.Id
        ) e on e.BookingId = bs.Id and e.BookingRoomId = hr.Id
        left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        left join HtGuestInfos g on g.Id= bg.GuestId
        left join (select sum(PaidAmount) Amount,BookingId from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
        left join (select BookingId,sum(RefundAmount) RefundAmount,max(TransactionNo) RefundTranNo from HtAdvanceRefunds group by BookingId) af on af.BookingId = bs.Id
        left join (select p.BookingId,sum(isnull(ReceiveAmount,0) - isnull(RefundAmount,0)) NetReceive
        from (
        select BookingId,sum(PaidAmount) ReceiveAmount,0 RefundAmount from HtBookingPayments where convert(date,PaidDate) <= '{queryDate}' group by BookingId
        union all
        select BookingId,0 ReceiveAmount,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId
        )p group by p.BookingId
        )p on p.BookingId = bs.Id
        where bs.IsDeleted = 0 and (
        /*select convert(date,max(ActualCheckOutTime)) from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0*/
        select case when count(ActualCheckOutTime) = count(*) then convert(date,max(ActualCheckOutTime)) else null end as max_date from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0
        ) = '{queryDate}'        
        and bs.BookingStatus = 3 and bs.BookingType = 'R' 
        union all
        select bs.Id BookingId, g.Id GuestId,isnull(g.Salutation,'')+' '+g.FirstName+' '+isnull(g.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        ,hr.ActualCheckOutTime CheckOutTime,hr.TotalGuest,hr.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,hr.ExtraBedCharge,bs.Discount,
        ISNULL(b.BillSpecialDiscount,0) AS SpecialDiscount,hr.NetRent,0 AdvanceAmount,b.BillNumber,
         case when datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) = 0 then 
        case when hr.BookingDayStatus = 1 then 0.5  else 1 end 
        else 
        case when BookingDayStatus = 1 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 0.5 
        when BookingDayStatus = 2 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 1 
        else datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) end
        end TotalStay,b.FoodBill,0 AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,e.ExtraCharge,pre.NetReceive PreReceive,null RefundTranNo,b.IsComplimentary,b.CmpRemarks,e.ConfBill ,bs.BookingStatus   
        FROM HtBookingServices bs
        inner join HtBookingRooms hr on hr.BookingId = bs.Id
        inner join HtRoomInfos r on r.Id = hr.RoomId
        inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        left join (
        select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000001' then d.NetAmount else 0 end) RoomBill
        ,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill,
        MAX(ISNULL(b.SpecialDiscount,0)) AS BillSpecialDiscount,d.IsComplimentary,b.CmpRemarks
        from HtBillingDetails d
        inner join HtBillings b on b.Id = d.BillId
        inner join HtServices s on s.Id = d.ServiceId
        where b.BookingId > 0 and s.ServiceCode in ('SR000001','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId,d.IsComplimentary,b.CmpRemarks 
        ) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        left join (
        select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000003' then d.NetAmount else 0 end) ExtraCharge
        ,sum(case when s.ServiceCode in ('SR000005','SR000006','SR000007')  then d.NetAmount else 0 end) ConfBill
        from HtBillingDetails d
        inner join HtBillings b on b.Id = d.BillId
        inner join HtServices s on s.Id = d.ServiceId
        where b.BookingId > 0 and s.ServiceCode in ( 'SR000003','SR000005','SR000006','SR000007') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        ) e on e.BookingId = bs.Id AND e.BookingRoomId = hr.Id
        left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        left join HtGuestInfos g on g.Id= bg.GuestId
        inner join (
        select convert(date,PaidDate) ReceiveDate, BookingId,sum(PaidAmount) NetReceive from HtBookingPayments where convert(date,PaidDate) = '{queryDate}' group by BookingId,convert(date,PaidDate)
        ) p on p.BookingId = bs.Id
        left join (
        select BookingId,sum(PaidAmount) NetReceive from HtBookingPayments where convert(date,PaidDate) < '{queryDate}' group by BookingId
        ) pre on pre.BookingId = bs.Id
        where bs.IsDeleted = 0 and bs.BookingType = 'R' and p.ReceiveDate = '{queryDate}' and (
        /*select convert(date,max(ActualCheckOutTime)) from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0*/
        select case when count(ActualCheckOutTime) = count(*) then convert(date,max(ActualCheckOutTime)) else null end as max_date from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0
        ) < '{queryDate}'
        /*no show*/
        union all
        select bs.Id BookingId, g.Id GuestId,isnull(g.Salutation,'')+' '+g.FirstName+' '+isnull(g.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        ,hr.ActualCheckOutTime CheckOutTime,hr.TotalGuest,hr.Rent RoomRent,(ab.Amount+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,hr.ExtraBedCharge,bs.Discount,
        ISNULL(b.BillSpecialDiscount,0) AS SpecialDiscount,hr.NetRent,ab.Amount AdvanceAmount,b.BillNumber,
         case when datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) = 0 then 
        case when hr.BookingDayStatus = 1 then 0.5  else 1 end 
        else 
        case when BookingDayStatus = 1 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 0.5 
        when BookingDayStatus = 2 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 1 
        else datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) end
        end TotalStay,b.FoodBill,isnull(af.RefundAmount,0) AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax
        ,bs.ServiceCharge,e.ExtraCharge,0 PreReceive ,'No Show '+af.RefundTranNo RefundTranNo,b.IsComplimentary,b.CmpRemarks,e.ConfBill,bs.BookingStatus   
        FROM HtBookingServices bs
        inner join HtBookingRooms hr on hr.BookingId = bs.Id
        inner join HtRoomInfos r on r.Id = hr.RoomId
        inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        left join (
        select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000001' then d.NetAmount else 0 end) RoomBill
        ,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill,
        MAX(ISNULL(b.SpecialDiscount,0)) AS BillSpecialDiscount,d.IsComplimentary,b.CmpRemarks
        from HtBillingDetails d
        inner join HtBillings b on b.Id = d.BillId
        inner join HtServices s on s.Id = d.ServiceId
        where b.BookingId > 0 and s.ServiceCode in ('SR000001','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId,d.IsComplimentary,b.CmpRemarks
        ) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        left join (
        select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000003' then d.NetAmount else 0 end) ExtraCharge
        ,sum(case when s.ServiceCode in ('SR000005','SR000006','SR000007')  then d.NetAmount else 0 end) ConfBill
        from HtBillingDetails d
        inner join HtBillings b on b.Id = d.BillId
        inner join HtServices s on s.Id = d.ServiceId
        where b.BookingId > 0 and s.ServiceCode in ( 'SR000003','SR000005','SR000006','SR000007') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        ) e on e.BookingId = bs.Id AND e.BookingRoomId = hr.Id
        left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        left join HtGuestInfos g on g.Id= bg.GuestId
        left join (select sum(PaidAmount) Amount,BookingId from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
        left join (select BookingId,sum(RefundAmount) RefundAmount,max(TransactionNo) RefundTranNo from HtAdvanceRefunds group by BookingId) af on af.BookingId = bs.Id
        left join (select p.BookingId,sum(isnull(ReceiveAmount,0) - isnull(RefundAmount,0)) NetReceive
        from (
        select BookingId,sum(PaidAmount) ReceiveAmount,0 RefundAmount from HtBookingPayments where convert(date,PaidDate) <= '{queryDate}' group by BookingId
        union all
        select BookingId,0 ReceiveAmount,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId
        )p group by p.BookingId
        )p on p.BookingId = bs.Id
        where bs.IsDeleted = 0 and (
        select case when count(CheckOutTime) = count(*) then convert(date,max(CheckOutTime)) else null end as max_date from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0
        ) = '{queryDate}'        
        and bs.BookingStatus = 4 and bs.BookingType = 'R' 
        ";
        #endregion

        #region hallBooking
        query += $@"
        union all
        select bs.Id BookingId, g.Id GuestId,isnull(g.Salutation,'')+' '+g.FirstName+' '+isnull(g.LastName,'') GuestName,null RoomCategoryId,null CategoryName,h.Id RoomId,h.HallName RoomNo,hr.BookingDate CheckInTime
        ,hr.BookingDate CheckOutTime,bs.TotalGuest,hr.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,0 ExtraBedCharge,bs.Discount,
        ISNULL(b.BillSpecialDiscount,0) AS SpecialDiscount,hr.NetRent,ab.Amount AdvanceAmount,b.BillNumber,
        0 TotalStay,b.FoodBill,isnull(af.RefundAmount,0) AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax
        ,bs.ServiceCharge,0 ExtraCharge,0 PreReceive ,af.RefundTranNo,0 IsComplimentary,null CmpRemarks,0 ConfBill,bs.BookingStatus 
        FROM HtBookingServices bs
        inner join HtBookingHalls hr on hr.BookingId = bs.Id
        inner join HtHallInfos h on h.Id = hr.HallId
        left join (
        select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000004' then d.NetAmount else 0 end) RoomBill
        ,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill,MAX(ISNULL(b.SpecialDiscount,0)) AS BillSpecialDiscount
        from HtBillingDetails d
        inner join HtBillings b on b.Id = d.BillId
        inner join HtServices s on s.Id = d.ServiceId
        where b.BookingId > 0 and s.ServiceCode in ('SR000004','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        ) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        left join HtGuestInfos g on g.Id= bg.GuestId
        left join (select sum(PaidAmount) Amount,BookingId from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
        left join (select BookingId,sum(RefundAmount) RefundAmount,max(TransactionNo) RefundTranNo from HtAdvanceRefunds group by BookingId) af on af.BookingId = bs.Id
        left join (select p.BookingId,sum(isnull(ReceiveAmount,0) - isnull(RefundAmount,0)) NetReceive
        from (
        select BookingId,sum(PaidAmount) ReceiveAmount,0 RefundAmount from HtBookingPayments where convert(date,PaidDate) <= '{queryDate}' group by BookingId
        union all
        select BookingId,0 ReceiveAmount,sum(RefundAmount) RefundAmount from HtAdvanceRefunds group by BookingId
        )p group by p.BookingId
        )p on p.BookingId = bs.Id
        where bs.IsDeleted = 0 and bs.BookingType = 'H' and convert(date,bs.CheckOutTime) = '{queryDate}' and bs.BookingStatus = 3
        union all
        select bs.Id BookingId, g.Id GuestId,isnull(g.Salutation,'')+' '+g.FirstName+' '+isnull(g.LastName,'') GuestName,null RoomCategoryId,null CategoryName,h.Id RoomId,h.HallName RoomNo,hr.BookingDate CheckInTime
        ,hr.BookingDate CheckOutTime,bs.TotalGuest,hr.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,0 ExtraBedCharge,bs.Discount,
        ISNULL(b.BillSpecialDiscount,0) AS SpecialDiscount,hr.NetRent,0 AdvanceAmount,b.BillNumber,0 TotalStay,b.FoodBill,0 AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge
        ,0 ExtraCharge,pre.NetReceive PreReceive,null RefundTranNo,0 IsComplimentary,null CmpRemarks ,0 ConfBill,bs.BookingStatus  
        FROM HtBookingServices bs
        inner join HtBookingHalls hr on hr.BookingId = bs.Id
        inner join HtHallInfos h on h.Id = hr.HallId
        left join (
        select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000004' then d.NetAmount else 0 end) RoomBill
        ,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill,MAX(ISNULL(b.SpecialDiscount,0)) AS BillSpecialDiscount
        from HtBillingDetails d
        inner join HtBillings b on b.Id = d.BillId
        inner join HtServices s on s.Id = d.ServiceId
        where b.BookingId > 0 and s.ServiceCode in ('SR000004','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        ) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        left join HtGuestInfos g on g.Id= bg.GuestId
        inner join (
        select convert(date,PaidDate) ReceiveDate, BookingId,sum(PaidAmount) NetReceive from HtBookingPayments where convert(date,PaidDate) = '{queryDate}' group by BookingId,convert(date,PaidDate)
        ) p on p.BookingId = bs.Id
        left join (
        select BookingId,sum(PaidAmount) NetReceive from HtBookingPayments where convert(date,PaidDate) < '{queryDate}' group by BookingId
        ) pre on pre.BookingId = bs.Id
        where bs.IsDeleted = 0 and bs.BookingType = 'H' and p.ReceiveDate = '{queryDate}' and convert(date,bs.CheckOutTime) < '{queryDate}' ";

        #endregion

        var data = await _iReadDbConnection.QueryAsync<BookingDailySalesReportVm>(query);
        return data.ToList();
    }

    public async Task<List<BookingDailySalesReportVm>> GetTotalAdvanceRecieveByToday(string strQueryDate)
    {
        var queryDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(strQueryDate)).ToString("dd/MMM/yyyy");
        string query = $"select sum(PaidAmount) AdvanceAmount from HtBookingPayments where BillingId is null and convert(date,PaidDate) = '{queryDate}'";

        var data = await _iReadDbConnection.QueryAsync<BookingDailySalesReportVm>(query);
        return data.ToList();
    }

    #endregion

    #region GetBookingReportHtml

    //public async Task<string> GetBookingDailySalesReportHtml(BookingDailySalesReportVm vm, bool isPrint = false)
    //{
    //    string fullHtml = "";
    //    var data = await GetBookingDailySalesData(vm);
    //    if (data != null && data.Count > 0)
    //    {
    //        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
    //        fullHtml += "<thead>";

    //        fullHtml += $@"<tr style='height:30px;'><td colspan='19' class='text-center'>Date: {vm.StrQueryDate}</td></tr>";

    //        fullHtml += "<tr style='height:30px;' class='text-center'>";

    //        fullHtml += $@"<th style='width:50px;' rowspan='2'>SL No</th>
    //                        <th style='width:150px;' rowspan='2'>Customer Name</th>
    //                        <th style='width:50px;' rowspan='2'>Room No</th>
    //                        <th style='width:100px;' rowspan='2'>Check In</th>
    //                        <th style='width:100px;' rowspan='2'>Check Out</th>
    //                        <th style='width:50px;' rowspan='2'>Total Stay</th>
    //                        <th style='width:80px;' rowspan='2'>Room Charge</th>
    //                        <th style='width:70px;' rowspan='2'>Bill No</th>
    //                        <th colspan='5'>Income</th>
    //                        <th colspan='3'>Expenditure</th>
    //                        <th rowspan='2' style='width:80px;font-size:10px;'>Net Receive</th>
    //                        <th colspan='2'>Due & Sales</th>";

    //        fullHtml += "</tr>";

    //        fullHtml += "<tr style='height:30px;font-size:9px;' class='text-center'>";

    //        fullHtml += $@"<th style='width:50px;'>Total Room Rent</th>
    //                        <th style='width:50px;'>Room Adv</th>
    //                        <th style='width:50px;'>Food</th>
    //                        <th style='width:50px;'>Extra Bed</th>
    //                        <th style='width:50px;'>Total</th>
    //                        <th style='width:50px;'>Discount</th>
    //                        <th style='width:50px;'>Adv Refund</th>
    //                        <th style='width:50px;'>Total</th>
    //                        <th style='width:50px;'>Due & Sales</th>
    //                        <th style='width:50px;'>Total Cash</th>";

    //        fullHtml += "</tr>";

    //        fullHtml += "</thead>";
    //        fullHtml += "<tbody>";

    //        for (int i = 0; i < data.Count; i++)
    //        {
    //            BookingDailySalesReportVm objBooking = data[i];

    //            var bookingCount = data.Where(x => x.BookingId == objBooking.BookingId).Count();
    //            //var guestTotalRoomCount = data.Where(x => x.GuestId == objBooking.GuestId).Count();
    //            //var billCount = data.Where(x => x.BillNumber == objBooking.BillNumber).Count();
    //            var bookingRoomRentTotal = data.Where(x => x.BookingId == objBooking.BookingId).Sum(x => x.Rent);
    //            var bookingRoomNetRentTotal = data.Where(x => x.BookingId == objBooking.BookingId).Sum(x => x.NetRent + x.FoodBill);
    //            //var bookingDiscountTotal = data.Where(x => x.BookingId == objBooking.BookingId).Sum(x => x.Discount);
    //            var totalExpenditure = objBooking.Discount + objBooking.AdvanceRefund;
    //            var due = bookingRoomNetRentTotal - objBooking.Discount - objBooking.NetReceive;

    //            fullHtml += "<tr>";

    //            fullHtml += $@"<td style='text-align:left;'>{i + 1}</td>";

    //            if (i == 0 || objBooking.BookingId != data[i - 1].BookingId)
    //            {
    //                fullHtml += $@"<td rowspan='{bookingCount}' style='text-align:left;'><b>{objBooking.GuestName}</b></td>";
    //            }

    //            fullHtml += $@"<td style='text-align:center;'><b>{objBooking.RoomNo}</b></td>                              
    //                            <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckInTime)}</td>
    //                            <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckOutTime)}</td>
    //                            <td style='text-align:center;'>{objBooking.TotalStay}</td>
    //                            <td style='text-align:center;'>{objBooking.RoomRent}</td>";

    //            if (i == 0 || objBooking.BookingId != data[i - 1].BookingId)
    //            {
    //                fullHtml += $@"<td rowspan='{bookingCount}' style='text-align:left;'>{objBooking.BillNumber} </td>";
    //            }

    //            if (i == 0 || objBooking.BookingId != data[i - 1].BookingId)
    //            {
    //                fullHtml += $@"<td rowspan='{bookingCount}' style='text-align:center;'>{bookingRoomRentTotal} </td>
    //                                <td rowspan='{bookingCount}' style='text-align:center;'>{objBooking.AdvanceAmount} </td>";
    //            }

    //            fullHtml += $@"<td style='text-align:center;'>{objBooking.FoodBill:N2}</td>
    //                            <td style='text-align:center;'>{objBooking.ExtraBedCharge}</td>";

    //            //if (i == 0 || objBooking.BookingId != data[i - 1].BookingId)
    //            //{
    //            //    fullHtml += $@"<td style='text-align:center;'>{bookingRoomNetRentTotal}</td>
    //            //                    <td rowspan='{bookingCount}' style='text-align:center;'>{bookingDiscountTotal} </td>";
    //            //}
    //            if (i == 0 || objBooking.BookingId != data[i - 1].BookingId)
    //            {
    //                fullHtml += $@"<td rowspan='{bookingCount}' style='text-align:center;'>{bookingRoomNetRentTotal}</td>
    //                                <td rowspan='{bookingCount}' style='text-align:center;'>{objBooking.Discount} </td>
    //                                <td rowspan='{bookingCount}' style='text-align:center;'>{objBooking.AdvanceRefund} </td>
    //                                <td rowspan='{bookingCount}' style='text-align:center;'>{totalExpenditure} </td>
    //                                <td rowspan='{bookingCount}' style='text-align:center;'>{objBooking.NetReceive} </td>
    //                                <td rowspan='{bookingCount}' style='text-align:center;'>{due} </td>
    //                                <td rowspan='{bookingCount}' style='text-align:center;'>{objBooking.NetReceive} </td>";

    //            }
    //            fullHtml += "</tr>";

    //        }
    //    }
    //    fullHtml += "</tbody>";


    //    try
    //    {
    //        var groupData = data.Select(o => new { o.BookingId, o.CategoryName, o.Vat, o.Tax, o.Discount, o.NetReceive, o.AdvanceAmount, o.AdvanceRefund }).Distinct().ToList();
    //        fullHtml += "<tfoot>";
    //        fullHtml += $@"<tr><td colspan='8' style='text-align:right;'><b>TOTAL</b></td>                              
    //                        <td style='text-align:center;'>{data.Sum(o => o.NetRent):F2}</td>
    //                        <td style='text-align:center;'>{groupData.Sum(o=>o.AdvanceAmount)}</td>
    //                        <td style='text-align:center;'>{data.Sum(o => o.FoodBill):F2}</td>
    //                        <td style='text-align:center;'>{data.Sum(o => o.ExtraBedCharge):F2}</td>
    //                        <td style='text-align:center;'></td>
    //                        <td style='text-align:center;'>{groupData.Sum(o => o.Discount):F2}</td>
    //                        <td style='text-align:center;'>{groupData.Sum(o => o.AdvanceRefund):F2}</td>
    //                         <td style='text-align:center;'></td>
    //                        <td style='text-align:center;'>{groupData.Sum(o => o.NetReceive):F2}</td>
    //                        <td style='text-align:center;'></td>
    //                        <td style='text-align:center;'>{groupData.Sum(o => o.NetReceive):F2}</td>";
    //        fullHtml += "</tr>";
    //        fullHtml += "</tfoot>";
    //    }
    //    catch (Exception ex)
    //    {
    //    }



    //    fullHtml += "</table>";

    //    return fullHtml;
    //}


    public async Task<string> GetBookingDailySalesReportHtml(BookingDailySalesReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await GetBookingDailySalesData(vm);
        //var advanceData = await GetTotalAdvanceRecieveByToday(vm.StrQueryDate);

        //changed by tawkir..
        var advanceData = await ConvertAdvanceToDailySales(vm.StrQueryDate);
        if (advanceData.Count > 0)
            data.AddRange(advanceData);

        data.ForEach(x =>
        {
            if (!x.IsAdvance)
                x.AdvanceAmount = 0;
        });
        decimal grandDue = 0;
        decimal grandComplementary = 0;
        decimal grandDuePreCollection = 0;


        //var groupData = data.Select(o => new { o.BookingId, o.GuestName, o.Vat, o.Tax, o.Discount, o.SpecialDiscount, o.NetReceive, o.AdvanceAmount, o.AdvanceRefund, o.BillNumber, o.Due, o.ReservationAdvance, o.ExtraCharge, o.PreReceive, o.RefundTranNo, o.ComplimentaryAmount, o.ConfBill, o.BookingStatus }).Distinct().ToList();

        //var groupData = data.Select(o => new { o.BookingId, o.GuestName, o.Vat, o.Tax, o.Discount, o.SpecialDiscount, o.NetReceive, o.AdvanceAmount, o.AdvanceRefund, o.BillNumber, o.Due, o.ReservationAdvance, o.PreReceive, o.RefundTranNo, o.ComplimentaryAmount, o.ConfBill, o.BookingStatus }).Distinct().ToList();

        var groupData = data.Select(o => new { o.BookingId, o.GuestName, o.Vat, o.Tax, o.Discount, o.SpecialDiscount, o.NetReceive, o.AdvanceAmount, o.AdvanceRefund, o.BillNumber, o.Due, o.ReservationAdvance, o.PreReceive, o.RefundTranNo, o.ComplimentaryAmount, o.BookingStatus }).Distinct().ToList();

        // === Header Section ===
        if (isPrint)
        {
            fullHtml += $@" <div style='text-align:center;margin-bottom:20px;'>
        
                                <p style='margin:4px 0;color:#555;font-size:14px;'>
                                    <b>Date: </b> {vm.StrQueryDate}
                                </p>
                                <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                            </div>";
        }

        fullHtml += "<table class='table  table-bordered report-table landscape-table' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;font-size:xx-small' border='1'>";
        fullHtml += "<thead>";

        fullHtml += $@"<tr style='height:30px;'><td colspan='26' class='text-center'>Date: {vm.StrQueryDate}</td></tr>";

        fullHtml += "<tr style='height:30px;' class='text-center'>";
        fullHtml += $@"<th style='width:2.83%;font-size:10px;' rowspan='2'>SL</th>
                        <th style='width:8.87%;font-size:10px;' rowspan='2'>Guest Name</th>
                        <th style='width:3.53%;font-size:10px;' rowspan='2'>Room No</th>
                        <th style='width:5.64%;font-size:10px;' rowspan='2'>Check In</th>
                        <th style='width:5.64%;font-size:10px;' rowspan='2'>Check Out</th>
                        <th style='width:3.24%;font-size:10px;' rowspan='2'>Total Stay</th>
                        <th style='width:4.94%;font-size:10px;' rowspan='2'>Room Charge</th>
                        <th style='width:4.94%;font-size:10px;' rowspan='2'>Bill No</th>
                        <th style='width:2.82%;font-size:10px;' rowspan='2'>VAT</th>
                        <th style='width:2.82%;font-size:10px;' rowspan='2'>TAX</th>
                        <th colspan='8' style='font-size:11px;'>Income</th>
                        <th colspan='4' style='font-size:11px;'>Expenditure</th>
                        <th rowspan='2' style='width:4.23%;font-size:10px;'>Net Receive</th>
                        <th colspan='2' style='font-size:11px;'>Due & Sales</th>
                        <th style='width:3.95%;font-size:9px;' rowspan='2'>Remarks</th>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height:30px;font-size:9px;' class='text-center'>";

        fullHtml += $@"<th style='width:4.23%;'>Total Room Rent</th>
                       <th style='width:3.53%;'>Room Adv</th>
                       <th style='width:3.53%;'>Res Adv</th>
                       <th style='width:3.53%;'>Food</th>
                       <th style='width:3.53%;'>Conf.</th>
                       <th style='width:3.53%;'>Extra Bed</th>
                       <th style='width:3.53%;'>Extra Charge</th>
                       <th style='width:3.53%;'>Total</th>
                       <th style='width:4.09%;'>Discount</th>
                       <th style='width:3.95%;'>Adv Refund</th>
                       <th style='width:3.95%;'>Compl.</th>
                       <th style='width:4.23%;'>Total</th>
                       <th style='width:4.23%;'>Due & Sales</th>
                       <th style='width:4.23%;'>Total Cash</th>";

        fullHtml += "</tr>";

        fullHtml += "</thead>";
        fullHtml += "<tbody>";
        if (data != null && data.Count > 0)
        {
            for (int j = 0; j < groupData.Count; j++)
            {
                var objGroup = groupData[j];
                var roomList = data.Where(o => o.BookingId == objGroup.BookingId && o.BillNumber == objGroup.BillNumber).ToList();
                int roomCount = roomList.Count;

                //tawkir added: 04/01/2026 for due collection with advance
                bool isPostCheckoutCollection = objGroup.PreReceive > 0 ? true : false;

                decimal income = (roomList.Sum(x => x.Rent + x.FoodBill + x.ExtraBedCharge + x.ExtraCharge + x.ConfBill) + objGroup.Vat + objGroup.Tax);
                if (objGroup.BookingStatus == 4)//no show
                    income = objGroup.AdvanceRefund + objGroup.Vat + objGroup.Tax;

                decimal totalDiscount = objGroup.Discount + objGroup.SpecialDiscount;

                decimal expenditure = (totalDiscount + objGroup.AdvanceRefund);
                decimal due = (income - expenditure) - objGroup.NetReceive - objGroup.PreReceive;
                decimal complementaryAmount = 0;
                if (roomList.Where(r => r.IsComplimentary > 0).ToList().Count > 0)
                {
                    complementaryAmount = due;
                    due = 0;
                    grandComplementary += complementaryAmount;
                }
                due = (due < 0) ? 0 : due;
                grandDue += due;

                //tawkir added: 04/01/2026 for due collection with advance
                if (isPostCheckoutCollection)
                {
                    grandDuePreCollection += objGroup.PreReceive;
                }

                for (int i = 0; i < roomList.Count; i++)
                {
                    BookingDailySalesReportVm objRoom = roomList[i];
                    objRoom.CmpRemarks = (string.IsNullOrEmpty(objGroup.RefundTranNo)) ? objRoom.CmpRemarks : objGroup.RefundTranNo;
                    fullHtml += "<tr>";
                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:left;'>{j + 1}</td>" : "";
                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:left;'>{objGroup.GuestName}</td>" : "";
                    fullHtml += $@"<td style='text-align:left;'>{objRoom.RoomNo}</td>";
                    fullHtml += $@"<td style='text-align:left;'>{DU.Utility.ConvertDateToStr(objRoom.CheckInTime)}</td>";
                    fullHtml += $@"<td style='text-align:left;'>{DU.Utility.ConvertDateToStr(objRoom.CheckOutTime)}</td>";
                    fullHtml += $@"<td style='text-align:center;'>{objRoom.TotalStay}</td>";
                    fullHtml += $@"<td style='text-align:right;'>{objRoom.RoomRent}</td>";
                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:left;'>{objGroup.BillNumber}</td>" : "";
                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{objGroup.Vat}</td>" : "";
                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{objGroup.Tax}</td>" : "";

                    if (objGroup.BookingStatus == 4) //no show
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{objGroup.AdvanceRefund}</td>" : "";
                    else
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{roomList.Where(x => x.RoomCategoryId > 0).Sum(x => x.Rent)}</td>" : "";

                    if (objRoom.IsAdvance)
                    {
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{objGroup.AdvanceAmount}</td>" : "";
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{objGroup.ReservationAdvance}</td>" : "";
                    }
                    else
                    {
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'></td>" : "";
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'></td>" : "";
                    }
                    fullHtml += $@"<td style='text-align:right;'>{objRoom.FoodBill}</td>";
                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{roomList.Where(x => x.RoomCategoryId == 0).Sum(x => x.Rent) + roomList.Sum(x => x.ConfBill)}</td>" : "";
                    fullHtml += $@"<td style='text-align:right;'>{objRoom.ExtraBedCharge}</td>";
                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{roomList.Sum(x=>x.ExtraCharge)}</td>" : "";

                    if (objGroup.BookingStatus == 4) // no show
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{objGroup.AdvanceRefund + objGroup.AdvanceAmount + objGroup.ReservationAdvance}</td>" : "";
                    else
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{roomList.Sum(x => x.Rent + x.FoodBill + x.ExtraBedCharge + x.ExtraCharge + x.ConfBill) + objGroup.AdvanceAmount + objGroup.ReservationAdvance}</td>" : "";

                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{totalDiscount}</td>" : "";

                    //tawkir added: 04/01/2026 for due collection with advance
                    if (isPostCheckoutCollection)
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{objGroup.PreReceive}</td>" : "";
                    else
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{objGroup.AdvanceRefund}</td>" : "";


                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{complementaryAmount}</td>" : "";

                    //tawkir added: 04/01/2026 for due collection with advance
                    if (isPostCheckoutCollection)
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{(totalDiscount + objGroup.AdvanceRefund + complementaryAmount + objGroup.PreReceive)}</td>" : "";
                    else
                        fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{(totalDiscount + objGroup.AdvanceRefund + complementaryAmount)}</td>" : "";
                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{objGroup.NetReceive}</td>" : "";
                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{due}</td>" : "";
                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:right;'>{objGroup.NetReceive}</td>" : "";
                    //fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:left;'>{objGroup.RefundTranNo}</td>" : "";
                    fullHtml += (i == 0) ? $@"<td rowspan='{roomCount}' style='text-align:left;'>{objRoom.CmpRemarks}</td>" : "";
                    fullHtml += "</tr>";
                }
            }
        }
        fullHtml += "</tbody>";

        try
        {

            fullHtml += "<tfoot>";
            fullHtml += $@"<tr style='font-size:10px;'><td colspan='8' style='text-align:right;'><b>TOTAL</b></td> 
                            <td style='text-align:right;'><b>{groupData.Sum(o => o.Vat):F2}</b></td>
                            <td style='text-align:right;'><b>{groupData.Sum(o => o.Tax):F2}</b></td>
                            <td style='text-align:right;'><b>{data.Where(x => x.RoomCategoryId > 0).Sum(o => o.Rent):F2}</b></td>
                            <td style='text-align:right;'><b>{groupData.Sum(o => o.AdvanceAmount)}</b></td>
                            <td style='text-align:right;'><b>{groupData.Sum(o => o.ReservationAdvance)}</b></td>
                            <td style='text-align:right;'><b>{data.Sum(o => o.FoodBill):F2}</b></td>
                            <td style='text-align:right;'><b>{(data.Where(x => x.RoomCategoryId == 0).Sum(o => o.Rent) + data.Sum(o => o.ConfBill)):F2}</b></td>
                            <td style='text-align:right;'><b>{data.Sum(o => o.ExtraBedCharge):F2}</b></td>
                            <td style='text-align:right;'><b>{data.Sum(o => o.ExtraCharge):F2}</b></td>
                            <td style='text-align:right;'><b>{data.Sum(x => x.Rent + x.FoodBill + x.ExtraBedCharge + x.ExtraCharge + x.ConfBill) + groupData.Sum(o => o.AdvanceAmount + o.ReservationAdvance)}</b></td>
                            <td style='text-align:right;'><b>{groupData.Sum(o => o.Discount + o.SpecialDiscount):F2}</b></td>
                            <td style='text-align:right;'><b>{(groupData.Sum(o => o.AdvanceRefund) + grandDuePreCollection):F2}</b></td>
                            <td style='text-align:right;'><b>{grandComplementary:F2}</b></td>
                            <td style='text-align:right;'><b>{(groupData.Sum(o => o.Discount + o.SpecialDiscount + o.AdvanceRefund) + grandComplementary + grandDuePreCollection):F2}</b></td>
                            <td style='text-align:right;'><b>{groupData.Sum(o => o.NetReceive):F2}</b></td>
                            <td style='text-align:right;'><b>{grandDue:F2}</b></td>
                            <td style='text-align:right;'><b>{groupData.Sum(o => o.NetReceive):F2}</b></td>
                            <td style='text-align:right;'></td>";
            fullHtml += "</tr>";
            fullHtml += $@"<tr><td colspan='3' style='text-align:right;'><b>Cash Received</b></td> 
                            <td colspan='23' style='text-align:left;font-weight:bold;font-size:12px;'>{groupData.Sum(o => o.NetReceive):F2}</td>";
            fullHtml += "</tr>";
            fullHtml += $@"<tr><td colspan='3' style='text-align:right;'><b>In Word</b></td> 
                            <td colspan='23' style='text-align:left;font-weight:bold;font-size:12px;'>{Utility.ConvertToWordInt(Convert.ToDouble(groupData.Sum(o => o.NetReceive)))}</td>";
            fullHtml += "</tr>";
            fullHtml += "</tfoot>";
            fullHtml += "</table>";

        }
        catch (Exception ex)
        {
        }


        // === Footer Section ===
        fullHtml += @"<div class='report-footer'>
    		            <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
    		            <p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";
        fullHtml += @"</div>";
        return fullHtml;


        #region
        //if (data != null && data.Count > 0)
        //{



        //    for (int i = 0; i < data.Count; i++)
        //    {
        //        BookingDailySalesReportVm objBooking = data[i];

        //        var bookingCount = data.Where(x => x.BookingId == objBooking.BookingId).Count();
        //        //var guestTotalRoomCount = data.Where(x => x.GuestId == objBooking.GuestId).Count();
        //        //var billCount = data.Where(x => x.BillNumber == objBooking.BillNumber).Count();
        //        var bookingRoomRentTotal = data.Where(x => x.BookingId == objBooking.BookingId).Sum(x => x.Rent);
        //        var bookingRoomNetRentTotal = data.Where(x => x.BookingId == objBooking.BookingId).Sum(x => x.NetRent + x.FoodBill);
        //        //var bookingDiscountTotal = data.Where(x => x.BookingId == objBooking.BookingId).Sum(x => x.Discount);
        //        var totalExpenditure = objBooking.Discount + objBooking.AdvanceRefund;
        //        var due = bookingRoomNetRentTotal - objBooking.Discount - objBooking.NetReceive;

        //        fullHtml += "<tr>";

        //        fullHtml += $@"<td style='text-align:left;'>{i + 1}</td>";

        //        if (i == 0 || objBooking.BookingId != data[i - 1].BookingId)
        //        {
        //            fullHtml += $@"<td rowspan='{bookingCount}' style='text-align:left;'><b>{objBooking.GuestName}</b></td>";
        //        }

        //        fullHtml += $@"<td style='text-align:center;'><b>{objBooking.RoomNo}</b></td>                              
        //                        <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckInTime)}</td>
        //                        <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckOutTime)}</td>
        //                        <td style='text-align:center;'>{objBooking.TotalStay}</td>
        //                        <td style='text-align:center;'>{objBooking.RoomRent}</td>";

        //        if (i == 0 || objBooking.BookingId != data[i - 1].BookingId)
        //        {
        //            fullHtml += $@"<td rowspan='{bookingCount}' style='text-align:left;'>{objBooking.BillNumber} </td>";
        //        }

        //        if (i == 0 || objBooking.BookingId != data[i - 1].BookingId)
        //        {
        //            fullHtml += $@"<td rowspan='{bookingCount}' style='text-align:center;'>{bookingRoomRentTotal} </td>
        //                            <td rowspan='{bookingCount}' style='text-align:center;'>{objBooking.AdvanceAmount} </td>";
        //        }

        //        fullHtml += $@"<td style='text-align:center;'>{objBooking.FoodBill:N2}</td>
        //                        <td style='text-align:center;'>{objBooking.ExtraBedCharge}</td>";

        //        //if (i == 0 || objBooking.BookingId != data[i - 1].BookingId)
        //        //{
        //        //    fullHtml += $@"<td style='text-align:center;'>{bookingRoomNetRentTotal}</td>
        //        //                    <td rowspan='{bookingCount}' style='text-align:center;'>{bookingDiscountTotal} </td>";
        //        //}
        //        if (i == 0 || objBooking.BookingId != data[i - 1].BookingId)
        //        {
        //            fullHtml += $@"<td rowspan='{bookingCount}' style='text-align:center;'>{bookingRoomNetRentTotal}</td>
        //                            <td rowspan='{bookingCount}' style='text-align:center;'>{objBooking.Discount} </td>
        //                            <td rowspan='{bookingCount}' style='text-align:center;'>{objBooking.AdvanceRefund} </td>
        //                            <td rowspan='{bookingCount}' style='text-align:center;'>{totalExpenditure} </td>
        //                            <td rowspan='{bookingCount}' style='text-align:center;'>{objBooking.NetReceive} </td>
        //                            <td rowspan='{bookingCount}' style='text-align:center;'>{due} </td>
        //                            <td rowspan='{bookingCount}' style='text-align:center;'>{objBooking.NetReceive} </td>";

        //        }
        //        fullHtml += "</tr>";

        //    }
        //}
        //fullHtml += "</tbody>";

        #endregion





    }

    #endregion

    #region DateWiseAdvancePayment

    public async Task<List<BookingDailyAdvanceReportVm>> GetBookingAdvancePaymentByDate(string strQueryDate)
    {
        var queryDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(strQueryDate)).ToString("dd/MMM/yyyy");

        //string query = $@"select bs.Id BookingId,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,'') GuestName,null RoomCategoryId,null CategoryName,null RoomId,bs.BookingNo RoomNo
        //,bs.CheckInTime,bs.CheckOutTime CheckOutTime,hr.TotalGuest,0 RoomRent,0 Rent,0 ExtraBedCharge,0 Discount,0 NetRent
        //,0 AdvanceAmount,bp.TransactionNo BillNumber,null TotalStay,null FoodBill,0 AdvanceRefund,0 NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,max(bp.PaidAmount) PaidAmount
        //from HtBookingPayments bp
        //inner join HtBookingServices bs on bs.Id = bp.BookingId
        //inner join HtBookingRooms hr on hr.BookingId = BS.Id
        //inner join HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
        //inner join HtGuestInfos hg on hg.Id = bg.GuestId
        //where BillingId is null and convert(date,bp.PaidDate) = '{queryDate}' 
        //and (hr.ActualCheckInTime is null or convert(date,hr.ActualCheckInTime) > '{queryDate}' ) 
        //and (hr.ActualCheckOutTime is null or convert(date,hr.ActualCheckOutTime) > '{queryDate}' ) 
        //group by bs.Id,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,''),bp.TransactionNo,bs.CheckInTime,bs.BookingNo 
        //,bs.CheckOutTime,hr.TotalGuest,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax),hr.ExtraBedCharge,bs.Discount,hr.NetRent,bs.Vat,bs.Tax,bs.ServiceCharge
        //union all
        //select bs.Id BookingId,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        //,hr.CheckOutTime CheckOutTime,hr.TotalGuest,0 RoomRent,0 Rent,0 ExtraBedCharge,0 Discount,0 NetRent
        //,sum(bp.PaidAmount) AdvanceAmount,bp.TransactionNo BillNumber,null TotalStay,null FoodBill,0 AdvanceRefund,0 NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,0 PaidAmount
        //from HtBookingPayments bp
        //inner join HtBookingServices bs on bs.Id = bp.BookingId
        //inner join HtBookingRooms hr on hr.BookingId = BS.Id
        //inner join HtRoomInfos r on r.Id = hr.RoomId
        //inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        //inner join HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
        //inner join HtGuestInfos hg on hg.Id = bg.GuestId
        //where BillingId is null and convert(date,bp.PaidDate) = '{queryDate}' 
        //and hr.ActualCheckInTime is not null and convert(date,hr.ActualCheckInTime) <= '{queryDate}'
        //and (hr.ActualCheckOutTime is null or convert(date,hr.ActualCheckOutTime) > '{queryDate}') 
        //group by bs.Id,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,''),rc.Id,rc.CategoryName,r.Id,r.RoomNo,hr.CheckInTime
        //,hr.CheckOutTime,hr.TotalGuest,r.Rent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax),hr.ExtraBedCharge,bs.Discount,hr.NetRent,bs.Vat,bs.Tax,bs.ServiceCharge,bp.TransactionNo";


        string query = $@"select bs.Id BookingId,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,'') GuestName,null RoomCategoryId,null CategoryName,null RoomId,bs.BookingNo RoomNo
        ,bs.CheckInTime,bs.CheckOutTime CheckOutTime,null TotalGuest,0 RoomRent,0 Rent,0 ExtraBedCharge,0 Discount,0 NetRent
        ,0 AdvanceAmount,bp.TransactionNo BillNumber,null TotalStay,null FoodBill,0 AdvanceRefund,0 NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,max(bp.PaidAmount) PaidAmount
        from HtBookingPayments bp
        inner join HtBookingServices bs on bs.Id = bp.BookingId
        inner join HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
        inner join HtGuestInfos hg on hg.Id = bg.GuestId
        where BillingId is null and convert(date,bp.PaidDate) = '{queryDate}' and bs.BookingType = 'R'
        and (
         (select min(convert(date,ActualCheckInTime)) from HtBookingRooms where BookingId = bs.Id) is null
          or 
         (select min(convert(date,ActualCheckInTime)) from HtBookingRooms where BookingId = bs.Id) > '{queryDate}'
         )
        group by bs.Id,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,''),bp.TransactionNo,bs.CheckInTime,bs.BookingNo 
        ,bs.CheckOutTime,bs.Discount,bs.Vat,bs.Tax,bs.ServiceCharge
        union all
        select bs.Id BookingId,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        ,hr.CheckOutTime CheckOutTime,hr.TotalGuest,0 RoomRent,0 Rent,0 ExtraBedCharge,0 Discount,0 NetRent
        ,sum(bp.PaidAmount) AdvanceAmount,bp.TransactionNo BillNumber,null TotalStay,null FoodBill,0 AdvanceRefund,0 NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,0 PaidAmount
        from HtBookingPayments bp
        inner join HtBookingServices bs on bs.Id = bp.BookingId
        inner join HtBookingRooms hr on hr.BookingId = BS.Id
        inner join HtRoomInfos r on r.Id = hr.RoomId
        inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        inner join HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
        inner join HtGuestInfos hg on hg.Id = bg.GuestId
        where BillingId is null and convert(date,bp.PaidDate) = '{queryDate}' and bs.BookingType = 'R' 
        and (select min(convert(date,ActualCheckInTime)) from HtBookingRooms where BookingId = bs.Id) is not null
        and (select min(convert(date,ActualCheckInTime)) from HtBookingRooms where BookingId = bs.Id) <= '{queryDate}'
        group by bs.Id,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,''),rc.Id,rc.CategoryName,r.Id,r.RoomNo,hr.CheckInTime
        ,hr.CheckOutTime,hr.TotalGuest,r.Rent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax),hr.ExtraBedCharge,bs.Discount,hr.NetRent,bs.Vat,bs.Tax,bs.ServiceCharge,bp.TransactionNo";


        /*HALL BOOKING ADVANCE*/
        query += $@" /*before checkin advance*/
        union all
        select bs.Id BookingId,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,'') GuestName,null RoomCategoryId,null CategoryName,null RoomId,bs.BookingNo RoomNo
        ,bs.CheckInTime,bs.CheckOutTime CheckOutTime,null TotalGuest,0 RoomRent,0 Rent,0 ExtraBedCharge,0 Discount,0 NetRent
        ,0 AdvanceAmount,bp.TransactionNo BillNumber,null TotalStay,null FoodBill,0 AdvanceRefund,0 NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,max(bp.PaidAmount) PaidAmount
        from HtBookingPayments bp
        inner join HtBookingServices bs on bs.Id = bp.BookingId
        inner join HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
        inner join HtGuestInfos hg on hg.Id = bg.GuestId
        where BillingId is null and convert(date,bp.PaidDate) = '{queryDate}' and bs.BookingType = 'H' and convert(Date, bs.CheckInTime) > '{queryDate}' 
        group by bs.Id,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,''),bp.TransactionNo,bs.CheckInTime,bs.BookingNo ,bs.CheckOutTime,bs.Discount,bs.Vat,bs.Tax,bs.ServiceCharge
        /*after checkin advance*/
        union all
        select bs.Id BookingId,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,'') GuestName,null RoomCategoryId,null CategoryName,h.Id RoomId,h.HallName RoomNo,hr.BookingDate CheckInTime
        ,hr.BookingDate CheckOutTime,bs.TotalGuest,0 RoomRent,0 Rent,0 ExtraBedCharge,0 Discount,0 NetRent
        ,max(bp.PaidAmount) AdvanceAmount,bp.TransactionNo BillNumber,null TotalStay,null FoodBill,0 AdvanceRefund,0 NetReceive,bs.Vat,bs.Tax,bs.ServiceCharge,0 PaidAmount
        from HtBookingPayments bp
        inner join HtBookingServices bs on bs.Id = bp.BookingId
        inner join HtBookingHalls hr on hr.BookingId = BS.Id
        inner join HtHallInfos h on h.id = hr.HallId
        inner join HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
        inner join HtGuestInfos hg on hg.Id = bg.GuestId
        where BillingId is null and convert(date,bp.PaidDate) = '{queryDate}' and bs.BookingType = 'H' and convert(Date, bs.CheckInTime) <= '{queryDate}'  
        group by bs.Id,bg.GuestId, HG.FirstName+' '+isnull(HG.LastName,''),h.Id,h.HallName,bs.CheckInTime,bp.id,hr.BookingDate
        ,bs.CheckOutTime,bs.TotalGuest,h.Rent,bs.Vat,bs.Tax,bs.ServiceCharge,bp.TransactionNo";


        var data = await _iReadDbConnection.QueryAsync<BookingDailyAdvanceReportVm>(query);
        return data.ToList();
    }

    private async Task<List<BookingDailySalesReportVm>> ConvertAdvanceToDailySales(string strQueryDate)
    {
        var advanceDataList = await GetBookingAdvancePaymentByDate(strQueryDate);

        var dataList = new List<BookingDailySalesReportVm>();

        if (advanceDataList != null && advanceDataList.Count > 0)
        {
            foreach (var advance in advanceDataList)
            {
                var model = new BookingDailySalesReportVm();
                model.BookingId = advance.BookingId;
                model.GuestId = advance.GuestId;
                model.GuestName = advance.GuestName;
                model.RoomId = advance.RoomId;
                model.RoomNo = advance.RoomNo;
                model.RoomRent = advance.RoomRent;
                model.CheckInTime = advance.CheckInTime;
                model.CheckOutTime = advance.CheckOutTime;
                model.ReservationAdvance = advance.PaidAmount;
                model.AdvanceAmount = advance.AdvanceAmount;
                model.NetReceive = model.ReservationAdvance + model.AdvanceAmount;
                model.IsAdvance = true;
                model.BillNumber = advance.BillNumber;

                //var existBooking = dataList.FirstOrDefault(x=>x.BookingId == model.BookingId && model.BillNumber != null);
                //if (existBooking != null)
                //    continue;

                dataList.Add(model);
            }
        }

        return dataList;
    }

    #endregion

    #region RoomOccupancyReportHtml
    public async Task<List<RoomOccupancyReportVm>> GetRoomOccupancyData(RoomOccupancyReportVm vm)
    {
        vm.FromDateStr = (string.IsNullOrEmpty(vm.FromDateStr)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.FromDateStr;
        vm.ToDateStr = (string.IsNullOrEmpty(vm.ToDateStr)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.ToDateStr;

        vm.FromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.FromDateStr));
        vm.ToDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.ToDateStr));

        var queryDateFrom = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.FromDateStr)).ToString("dd/MMM/yyyy");
        var queryDateTo = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.ToDateStr)).ToString("dd/MMM/yyyy");




        #region complex query
        //string query = $@"DECLARE @StartDate DATE = '{queryDateFrom}';
        //    DECLARE @EndDate DATE = '{queryDateTo}';

        //    WITH DateRange AS (
        //        select @StartDate AS ReportDate
        //        union all
        //        select DATEADD(DAY, 1, ReportDate)
        //        from DateRange
        //        where DATEADD(DAY, 1, ReportDate) <= @EndDate
        //    ),
        //    ExpandedRoomBookings AS (
        //        select RoomId,cast(Dates.ReportDate AS DATE) BookingDate
        //        from HtBookingRooms br
        //     inner join HtBookingServices bs on bs.Id = br.BookingId
        //        cross apply (
        //            select DATEADD(DAY, N, br.ActualCheckInTime) AS ReportDate
        //            from (VALUES
        //            (0), (1), (2), (3), (4), (5), (6), (7), (8), (9), (10), (11), (12), (13), (14), (15), (16), (17), (18), (19), (20), (21), (22), (23), (24), (25), (26), (27), (28), (29), (30)
        //            ,(31), (32), (33), (34), (35), (36), (37), (38), (39), (40), (41), (42), (43), (44), (45), (46), (47), (48), (49), (50), (51), (52), (53), (54), (55), (56), (57), (58), (59), (60)
        //            ) X(N) 
        //            where DATEADD(DAY, N, br.ActualCheckInTime) <= isnull(CAST(br.ActualCheckOutTime AS DATE), CAST(br.CheckOutTime AS DATE))
        //      /* or cast(br.ActualCheckInTime as date) = cast(br.ActualCheckOutTime as date) */
        //        ) Dates
        //        where cast(br.ActualCheckInTime AS DATE) <= @EndDate AND isnull(CAST(br.ActualCheckOutTime AS DATE), CAST(br.CheckOutTime AS DATE))  >= @StartDate
        //        union all
        //        select RoomId,cast(br.ActualCheckInTime AS DATE) BookingDate
        //        from HtBookingRooms br
        //     inner join HtBookingServices bs on bs.Id = br.BookingId
        //        where cast(br.ActualCheckInTime AS DATE) = isnull(CAST(br.ActualCheckOutTime AS DATE), CAST(br.CheckOutTime AS DATE))
        //    ),
        //    RoomSales AS (
        //        select BookingDate,count( /*DISTINCT*/ RoomID) AS TotalSalesRooms from ExpandedRoomBookings group by BookingDate
        //    )
        //    select
        //        D.ReportDate,COUNT(R.Id) as TotalRoom,isnull(S.TotalSalesRooms, 0) as RoomSold,
        //        round(isnull(S.TotalSalesRooms, 0) * 100.0 / count(R.Id), 2) AS OccupencyRate,p.PaidAmount SalesAmount
        //    from DateRange D
        //    cross join HtRoomInfos R
        //    left join RoomSales S ON D.ReportDate = S.BookingDate
        //    left join (
        //    select cast(bp.PaidDate as date) PaidDate,sum(bp.PaidAmount) PaidAmount
        //    from HtBookingPayments bp
        //    inner join HtBookingServices bs on bs.Id = bp.BookingId
        //    where bs.BookingType = 'R'
        //    group by  cast(bp.PaidDate as date)
        //    ) p on p.PaidDate = d.ReportDate
        //    GROUP BY D.ReportDate, S.TotalSalesRooms,P.PaidAmount
        //    ORDER BY D.ReportDate OPTION (MAXRECURSION 0)";

        #endregion

        #region loopQuery

        DateTime loopDate = vm.FromDate;
        string query = $@" select d.DateRange ReportDate,count(ri.id) TotalRoom,isnull(s.RoomSold,0) RoomSold,isnull(p.PaidAmount,0) SalesAmount,
        round(isnull(s.RoomSold,0) * 100.0 / count(ri.Id), 2) AS OccupencyRate from ( ";
        while (loopDate <= vm.ToDate)
        {
            query += (loopDate.Date == vm.FromDate) ? "" : " union all ";
            query += $@"select '{loopDate:dd/MMM/yyyy}' as DateRange";
            loopDate = loopDate.AddDays(1);
        }
        query += $@" ) d cross join HtRoomInfos ri left join  ( ";
        loopDate = vm.FromDate;
        while (loopDate <= vm.ToDate)
        {
            query += (loopDate.Date == vm.FromDate) ? "" : " union all ";
            query += $@"select '{loopDate:dd/MMM/yyyy}' ReportDate,count(br.RoomId) RoomSold
            from HtBookingRooms br inner join HtBookingServices bs on bs.Id = br.BookingId
            where cast(br.ActualCheckInTime as date ) = '{loopDate:dd/MMM/yyyy}' 
            or (cast(br.ActualCheckInTime as date) < '{loopDate:dd/MMM/yyyy}' 
            and isnull(cast(br.ActualCheckOutTime as date), cast(br.CheckOutTime as date)) > '{loopDate:dd/MMM/yyyy}')";

            loopDate = loopDate.AddDays(1);
        }
        query += $@" ) s on s.ReportDate = d.DateRange
        left join (
        select cast(bp.PaidDate as date) PaidDate,sum(bp.PaidAmount) PaidAmount
        from HtBookingPayments bp
        inner join HtBookingServices bs on bs.Id = bp.BookingId
        where bs.BookingType IN ('R','H')
        group by  cast(bp.PaidDate as date)
        ) p on p.PaidDate = d.DateRange
        group by d.DateRange,isnull(s.RoomSold,0),isnull(p.PaidAmount,0) 
        order by d.DateRange";
        #endregion

        var data = await _iReadDbConnection.QueryAsync<RoomOccupancyReportVm>(query);
        return data.ToList();
    }
    public async Task<string> RoomOccupancyReportHtml(RoomOccupancyReportVm vm)
    {
        try
        {
            string fullHtml = "";
            List<RoomOccupancyReportVm> objDataList = await GetRoomOccupancyData(vm);
            decimal totalRoom = objDataList.Sum(o => o.TotalRoom);
            decimal soldRoom = objDataList.Sum(o => o.RoomSold);
            decimal totalAmount = objDataList.Sum(o => o.SalesAmount);

            decimal totalOccupency = (totalRoom > 0 && soldRoom > 0) ? Math.Round((soldRoom / totalRoom) * 100, 2) : 0;
            decimal arr = (soldRoom > 0 && totalAmount > 0) ? Math.Round(totalAmount / soldRoom, 2) : 0;

            //decimal totalOccupency = (objDataList.Sum(o => o.TotalRoom) > 0 && objDataList.Sum(o => o.RoomSold) > 0) ?
            //Math.Round(Convert.ToDecimal(objDataList.Sum(o => o.RoomSold) / objDataList.Sum(o => o.TotalRoom)) * 100, 2) : 0;

            if (objDataList.Count > 0)
            {
                fullHtml += "<table class='table table-bordered report-table' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
                fullHtml += "<thead>";
                fullHtml += $@"<tr style='height:30px;'><td colspan='6' class='text-center'>Date: {DateTime.Today:dd/MMM/yyyy}</td></tr>";
                fullHtml += "<tr style='height:30px;' class='text-center'>";
                fullHtml += $@"<th style='width:60px;'>SL</th>
                            <th style='width:80px;'> Date </th>
                            <th style='width:100px;'> Total Rooms </th>
                            <th style='width:100px;'> Sold Rooms</th>
                            <th style='width:130px;'> Sales Amount </th>
                            <th style='width:100px;'> Occupancy </th>";

                fullHtml += "</tr>";
                fullHtml += "</thead>";
                fullHtml += "<tbody>";

                for (int i = 0; i < objDataList.Count; i++)
                {
                    RoomOccupancyReportVm objItem = objDataList[i];
                    fullHtml += "<tr>";

                    fullHtml += $@"<td style='text-align:left;'>{i + 1}</td>";
                    fullHtml += $@"<td style='text-align:center;'><b>{objItem.ReportDate:dd/MM/yyyy}</b></td>                              
                                <td style='text-align:center;'>{objItem.TotalRoom}</td>
                                <td style='text-align:center;'>{objItem.RoomSold}</td>
                                <td style='text-align:center;'>{objItem.SalesAmount:N2}</td>
                                <td style='text-align:center;'>{objItem.OccupencyRate:F2} %</td>";
                    fullHtml += "</tr>";

                }

                fullHtml += "</tbody>";

                fullHtml += "<tfoot>";
                fullHtml += $@"<tr><td colspan='2' style='text-align:right;'><b>TOTAL</b></td>                              
                                <td style='text-align:center;'>{objDataList.Sum(o => o.TotalRoom)}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.RoomSold)}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.SalesAmount):F2}</td>
                                <td style='text-align:center;'>{totalOccupency:F2}</td>";
                fullHtml += "</tr>";
                fullHtml += $@"<tr><td colspan='2' style='text-align:right;'><b>Occupancy Rate : </b></td>                              
                                <td style='text-align:center;'>{totalOccupency:F2} %</td>
                                <td style='text-align:center;'>Average Room Rate (ARR)</td>
                                <td style='text-align:center;'>{arr:F2}</td>
                                <td style='text-align:center;'></td>";
                fullHtml += "</tr>";
                fullHtml += "</tfoot>";


                fullHtml += "</table>";

                // === Footer Section ===
                fullHtml += @"<div class='report-footer'>
    			<hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
    			<p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

                fullHtml += @"</div>";
            }


            return fullHtml;

        }
        catch (Exception ex)
        {
            throw;
        }
    }
    #endregion

    #region CategoryWiseRoomInfoReportHtml

    public async Task<string> CategoryWiseRoomAvailableReportHtml(CategoryWiseRoomAvailableReportVm vm, bool isPrint = false)
    {
        var data = await CategoryWiseRoomAvailableReportData(vm);
        if (data == null || !data.Any())
            return "<p class='text-danger text-center'>No data found for the selected criteria.</p>";

        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StartDateStr));
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.EndDateStr));

        int dateDiff = (toDate - fromDate).Days + 1; // Including both start and end date
        var dateList = AppUtility.DateRangeList(fromDate, toDate);

        var totalList = data.Where(x => x.CategoryName == "***TOTAL***").ToList();
        var categoryList = data.Where(x => x.CategoryName != "***TOTAL***").DistinctBy(x => x.CategoryName).ToList();

        StringBuilder html = new StringBuilder();
        if (isPrint)
        {
            html.Append($"<h6 style='text-align:center;padding-bottom:5px; font-size:10px;'><b>Report Date: {vm.StartDateStr} to {vm.EndDateStr}</b> </h6>");
        }
        html.Append("<table class='table table-bordered table-responsive' id='print_table' style='width:100%;border-collapse:collapse; repeat-header:yes;' border='1' >");

        // Header
        html.Append($@"<thead>
                    <tr style='text-align:center;height:32px;'>
                        <th style='width:100px;'></th>");

        if (dateList.Count() > 0)
        {
            foreach (var date in dateList)
            {
                html.Append($@"<th style='width:30px;text-align:left;'>{date.ToString("MMM")}<br/>{date.ToString("dd")}<br/>{date:ddd}</th>");
            }
        }

        html.Append("</tr></thead>");

        html.Append("<tbody>");

        #region Total Rooms
        if (dateList.Count() > 0)
        {
            html.Append("<tr style='font-weight:500;height:30px;'>");
            html.AppendFormat("<td style='text-align:left;width:150px;'>Total Room</td>");

            foreach (var date in dateList)
            {
                var filterData = totalList.FirstOrDefault(x => x.BookingDate.Date == date.Date);

                html.Append($@"<td style='text-align:center;'>{filterData.TotalRooms}</td>");
            }
            html.Append("</tr>");
        }
        #endregion

        #region Expected Arrivals
        if (dateList.Count() > 0)
        {
            html.Append("<tr style='font-weight:500;height:30px;'>");
            html.AppendFormat("<td style='text-align:left;'>Expected Arrival</td>");

            foreach (var date in dateList)
            {
                var filterData = totalList.FirstOrDefault(x => x.BookingDate.Date == date.Date);

                html.Append($@"<td style='text-align:center;'>{filterData.ExpectedArrivals}</td>");
            }
            html.Append("</tr>");
        }
        #endregion

        #region Expected Departure
        if (dateList.Count() > 0)
        {
            html.Append("<tr style='font-weight:500;height:30px;'>");
            html.AppendFormat("<td style='text-align:left;'>Expected Departure</td>");

            foreach (var date in dateList)
            {
                var filterData = totalList.FirstOrDefault(x => x.BookingDate.Date == date.Date);

                html.Append($@"<td style='text-align:center;'>{filterData.ExpectedDepartures}</td>");
            }
            html.Append("</tr>");
        }
        #endregion

        #region In House
        if (dateList.Count() > 0)
        {
            html.Append("<tr style='font-weight:500;height:30px;'>");
            html.AppendFormat("<td style='text-align:left;'>In House</td>");

            foreach (var date in dateList)
            {
                var filterData = totalList.FirstOrDefault(x => x.BookingDate.Date == date.Date);

                html.Append($@"<td style='text-align:center;'>{filterData.InHouseNights}</td>");
            }
            html.Append("</tr>");
        }
        #endregion

        #region Occupancy
        if (dateList.Count() > 0)
        {
            html.Append("<tr style='font-weight:500;height:30px;'>");
            html.AppendFormat("<td style='text-align:left;'>Occupancy %</td>");

            foreach (var date in dateList)
            {
                var filterData = totalList.FirstOrDefault(x => x.BookingDate.Date == date.Date);

                var colorOcp = filterData.OccupancyPct > 0 ? "background-color: yellowgreen;" : "background-color: lightgray;";

                html.Append($@"<td style='text-align:center;{colorOcp}'>{filterData.OccupancyPct} %</td>");
            }
            html.Append("</tr>");
        }
        #endregion

        #region Out Of Order	
        if (dateList.Count() > 0)
        {
            html.Append("<tr style='font-weight:500;height:30px;'>");
            html.AppendFormat("<td style='text-align:left;'>Out Of Order</td>");

            foreach (var date in dateList)
            {
                var filterData = totalList.FirstOrDefault(x => x.BookingDate.Date == date.Date);

                var colorOFO = filterData.OutOfOrderRooms > 0 ? "background-color: indianred; color:white;" : "background-color: yellowgreen;";

                html.Append($@"<td style='text-align:center;{colorOFO}'>{filterData.OutOfOrderRooms}</td>");
            }
            html.Append("</tr>");
        }
        #endregion

        #region Vacant
        if (dateList.Count() > 0)
        {
            html.Append("<tr style='font-weight:500;height:30px;'>");
            html.AppendFormat("<td style='text-align:left;'>Vacant</td>");

            foreach (var date in dateList)
            {
                var filterData = totalList.FirstOrDefault(x => x.BookingDate.Date == date.Date);

                var colorVacant = filterData.VacantRooms > 0 ? "background-color: yellowgreen;" : "background-color: lightgray;";

                html.Append($@"<td style='text-align:center;{colorVacant}'>{filterData.VacantRooms}</td>");
            }
            html.Append("</tr>");
        }
        #endregion

        html.Append("<tr style='font-weight:500;height:30px;'>");
        html.AppendFormat($@"<td colspan='{dateList.Count() + 1}' style='text-align:center;font-weight:bold;font-size:16px;'>Available Room Type Wise</td>");
        html.Append("</tr>");

        if (categoryList.Count > 0)
        {
            foreach (var category in categoryList)
            {
                html.Append("<tr style='font-weight:500;height:30px;'>");
                html.AppendFormat("<td style='text-align:left;width:30%;'>{0}-({1})</td>", category.CategoryName, category.AvailableRooms);

                if (dateList.Count() > 0)
                {
                    foreach (var date in dateList)
                    {
                        var filterData = data.FirstOrDefault(x => x.BookingDate.Date == date.Date && x.CategoryName == category.CategoryName);

                        var colorVc = filterData.VacantRooms > 0 is false ? "background-color: indianred; color:white;" : "";

                        html.Append($@"<td style='text-align:center;{colorVc}'>{filterData.VacantRooms}</td>");
                    }
                }

                html.Append("</tr>");
            }
        }

        html.Append("</tbody>");

        html.Append("</table>");

        return html.ToString();
    }

    #endregion

    #region CategoryWiseRoomInfoReporData

    public async Task<List<CategoryWiseRoomAvailableReportVm>> CategoryWiseRoomAvailableReportData(CategoryWiseRoomAvailableReportVm vm)
    {
        vm.StartDateStr = (string.IsNullOrEmpty(vm.StartDateStr)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StartDateStr;
        vm.EndDateStr = (string.IsNullOrEmpty(vm.EndDateStr)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.EndDateStr;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StartDateStr)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.EndDateStr)).ToString("dd/MMM/yyyy");
        string fromDateFilter = (!string.IsNullOrEmpty(vm.StartDateStr)) ? $" and convert(date,CheckInTime) >= '{fromDate}'" : "";
        string toDateFilter = (!string.IsNullOrEmpty(vm.EndDateStr)) ? $" and convert(date,CheckInTime) <= '{toDate}'" : "";

        string dateFilter = (!string.IsNullOrEmpty(vm.EndDateStr)) ? $" DECLARE @StartDate DATE = '{fromDate}'; DECLARE @EndDate DATE = '{toDate}';" : "";

        string query = $@" {dateFilter}
                        ;WITH DateList AS (
                        SELECT DATEADD(DAY, number, @StartDate) AS BookingDate
                        FROM master.dbo.spt_values
                        WHERE type = 'P' 
                          AND number <= DATEDIFF(DAY, @StartDate, @EndDate)
                        ),
                        AllRoomDates AS (
                            SELECT 
                                d.BookingDate,
                                c.id AS CategoryId,
                                c.CategoryName,
                                r.id AS RoomId,
                                r.CleaningStatus Status
                            FROM DateList d
                            JOIN HtRoomInfos r ON 1=1
                            JOIN HtRoomCategories c ON c.id = r.RoomCategoryId
                        ),
                        BookedRoomDates AS (
                            SELECT 
                                rc.Id AS CategoryId,
                                CAST(d.BookingDate AS DATE) AS BookingDate,
                                COUNT(br.RoomCategoryId) AS BookedCount
                            FROM HtBookingRooms br
                            JOIN HtRoomCategories rc ON rc.Id = br.RoomCategoryId
                            JOIN DateList d 
                                ON d.BookingDate >= CAST(ISNULL(br.ActualCheckInTime, br.CheckInTime) AS DATE)
                               AND d.BookingDate <  CAST(ISNULL(br.ActualCheckOutTime, br.CheckOutTime) AS DATE)
                            GROUP BY rc.id, d.BookingDate
                        ),
                        ExpectedArrival AS (
                            SELECT 
                                CAST(ISNULL(br.ActualCheckInTime, br.CheckInTime) AS DATE) AS BookingDate,
                                br.RoomCategoryId AS CategoryId,
                                COUNT(*) AS ExpectedArrivals
                            FROM HtBookingRooms br
                            GROUP BY CAST(ISNULL(br.ActualCheckInTime, br.CheckInTime) AS DATE), br.RoomCategoryId
                        ),
                        ExpectedDeparture AS (
                            SELECT 
                                CAST(ISNULL(br.ActualCheckOutTime, br.CheckOutTime) AS DATE) AS BookingDate,
                                br.RoomCategoryId AS CategoryId,
                                COUNT(*) AS ExpectedDepartures
                            FROM HtBookingRooms br
                            GROUP BY CAST(ISNULL(br.ActualCheckOutTime, br.CheckOutTime) AS DATE), br.RoomCategoryId
                        ),
                        RoomTotals AS (
                            SELECT 
                                r.RoomCategoryId AS CategoryId,
                                COUNT(*) AS TotalRooms
                            FROM HtRoomInfos r
                            GROUP BY r.RoomCategoryId
                        ),
                        RoomStatuses AS (
                            SELECT
                                a.BookingDate,
                                a.CategoryId,
                                a.CategoryName,
                                ISNULL(b.BookedCount, 0) AS InHouseNights,
                                t.TotalRooms,
                                SUM(CASE WHEN a.Status = 4 THEN 1 ELSE 0 END) AS OutOfOrderRooms,
                                SUM(CASE WHEN a.Status = 5 THEN 1 ELSE 0 END) AS HouseUseRooms,
                                SUM(CASE WHEN a.Status = 6 THEN 1 ELSE 0 END) AS OutOfServiceRooms,
                                ISNULL(ea.ExpectedArrivals, 0) AS ExpectedArrivals,
                                ISNULL(ed.ExpectedDepartures, 0) AS ExpectedDepartures
                            FROM AllRoomDates a
                            LEFT JOIN BookedRoomDates b 
                                ON a.CategoryId = b.CategoryId AND a.BookingDate = b.BookingDate
                            LEFT JOIN RoomTotals t 
                                ON a.CategoryId = t.CategoryId
                            LEFT JOIN ExpectedArrival ea 
                                ON a.BookingDate = ea.BookingDate AND a.CategoryId = ea.CategoryId
                            LEFT JOIN ExpectedDeparture ed 
                                ON a.BookingDate = ed.BookingDate AND a.CategoryId = ed.CategoryId
                            GROUP BY a.BookingDate, a.CategoryId, a.CategoryName, b.BookedCount, t.TotalRooms, ea.ExpectedArrivals, ed.ExpectedDepartures
                        )
                        SELECT 
                            rs.BookingDate,
                            ISNULL(rs.CategoryName, '***TOTAL***') AS CategoryName,
                            SUM(rs.TotalRooms - rs.OutOfOrderRooms - rs.OutOfServiceRooms - rs.HouseUseRooms) AS AvailableRooms,
                            SUM((rs.TotalRooms - rs.OutOfOrderRooms - rs.OutOfServiceRooms - rs.HouseUseRooms) - rs.InHouseNights) AS VacantRooms,
                            SUM(rs.ExpectedArrivals) AS ExpectedArrivals,
                            SUM(rs.ExpectedDepartures) AS ExpectedDepartures,
                            SUM(rs.InHouseNights) AS InHouseNights,
	                        SUM(rs.OutOfOrderRooms) AS OutOfOrderRooms,
                            SUM(rs.TotalRooms) AS TotalRooms,
                            CAST(
                                CASE 
                                    WHEN SUM(rs.TotalRooms - rs.OutOfOrderRooms - rs.OutOfServiceRooms - rs.HouseUseRooms) = 0 
                                        THEN 0
                                    ELSE (SUM(rs.InHouseNights) * 100.0) / 
                                         SUM(rs.TotalRooms - rs.OutOfOrderRooms - rs.OutOfServiceRooms - rs.HouseUseRooms)
                                END AS DECIMAL(5,2)
                            ) AS OccupancyPct
                        FROM RoomStatuses rs
                        GROUP BY GROUPING SETS (
                            (rs.BookingDate, rs.CategoryName),
                            (rs.BookingDate)                   
                        )
                        ORDER BY rs.BookingDate, CategoryName;";



        var data = await _iReadDbConnection.QueryAsync<CategoryWiseRoomAvailableReportVm>(query);
        return data.ToList();
    }

    #endregion

    #region CancelReportHtml

    public async Task<string> CancelReportHtml(CancelReportVm vm, bool isPrint = false)
    {
        var data = await CancelReportData(vm);

        if (data == null || !data.Any())
            return "<p class='text-danger text-center'>No data found for the selected criteria.</p>";

        StringBuilder html = new StringBuilder();
        if (isPrint)
        {
            html.Append($"<h6 style='text-align:center;padding-bottom:5px; font-size:10px;'><b> From: </b>{vm.FromDateStr} <b> To </b>{vm.ToDateStr}</h6>");
        }
        html.Append("<table class='table table-bordered table-responsive' id='print_table' style='width:100%;border-collapse:collapse; repeat-header:yes;font-size:10px;' border='1' >");

        // Header

        html.Append($@"<thead>
                 <tr style='text-align:center;height:32px;'>
                     <th style='width:8%'>Conf. No.</th>
                     <th style='width:10%'>Name</th>
                     <th style='width:15%'>Room</th>
                     <th style='width:10%'>Room Type</th>
                     <th style='width:5%'>PAX</th>
                     <th style='width:8%'>Cancel Date</th>
                     <th style='width:8%'>Arrival</th>
                     <th style='width:8%'>Departure</th>
                     <th style='width:10%'>Net Rent</th>
                     <th style='width:13%'>Reason for reservation Cancel</th>
                     <th style='width:12%'>Cancel By</th>
                 </tr>
             </thead>");

        html.Append("<tbody>");
        foreach (var item in data.OrderBy(x => x.CheckInTime))
        {
            html.Append("<tr style='font-weight:500;height:30px;'>");
            html.AppendFormat("<td style='text-align:center;'>{0}</td>", item.BookingNo);
            html.AppendFormat("<td style='text-align:left;'>{0}</td>", item.GuestName);
            html.AppendFormat("<td style='text-align:center;'>{0}</td>", item.Company);

            var shortCategory = !string.IsNullOrEmpty(item.RoomNo) && item.CategoryName?.Length > 30
                ? item.CategoryName.Substring(0, 30) + "..."
                : "-C-X-L-D-";

            html.AppendFormat("<td style='text-align:left;'>{0:F0,1}</td>", item.RoomNo, shortCategory);
            html.AppendFormat("<td style='text-align:center;'>{0:F0}</td>", item.TotalGuest);
            html.AppendFormat("<td style='text-align:center;'>{0}</td>", item.CancelDate.ToString("dd-MMM-yyyy"));
            html.AppendFormat("<td style='text-align:center;'>{0}</td>", item.CheckInTime.ToString("dd-MMM-yyyy"));
            html.AppendFormat("<td style='text-align:center;'>{0}</td>", item.CheckOutTime.ToString("dd-MMM-yyyy"));
            html.AppendFormat("<td style='text-align:center;'>{0:F0}</td>", item.RoomRate.ToString("N2"));
            html.AppendFormat("<td style='text-align:center;'>{0}</td>", item.CancelReason);
            html.AppendFormat("<td style='text-align:center;'>{0}</td>", item.CancelBy);

            html.Append("</tr>");
        }

        html.Append("<tr style='font-weight:500;height:30px;'>");

        var totalBooking = data.Count;
        var totalGuest = data.Sum(x => x.TotalGuest);
        var totalRate = data.Sum(x => x.RoomRate);

        html.Append($@"<td colspan='12'>
                         <table style='width:100%;font-size:10px;'>
                             <tr>
                                 <td style='text-align:left;'>No Of Booking: {totalBooking}</td>
                                 <td style='text-align:left;'>No Of PAX: {totalGuest}</td>
                                 <td style='text-align:left;'>Total Rate: {totalRate.ToString("N2")}</td>
                             </tr>
                         </table>
                     </td>");

        html.Append("</tr>");

        html.Append("</tbody>");

        html.Append("</table>");

        return html.ToString();
    }

    #endregion

    #region CancelReportData

    public async Task<List<CancelReportVm>> CancelReportData(CancelReportVm vm)
    {
        vm.FromDateStr = (string.IsNullOrEmpty(vm.FromDateStr)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.FromDateStr;
        vm.ToDateStr = (string.IsNullOrEmpty(vm.ToDateStr)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.ToDateStr;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.FromDateStr)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.ToDateStr)).ToString("dd/MMM/yyyy");
        string fromDateFilter = (!string.IsNullOrEmpty(vm.FromDateStr)) ? $" and convert(date,CheckInTime) >= '{fromDate}'" : "";
        string toDateFilter = (!string.IsNullOrEmpty(vm.ToDateStr)) ? $" and convert(date,CheckInTime) <= '{toDate}'" : "";

        string bookingFilter = (!string.IsNullOrEmpty(vm.BookingFilter)) ? $" and bs.BookingNo like '%{vm.BookingFilter}%'" : "";
        string guestFilter = (!string.IsNullOrEmpty(vm.GuestFilter)) ? $"and isnull(gi.Salutation,'')+' '+isnull(gi.FirstName,'')+' '+isnull(gi.LastName,'') like '%{vm.GuestFilter}%'" : "";
        string dateFilter = (!string.IsNullOrEmpty(vm.ToDateStr)) ? $" and convert(date,bs.CancelDate) between '{fromDate}' and '{toDate}'" : "";




        string query = $@"select bs.id BookingId,bs.BookingNo,bs.BookingDate,gi.id GuestId,isnull(gi.Salutation,'')+' '+isnull(gi.FirstName,'')+' '+isnull(gi.LastName,'') GuestName,cc.Name company,c.Name Country
                     ,br.TotalGuest,bs.CancelDate,bs.CancelReason, u.FullName CancelBy, br.CheckInTime,br.CheckOutTime
                     ,ri.RoomNo,rc.CategoryName,br.NetRent,DATEDIFF(DAY, br.CheckInTime, br.CheckOutTime) AS NoOfNights,
                     CASE WHEN DATEDIFF(DAY, br.CheckInTime, br.CheckOutTime) = 0 THEN br.NetRent
                     ELSE br.NetRent / DATEDIFF(DAY, br.CheckInTime, br.CheckOutTime) END AS RoomRate
                     from HtBookingServices bs 
                     inner join HtBookingRooms br on br.BookingId = bs.Id
                     left join HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
                     left join HtGuestInfos gi on gi.Id = bg.GuestId
                     left join ClientCompanies cc on cc.Id = gi.CompanyId
                     left join SetCountries c on c.Id = gi.CountryId
                     left join HtRoomCategories rc on rc.Id = br.RoomCategoryId
                     left join HtRoomInfos ri on ri.Id = br.RoomId
                     left join AspNetUsers u on u.Id = bs.CancelBy
                     where bs.BookingStatus = 9 and bs.IsDeleted = 0
                     {dateFilter}
                     {guestFilter}
                     {bookingFilter}
                     group by bs.id,bs.BookingNo,bs.BookingDate,gi.id,isnull(gi.Salutation,'')+' '+isnull(gi.FirstName,'')+' '+isnull(gi.LastName,''),cc.Name,c.Name
                     ,br.TotalGuest,bs.CancelDate,bs.CancelReason,bs.CancelBy,br.CheckInTime,br.CheckOutTime,br.NetRent,ri.RoomNo,rc.CategoryName,u.FullName
                     order by bs.id desc";

        var data = await _iReadDbConnection.QueryAsync<CancelReportVm>(query);
        return data.ToList();
    }

    #endregion

    #region ReservationRepotHtml
    public async Task<string> ReservationReportHtml(ReservationReportVm vm, bool isPrint = false)
    {
        var data = await ReservationRepotData(vm);

        if (data == null || !data.Any())
            return "<p class='text-danger text-center'>No data found for the selected criteria.</p>";

        StringBuilder html = new StringBuilder();
        if (isPrint)
        {
            html.Append($"<h6 style='text-align:center;padding-bottom:5px; font-size:10px;'><b> From: </b>{vm.FromDateStr} <b> To </b>{vm.ToDateStr}</h6>");
        }
        html.Append("<table class='table table-hover report-table' id='print_table' style='width:100%;border-collapse:collapse; repeat-header:yes;' border='1' >");

        // Header
        html.Append($@"<thead>
                    <tr style='text-align:center;height:32px;'>
                        <th style='width:8%'>Res. No.</th>
                        <th style='width:10%'>Guest Name</th>
                        <th style='width:10%'>Company Name</th>
                        <th style='width:15%'>Room Type</th>
                        <th style='width:3%'>PAX</th>
                        <th style='width:8%'>Arrival</th>
                        <th style='width:8%'>Departure</th>
                        <th style='width:7%'>Rate</th>
                        <th style='width:7%'>Advance</th>
                        <th style='width:5%'>Room</th>
                        <th style='width:6%'>Status</th>
                        <th style='width:8%'>Remarks</th>
                        <th style='width:5%'>User ID</th>
                    </tr>
                </thead>");

        html.Append("<tbody>");

        var shownGuests = new HashSet<string>();

        foreach (var item in data)
        {
            var statusHtml = item.BookingConfirmStatus == BookingConfirmEnum.Confirm ? "<span class='badge badge-success'>Confirmed</span>" :
                "<span class='badge badge-info'>Waiting</span>";

            html.Append("<tr style='font-weight:500;height:30px;'>");
            //html.AppendFormat("<td style='text-align:center;'>{0}</td>", item.BookingNo);
            html.AppendFormat("<td style='text-align:center;'><a href='../../BookingService/Details/{0}' target='_blank'>{1}</a></td>", item.BookingId, item.BookingNo);
            html.AppendFormat($"<td style='text-align:center;'>{item.GuestName}<br />{item.GuestMobile}</td>");
            html.AppendFormat("<td style='text-align:center;'>{0:F0}</td>", !string.IsNullOrEmpty(item.Company) ? item.Company : "N/A");

            var shortCategory = item.CategoryName?.Length > 30 ? item.CategoryName.Substring(0, 30) + "..." : item.CategoryName;

            html.AppendFormat("<td style='text-align:left;'>{0:F0}</td>", shortCategory);
            html.AppendFormat("<td style='text-align:center;'>{0:F0}</td>", item.TotalGuest);
            html.AppendFormat("<td style='text-align:center;'>{0}</td>", item.CheckInTime.ToString("dd-MMM-yyyy"));
            html.AppendFormat("<td style='text-align:center;'>{0}</td>", item.CheckOutTime.ToString("dd-MMM-yyyy"));
            html.AppendFormat("<td style='text-align:center;'>{0:F0}</td>", item.RoomRate.ToString("N2"));

            if (!shownGuests.Contains(item.GuestName))
            {
                html.AppendFormat("<td style='text-align:center;'>{0}</td>", item.AdvancePayment.ToString("N2"));
                shownGuests.Add(item.GuestName);
            }
            else
            {
                html.AppendFormat("<td style='text-align:center;'>0</td>");
            }

            html.AppendFormat("<td style='text-align:center;'>{0:F0}</td>", !string.IsNullOrEmpty(item.RoomNo) ? item.RoomNo : "N/A");
            html.AppendFormat($"<td style='text-align:center;'>{statusHtml}</td>");

            html.AppendFormat($"<td style='text-align:center;'>{item.Remarks}</td>");
            html.AppendFormat("<td style='text-align:center;'>{0:F0}</td>", item.UserName);
            html.Append("</tr>");
        }

        html.Append("<tr style='font-weight:500;height:30px;'>");

        var totalBooking = data.Count;
        var totalGuest = data.Sum(x => x.TotalGuest);
        var totalRate = data.Sum(x => x.RoomRate);

        html.Append($@"<td colspan='13'>
                            <table style='width:100%;'>
                                <tr>
                                    <td style='text-align:left;'>No Of Reserved Rooms: {totalBooking}</td>
                                    <td style='text-align:left;'>No Of PAX: {totalGuest}</td>
                                    <td style='text-align:left;'>Total Rate: {totalRate.ToString("N2")}</td>
                                </tr>
                            </table>
                        </td>");

        html.Append("</tr>");

        html.Append("</tbody>");

        html.Append("</table>");

        return html.ToString();
    }

    #endregion

    #region ReservationRepotData

    public async Task<List<ReservationReportVm>> ReservationRepotData(ReservationReportVm vm)
    {
        vm.FromDateStr = (string.IsNullOrEmpty(vm.FromDateStr)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.FromDateStr;
        vm.ToDateStr = (string.IsNullOrEmpty(vm.ToDateStr)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.ToDateStr;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.FromDateStr)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.ToDateStr)).ToString("dd/MMM/yyyy");
        string dateFilter = (!string.IsNullOrEmpty(vm.ToDateStr)) ? $" and convert(date,br.CheckInTime) between '{fromDate}' and '{toDate}'" : "";

        string bookingFilter = (!string.IsNullOrEmpty(vm.BookingFilter)) ? $" and bs.BookingNo like '%{vm.BookingFilter}%'" : "";
        string guestFilter = (!string.IsNullOrEmpty(vm.GuestFilter)) ? $"and (isnull(gi.Salutation,'') + ' ' + isnull(gi.FirstName,'') + ' ' + isnull(gi.LastName,'')) like '%{vm.GuestFilter}%'" : "";

        //string query = $@"select bs.id BookingId,bs.BookingNo,bs.BookingDate,gi.id GuestId,isnull(gi.Salutation,'')+' '+isnull(gi.FirstName,'')+' '+isnull(gi.LastName,'') GuestName,gi.Mobile GuestMobile
        //                    ,gi.Email GuestEmail ,cc.Name company,c.Name Country,bs.TotalGuest,bs.CheckInTime,bs.CheckOutTime,bs.RoomDiscountPercent,bs.FoodDiscountPercent
        //                    ,(string_agg(cast(ri.RoomNo as nvarchar(max)), ', ') within group (order by ri.RoomNo)) RoomNoList
        //                    ,(string_agg(cast(rc.CategoryName as nvarchar(max)), ', ') within group (order by ri.RoomNo)) RoomCategoryList,bs.NetRent,p.AdvancePayment,bs.Remarks
        //                    from HtBookingServices bs 
        //                    inner join HtBookingRooms br on br.BookingId = bs.Id
        //                    left join HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
        //                    left join HtGuestInfos gi on gi.Id = bg.GuestId
        //                    left join ClientCompanies cc on cc.Id = gi.CompanyId
        //                    left join SetCountries c on c.Id = gi.CountryId
        //                    left join HtRoomInfos ri on ri.Id = br.RoomId
        //                    left join HtRoomCategories rc on rc.Id = ri.RoomCategoryId
        //                    left join (
        //                    select bp.BookingId,sum(bp.PaidAmount) AdvancePayment
        //                    from HtBookingPayments bp 
        //                    where bp.BillingId is null and bp.BookingId is not null
        //                    group by bp.BookingId
        //                    ) p on p.BookingId = bs.Id
        //                    where bs.BookingStatus != 9 and bs.IsDeleted = 0 {dateFilter} {bookingFilter} {guestFilter}
        //                    group by bs.id,bs.BookingNo,bs.BookingDate,gi.id,isnull(gi.Salutation,'')+' '+isnull(gi.FirstName,'')+' '+isnull(gi.LastName,''),cc.Name,c.Name
        //                    ,bs.TotalGuest,bs.CheckInTime,bs.CheckOutTime,bs.NetRent,gi.Mobile,bs.RoomDiscountPercent,bs.FoodDiscountPercent,p.AdvancePayment,gi.Email,bs.Remarks
        //                    order by bs.id desc ";


        //string query = $@"select bs.id BookingId,bs.BookingNo,bs.BookingDate,gi.id GuestId,isnull(gi.Salutation,'')+' '+isnull(gi.FirstName,'')+' '+isnull(gi.LastName,'') GuestName,gi.Mobile GuestMobile
        //            ,gi.Email GuestEmail ,cc.Name company,c.Name Country,br.TotalGuest,br.CheckInTime,br.CheckOutTime,bs.RoomDiscountPercent,bs.FoodDiscountPercent
        //            ,ri.RoomNo,ri.Id RoomId ,rc.CategoryName,rc.Id CategoryId,br.NetRent,br.Discount,p.AdvancePayment,bs.Remarks, DATEDIFF(DAY, br.CheckInTime, br.CheckOutTime) AS NoOfNights,ui.UserName
        //            ,br.RoomRent RoomRate                    
        //            from HtBookingServices bs 
        //            inner join HtBookingRooms br on br.BookingId = bs.Id
        //            inner join AspNetUsers ui on ui.Id = bs.ActionById
        //            left join HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
        //            left join HtGuestInfos gi on gi.Id = bg.GuestId
        //            left join ClientCompanies cc on cc.Id = gi.CompanyId
        //            left join SetCountries c on c.Id = gi.CountryId
        //            left join HtRoomCategories rc on rc.Id = br.RoomCategoryId
        //            left join HtRoomInfos ri on ri.Id = br.RoomId
        //            left join (
        //            select bp.BookingId,sum(bp.PaidAmount) AdvancePayment
        //            from HtBookingPayments bp 
        //            where bp.BillingId is null and bp.BookingId is not null
        //            group by bp.BookingId
        //            ) p on p.BookingId = bs.Id
        //            where bs.BookingStatus != 9 and bs.IsDeleted = 0 {dateFilter} {bookingFilter} {guestFilter}
        //            group by bs.id,bs.BookingNo,bs.BookingDate,gi.id,isnull(gi.Salutation,'')+' '+isnull(gi.FirstName,'')+' '+isnull(gi.LastName,''),cc.Name,c.Name,ui.UserName
        //            ,br.TotalGuest,br.CheckInTime,br.CheckOutTime,br.NetRent,br.Discount,gi.Mobile,bs.RoomDiscountPercent,bs.FoodDiscountPercent,p.AdvancePayment,gi.Email,bs.Remarks,ri.RoomNo ,rc.CategoryName,br.RoomRent,ri.Id,rc.Id
        //            order by bs.id desc";

        string query = $@"SELECT 
                    bs.Id AS BookingId,
                    bs.BookingNo,
                    bs.BookingDate,
                    bs.BookingConfirmStatus,
                    gi.Id AS GuestId,
                    ISNULL(gi.Salutation, '') + ' ' + ISNULL(gi.FirstName, '') + ' ' + ISNULL(gi.LastName, '') AS GuestName,
                    gi.Mobile AS GuestMobile,
                    gi.Email AS GuestEmail,
                    cc.Name AS Company,
                    c.Name AS Country,
                    br.TotalGuest,
                    br.CheckInTime,
                    br.CheckOutTime,
                    ri.RoomNo,
                    ri.Id AS RoomId,
                    rc.CategoryName,
                    rc.Id AS CategoryId,
                    br.NetRent,
                    br.Discount,
                    p.AdvancePayment,
                    bs.Remarks,
                    DATEDIFF(DAY, br.CheckInTime, br.CheckOutTime) AS NoOfNights,
                    ui.UserName,
                    br.RoomRent AS RoomRate
                FROM HtBookingServices bs
                INNER JOIN HtBookingRooms br ON br.BookingId = bs.Id
                INNER JOIN AspNetUsers ui ON ui.Id = bs.ActionById
                LEFT JOIN HtBookingGuests bg ON bg.BookingId = bs.Id AND bg.IsMain = 1
                LEFT JOIN HtGuestInfos gi ON gi.Id = bg.GuestId
                LEFT JOIN ClientCompanies cc ON cc.Id = gi.CompanyId
                LEFT JOIN SetCountries c ON c.Id = gi.CountryId
                LEFT JOIN HtRoomCategories rc ON rc.Id = br.RoomCategoryId
                LEFT JOIN HtRoomInfos ri ON ri.Id = br.RoomId
                LEFT JOIN (
                    SELECT 
                        bp.BookingId,
                        SUM(bp.PaidAmount) AS AdvancePayment
                    FROM HtBookingPayments bp
                    WHERE bp.BillingId IS NULL AND bp.BookingId IS NOT NULL
                    GROUP BY bp.BookingId
                ) p ON p.BookingId = bs.Id
                --WHERE bs.BookingStatus = 1 
                WHERE br.ActualCheckInTime IS NULL 
                  AND bs.IsDeleted = 0  
                  {dateFilter} {bookingFilter} {guestFilter}
                ORDER BY bs.Id DESC;";


        var data = await _iReadDbConnection.QueryAsync<ReservationReportVm>(query);
        return data.ToList();
    }

    #endregion
}
