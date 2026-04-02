using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.OnlineBooking;
using Interface.Repository.Common;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.HotelManagement;

public class OnlineBookingService : BaseService<HtOnlineBooking>, IOnlineBookingService
{
    #region Config
    private IOnlineBookingRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IBookingRoomRepository _iBookingRoomRepository;
    private readonly IBookingGuestRepository _iBookingGuestRepository;
    private readonly IBookingPaymentRepository _iBookingPaymentRepository;
    private readonly IRoomCategoryRepository _iRoomCategoryRepository;
    private readonly IRoomInfoRepository _iRoomInfoRepository;
    private readonly IAutoCodeRepository _iAutoCodeRepository;

    public OnlineBookingService(IOnlineBookingRepository repository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IBookingRoomRepository iBookingRoomRepository,
        IBookingGuestRepository iBookingGuestRepository,
        IBookingPaymentRepository iBookingPaymentRepository,
        IRoomCategoryRepository iRoomCategoryRepository,
        IRoomInfoRepository iRoomInfoRepository,
        IAutoCodeRepository iAutoCodeRepository) : base(repository, iUnitOfWork)
    {
        _iRepository = repository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iBookingRoomRepository = iBookingRoomRepository;
        _iBookingGuestRepository = iBookingGuestRepository;
        _iBookingPaymentRepository = iBookingPaymentRepository;
        _iRoomCategoryRepository = iRoomCategoryRepository;
        _iRoomInfoRepository = iRoomInfoRepository;
        _iAutoCodeRepository = iAutoCodeRepository;
    }

    #endregion

    #region OnlineBookingEntryAsync

