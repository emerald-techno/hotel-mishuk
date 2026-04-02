using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.GuestInfo;
using Interface.Repository.HotelManagement;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class GuestInfoRepository : BaseRepository<HtGuestInfo>, IGuestInfoRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public GuestInfoRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }

    #endregion

    public async Task<DataTablePagination<HtGuestInfoSearchVm, HtGuestInfoSearchVm>> SearchAsync(DataTablePagination<HtGuestInfoSearchVm, HtGuestInfoSearchVm> vm)
    {
        var searchResult = Context.HtGuestInfos
            .Include(x => x.Company)
            .AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search Guest info not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.FirstName.ToLower().Contains(value));
        }

        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.FirstName)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<HtGuestInfoSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.CompanyName = filterData.Company?.Name;

                var salutation = !string.IsNullOrEmpty(filterData.Salutation) ? filterData.Salutation : "";
                var firstName = !string.IsNullOrEmpty(filterData.FirstName) ? filterData.FirstName : "";
                var lastName = !string.IsNullOrEmpty(filterData.LastName) ? filterData.LastName : "";
                searchDto.FullName = $"{salutation} {firstName} {lastName}";
            }
        }
        return vm;
    }

    #region GetGuestInfo

    public async Task<HtGuestInfoVm> GetGuestByIdAsync(long id)
    {
        var dataModel = await Context.HtGuestInfos
            .Include(c => c.Country)
            .Include(c => c.District)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        var model = _iMapper.Map<HtGuestInfoVm>(dataModel);
        model.CountryName = dataModel?.Country?.Name;
        model.DistrictName = dataModel?.District?.Name;

        return model;
    }

    #endregion

    #region Guest Details
    public async Task<HtGuestDetailsVm> GetGuestDetailsByIdAsync(long id)
    {
        var bookingInfo = await Context.HtBookingGuests
            .Include(x => x.Booking)
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync(x => x.GuestId == id);
        var guestInfo = await Context.HtGuestInfos.FirstOrDefaultAsync(x => x.Id == id);

        var model = new HtGuestDetailsVm();
        model.PhotoUrl = guestInfo.PhotoUrl;
        model.GuestName = guestInfo.Salutation + ": " + guestInfo.FirstName + " " + guestInfo.LastName;
        model.Mobile = guestInfo.Mobile;
        model.Email = guestInfo.Email;
        model.Occupation = guestInfo.Occupation;
        model.GenderText = guestInfo.Gender == "M" ? "Male" : "Female";
        model.Address = guestInfo.Address;
        model.PhotoUrl = guestInfo.PhotoUrl;
        model.VipStatus = guestInfo.IsVip ? "VIP" : "Regular";

        if (bookingInfo != null)
        {
            model.BookingNo = bookingInfo.Booking.BookingNo;
            model.BookingDate = bookingInfo.Booking.BookingDate.ToLongDateString();
            model.CheckInTime = bookingInfo.Booking.CheckInTime.ToLongDateString();
            model.CheckOutTime = bookingInfo.Booking.CheckOutTime.ToLongDateString();
            model.VisitPurpose = bookingInfo.Booking.VisitPurpose;
            model.BookingStatus = bookingInfo.Booking.BookingStatus.ToString();
            model.PaymentStatus = bookingInfo.Booking.PaymentStatus.ToString();
        }

        else
        {
            model.BookingNo = "Not Booked Yet";
            model.BookingDate = "Not Booked Yet";
            model.CheckInTime = "Not Booked Yet";
            model.CheckOutTime = "Not Booked Yet";
            model.VisitPurpose = "Not Booked Yet";
            model.BookingStatus = "Not Booked Yet";
            model.PaymentStatus = "Not Booked Yet";
        }

        return model;
    }
    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion
}