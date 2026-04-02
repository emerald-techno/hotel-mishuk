using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.OnlineBooking;
using Interface.Repository.HotelManagement;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class OnlineBookingRepository : BaseRepository<HtOnlineBooking>, IOnlineBookingRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public OnlineBookingRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
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

    public async Task<DataTablePagination<OnlineBookingSearchVm, OnlineBookingSearchVm>>
        SearchAsync(DataTablePagination<OnlineBookingSearchVm, OnlineBookingSearchVm> vm)
    {
        var searchResult = Context.HtOnlineBookings
                .Include(x => x.OnlineBookingDetails)
                    .ThenInclude(x => x.RoomCategory)
                .AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search online booking not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.OnlineBookingNumber.ToLower().Contains(value) || c.GuestMobile.ToLower().Contains(value)
            || c.GuestName.ToLower().Contains(value));
        }

        if (model.IsPendingOnly)
        {
            searchResult = searchResult.Where(c => c.Status == OnlineBookingStatusEnum.Pending && c.ArrivalDate.Date >= DateTime.Now.Date);
        }

        if (!string.IsNullOrEmpty(model.OnlineBookingNumber))
        {
            searchResult = searchResult.Where(c => c.OnlineBookingNumber.Contains(model.OnlineBookingNumber));
        }

        if (!string.IsNullOrEmpty(model.GuestName))
        {
            searchResult = searchResult.Where(c => c.GuestName.Contains(model.GuestName));
        }

        if (!string.IsNullOrEmpty(model.GuestMobile))
        {
            searchResult = searchResult.Where(c => c.GuestMobile.Contains(model.GuestMobile));
        }
        
        if (!string.IsNullOrEmpty(model.Status.ToString()))
        {
            searchResult = searchResult.Where(c => c.Status == model.Status);
        }

        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderByDescending(c => c.OnlineBookingDate)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<OnlineBookingSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;

                var categoryList = filterData.OnlineBookingDetails.Select(x => $"{x.RoomCount} {x.RoomCategory.CategoryName} " +
                $"({x.CheckInDate.ToString("dd/MMM/yyyy")} to {x.CheckOutDate.ToString("dd/MMM/yyyy")})").ToList();
                searchDto.CategoryList = string.Join("<br/>", categoryList);
            }
        }
        return vm;
    }

    #endregion

    #region OnlineBookingPrint

    public async Task<HtOnlineBooking> GetOnlineBookingByIdAsync(long id)
    {
        var data = await Context.HtOnlineBookings
            .Include(b => b.OnlineBookingDetails)
                .ThenInclude(b => b.RoomCategory)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (data == null) throw new Exception("No Food Booking Data Found");

        return data;
    }

    #endregion
}