    public async Task<(bool, long)> OnlineBookingEntryAsync(OnlineBookingVm vm)
    {
        var bookingModel = _iMapper.Map<HtOnlineBooking>(vm);
        bookingModel.OnlineBookingNumber = await GetOnlineBookingCode();
        bookingModel.OnlineBookingDate = DU.Utility.GetBdDateTimeNow();
        bookingModel.Status = OnlineBookingStatusEnum.Pending;
        bookingModel.ActionDate = DU.Utility.GetBdDateTimeNow();

        if (bookingModel.OnlineBookingDate.Date > bookingModel.ArrivalDate.Date)
            throw new Exception("Check-In Date Is Previous Date Than Booking Date..!!");

        if (bookingModel.ArrivalDate.Date > bookingModel.DepartureDate.Date)
            throw new Exception("CheakIn & CheckOut Date Is Not Correct..!!");

        if (vm.OnlineBookingDetails?.Count > 0 is false)
            throw new Exception("No Room Information Found...!!");

        if (string.IsNullOrEmpty(vm.GuestName) || string.IsNullOrEmpty(vm.GuestMobile))
            throw new Exception("No Guest Information Found...!!");

        //var bookingRooms = _iMapper.Map<List<HtOnlineBookingDetail>>(vm.OnlineBookingDetails);

        var bookingRooms = bookingModel.OnlineBookingDetails;

        if (bookingModel.OnlineBookingDetails.Count > 0)
        {
            foreach (var room in bookingModel.OnlineBookingDetails)
            {
                if (room.CheckInDate.Date > room.CheckOutDate.Date)
                    throw new Exception("Room CheakIn & CheckOut Date Is Not Correct..!!");

                room.ActionDate = Utility.GetBdDateTimeNow();

                var days = AppUtility.DaysDiffernceOnlyDate(room.CheckOutDate, room.CheckInDate);

                var categoryInfo = _iRoomCategoryRepository.GetFirstOrDefault(x => x.Id == room.RoomCategoryId && x.IsActive && !x.IsDeleted);
                if (categoryInfo == null)
                    throw new Exception("This Category Room Not Found...!!");

                if (days > 0)
                {
                    room.RoomRent = categoryInfo.OfferRate > 0 ? (double)categoryInfo.OfferRate: categoryInfo.TotalRent;
                    room.RoomCount = room.RoomCount;
                    room.TotalDays = days;
                    room.TotalRent = room.RoomRent * room.RoomCount * room.TotalDays;
                    room.Adult = room.Adult > 0 ? room.Adult : categoryInfo.Capacity;
                }
                else
                {
                    throw new Exception("Booking Days Have To More Than Zero....!!");
                }
            }

            bookingModel.TotalRent = bookingRooms.Sum(x => x.TotalRent);
            bookingModel.TotalRoomCount = bookingRooms.Sum(x => x.RoomCount);
            bookingModel.TotalAdult = bookingRooms.Sum(x => x.Adult);

            var minCheckInDate = bookingRooms.Select(x => x.CheckInDate).Min();
            var maxCheckOutDate = bookingRooms.Select(x => x.CheckOutDate).Max();

            if (minCheckInDate.Date != bookingModel.ArrivalDate.Date)
                bookingModel.ArrivalDate = minCheckInDate;

            if (maxCheckOutDate.Date != bookingModel.DepartureDate.Date)
                bookingModel.DepartureDate = maxCheckOutDate;

            bookingModel.OnlineBookingDetails = bookingRooms;
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iRepository.AddAsync(bookingModel);
        await _iUnitOfWork.CompleteAsync();

        ts.Complete();
        return (true, bookingModel.Id);
    }

    #endregion

    #region GetOnlineBookingCode

    public async Task<string> GetOnlineBookingCode()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.HtOnlineBookings.ToString(), "OnlineBookingNumber", "OBK", 6);
        return data;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<OnlineBookingSearchVm, OnlineBookingSearchVm>> SearchAsync(DataTablePagination<OnlineBookingSearchVm, OnlineBookingSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region OnlineBookingBillHtml
    public async Task<string> GetOnlineBookingBillByIdAsyncHtml(long id)
    {
        var data = await GetBookingBillByIdAsync(id);
        string leftAlign = "text-align:left;";
        string rightAlign = "text-align:right;";
        string noBorder = "border:0px;";
        string boldText = "font-weight:bold;";
        var fullHtml = $@"
        <div style='padding: 10px; font-family: Arial, sans-serif;'>
            <h2 style='text-align: center; color: #28a745;margin-bottom:10px;{boldText}'>Online Booking</h2>
            
            <div class='checkout_form'>
                <div class='row'>
                    <table style='width:100%;margin-bottom:10px;repeat-header:yes;' >
                        <tr style='height:30px; '>
                              <td  style='border:0px;width:30%'>
                                   <table style='width:100%;border:0px;' class='table-bill' border='0' >
                                        <tr style='height:17px;font-size:10px;'><td style='width:15%;{leftAlign + noBorder + boldText}'> Booking No</td><td style='{leftAlign + noBorder + boldText}'>: {data.OnlineBookingNumber}</td></tr>
                                        <tr style='height:17px;font-size:10px;'><td style='width:15%;{leftAlign + noBorder + boldText}'> Booking Date </td><td style='{leftAlign + noBorder}'>: {DU.Utility.ConvertDateToStr(data.OnlineBookingDate)}</td></tr>
                                        <tr style='height:17px;font-size:10px;'><td style='width:15%;{leftAlign + noBorder + boldText}'> Arrival Date</td><td style='{leftAlign + noBorder}'>: {DU.Utility.ConvertDateToStr(data.ArrivalDate)}</td></tr>
                                        <tr style='height:17px;font-size:10px;'><td style='width:15%;{leftAlign + noBorder + boldText}'> Departure Date</td><td style='{leftAlign + noBorder}'>: {DU.Utility.ConvertDateToStr(data.DepartureDate)}</td></tr>
                                       <tr style='height:17px;font-size:10px;'><td style='width:15%;{leftAlign + noBorder + boldText}'> Remarks</td><td style='{leftAlign + noBorder}'>: {data.Remarks}</td></tr>
                                  </table>
                              </td>
                              <td  style='border:0px;width:45%'>
                                   
                              </td>
                              <td  style='border:0px;width:25%' > 
                                     <table style='width:100%;border:0px;' class='table-bill' border='0' >
                                        <tr style='height:17px;font-size:10px;'><td style='width:30%;{leftAlign + noBorder + boldText}'> Name</td><td style='{leftAlign + noBorder}'>: {data.GuestName}</td></tr>
                                        <tr style='height:17px;font-size:10px;'><td style='width:30%;{leftAlign + noBorder + boldText}'> Phone </td><td style='{leftAlign + noBorder}'>: {data.GuestMobile}</td></tr>
                                        <tr style='height:17px;font-size:10px;'><td style='width:30%;{leftAlign + noBorder + boldText}'> Email</td><td style='{leftAlign + noBorder}'>: {data.GuestEmail}</td></tr>
                                        <tr style='height:17px;font-size:10px;'><td style='width:30%;{leftAlign + noBorder + boldText}'> Address</td><td style='{leftAlign + noBorder}'>: {data.GuestAddress}</td></tr>
                                        <tr style='height:17px;font-size:10px;'><td style='width:30%;{leftAlign + noBorder + boldText}'></td><td style='{leftAlign + noBorder}'></td></tr>
                                    </table>
                              </td>
                        </tr>
                    </table>
                </div>

                <div class='row mt-3'>
                    <table class='table table-bordered'>
                        <thead style='background-color:#28A745;color:white'>
                            <tr>
                                <th>Room Category</th>
                                <th>Check In</th>
                                <th>Check Out</th>
                                <th>Room Count</th>
                                <th>Total Night(s)</th>
                                <th>Rent</th>
                                <th>Total Rent</th>
                            </tr>
                        </thead>
                        <tbody>";

        if (data.OnlineBookingDetails != null && data.OnlineBookingDetails.Any())
        {
            foreach (var details in data.OnlineBookingDetails)
            {
                fullHtml += $@"
                            <tr>
                                <td data-label='Room Category'>{details.RoomCategoryName}</td>
                                <td data-label='Check In'>{details.CheckInDate:dd/MMM/yyyy}</td>
                                <td data-label='Check Out'>{details.CheckOutDate:dd/MMM/yyyy}</td>
                                <td data-label='Room Count'>{details.RoomCount}</td>
                                <td data-label='Total Night(s)'>{details.TotalDays} Night(s)</td>
                                <td data-label='Rent'  style='{rightAlign}'>{details.RoomRent} TK.</td>
                                <td data-label='Total Rent'style='{rightAlign}'>{details.TotalRent} TK.</td>
                            </tr>";
            }
        }
        else
        {
            fullHtml += @"
                            <tr>
                                <td colspan='7' class='text-center'>No Room Categories Found</td>
                            </tr>";
        }


        fullHtml += $@"
                        </tbody>
                        <tfoot>
                            <tr>
                                <td style='{rightAlign}' colspan='3'><b>Total Room(s)</b></td>
                                <td><b>{data.TotalRoomCount}</b></td>
                                <td style='{leftAlign}'><b>{data.OnlineBookingDetails?.Sum(x=>x.TotalDays).ToString("N0")} Night (s)</b></td>
                                <td style='{rightAlign}'><b>Grand Total</b></td>
                                <td style='{rightAlign}'><b>{data.TotalRent} TK.</b></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </div>
        </div>";

        return fullHtml;
    }

    #endregion

    #region GetOnlineBookingBillById
    public async Task<OnlineBookingVm> GetBookingBillByIdAsync(long id)
    {
        var data = await _iRepository.GetOnlineBookingByIdAsync(id);

        var model = _iMapper.Map<OnlineBookingVm>(data);

        model.OnlineBookingNumber = $"{data.OnlineBookingNumber}";
        model.OnlineBookingDate = data.OnlineBookingDate;
        model.ArrivalDate = data.ArrivalDate;
        model.DepartureDate = data.DepartureDate;
        model.GuestName = data.GuestName;
        model.GuestMobile = data.GuestMobile;
        model.Status = data.Status;

        if (model.OnlineBookingDetails.Any())
        {
            foreach (var item in model.OnlineBookingDetails)
            {
                var filterData = data.OnlineBookingDetails.Where(c => c.Id == item.Id).FirstOrDefault();

                item.RoomCategoryName = filterData.RoomCategory.CategoryName;
            }
        }

        return model;
    }

    #endregion

    #region UpcomingPendingBooking

    public async Task<List<OnlineBookingVm>> GetUpcomingPendingBooking()
    {
        var bookingList = await _iRepository.GetAsync(x => x.Status == OnlineBookingStatusEnum.Pending && x.ArrivalDate.Date >= DateTime.Now.Date && !x.IsDeleted);
        var dataList = _iMapper.Map<List<OnlineBookingVm>>(bookingList);
        return dataList;
    }

    #endregion
}