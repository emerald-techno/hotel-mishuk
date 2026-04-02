using Domain.Enums.AppEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Persistence.ContextModel;

namespace Utility.CachingUtility
{
    public class CacheStoreService
    {
        #region Config
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _iMemoryCache;

        public CacheStoreService()
        {
        }

        public CacheStoreService(ApplicationDbContext db, IMemoryCache iMemoryCache)
        {
            _db = db;
            _iMemoryCache = iMemoryCache;
        }

        #endregion

        public void Clear()
        {
            _iMemoryCache.Dispose();
        }

        public T Get<T>(string cacheKey) where T : class
        {
            T returnValue;

            if (!_iMemoryCache.TryGetValue("itemCacheKey", out returnValue))
            {
                return null;
            }

            return (T)returnValue;
        }

        public void Add<T>(string key, T o)
        {
            T cacheEntry;

            if (!_iMemoryCache.TryGetValue(key, out cacheEntry))
            {
                cacheEntry = o;

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(1000),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                };

                _iMemoryCache.Set(key, cacheEntry, cacheExpiryOptions);
            }

        }

        public List<T> GetSession<T>(string cacheKey) where T : class
        {
            var dataList = new List<T>();

            if (!_iMemoryCache.TryGetValue(cacheKey, out dataList))
            {
                dataList = GetDataFormDb<T>(cacheKey);

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(50),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                };

                _iMemoryCache.Set(cacheKey, dataList, cacheExpiryOptions);
            }
            else
            {
                _iMemoryCache.TryGetValue(cacheKey, out dataList);
            }

            return dataList;
        }

        public List<T> GetDataFormDb<T>(string cacheListName) where T : class
        {
            var dataList = new List<T>();

            if (cacheListName == CacheEnum.RoleList.ToString())
            {
                dataList = _db.Roles.OrderBy(c => c.Name).ToList() as List<T>;
            }

            return dataList;
        }

        public List<T> AddOrUpdate<T>(string cacheListName) where T : class
        {
            var dataList = new List<T>();

            if (cacheListName == CacheEnum.DepartmentList.ToString())
            {
                dataList = _db.Departments.OrderByDescending(c => c.Name).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.DesignationList.ToString())
            {
                dataList = _db.Designations.OrderByDescending(c => c.Name).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.EmployeeList.ToString())
            {
                dataList = _db.Employees.OrderByDescending(c => c.Name).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.CountryList.ToString())
            {
                dataList = _db.SetCountries.OrderBy(c => c.Name).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.CurrencyList.ToString())
            {
                dataList = _db.SetCurrencies.OrderByDescending(c => c.CurrencyName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.FinYearList.ToString())
            {
                dataList = _db.SetFincYears.OrderBy(c => c.YearStartDate).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.UserList.ToString())
            {
                dataList = _db.Users.Where(c => !c.IsDeleted).OrderByDescending(c => c.FullName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.BedTypeList.ToString())
            {
                dataList = _db.HtBedTypes.Where(c => !c.IsDeleted).OrderByDescending(c => c.TypeName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.RoomFacilityCategoryList.ToString())
            {
                dataList = _db.HtRoomFacilityCategories.Where(c => !c.IsDeleted).OrderByDescending(c => c.CategoryName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.RoomCategoryList.ToString())
            {
                dataList = _db.HtRoomCategories.Where(c => !c.IsDeleted).OrderByDescending(c => c.CategoryName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.FloorList.ToString())
            {
                dataList = _db.HtFloorInfos.Where(c => !c.IsDeleted).OrderByDescending(c => c.FloorName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.RoomList.ToString())
            {
                dataList = _db.HtRoomInfos.Where(c => !c.IsDeleted).OrderByDescending(c => c.RoomNo).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.DistrictList.ToString())
            {
                dataList = _db.SetDistricts.Where(c => !c.IsDeleted).OrderByDescending(c => c.Name).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.GuestList.ToString())
            {
                dataList = _db.HtGuestInfos.Include(c => c.Company).Where(c => !c.IsDeleted).OrderByDescending(c => c.FirstName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.RoomFacilityList.ToString())
            {
                dataList = _db.HtRoomFacilities.Where(c => !c.IsDeleted).OrderByDescending(c => c.FacilityName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.ComplementaryList.ToString())
            {
                dataList = _db.HtComplementaries.Where(c => !c.IsDeleted).OrderByDescending(c => c.Title).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.TaskTypeList.ToString())
            {
                dataList = _db.HkTaskTypes.Where(c => !c.IsDeleted).OrderByDescending(c => c.TypeName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.EmpLeaveType.ToString())
            {
                dataList = _db.LeaveTypes.OrderByDescending(c => c.TypeName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.AccHeadList.ToString())
            {
                dataList = _db.AccHeads.OrderByDescending(c => c.HeadName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.PrEmpSalaryPartList.ToString())
            {
                dataList = _db.PrEmpSalaryParts.OrderByDescending(c => c.PartType).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.CountryList.ToString())
            {
                dataList = _db.SetCountries.OrderBy(c => c.Name).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.AccGroupList.ToString())
            {
                dataList = _db.AccGroups.OrderByDescending(c => c.Name).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.AccLedgerList.ToString())
            {
                dataList = _db.AccLedgers.OrderByDescending(c => c.LedgerName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.SubAccHeadList.ToString())
            {
                var ledgerHeadIds = _db.AccLedgers.Where(c => !c.IsDeleted).Select(x => x.HeadId).ToList();
                dataList = _db.AccHeads.Where(c => !ledgerHeadIds.Contains(c.Id) && !c.IsDeleted).OrderByDescending(c => c.HeadName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.BankAccLedgerList.ToString())
            {
                var bankHeadIds = _db.AccHeads.Where(c => c.BankHead && !c.IsDeleted).Select(x => x.Id).ToList();
                dataList = _db.AccLedgers.Where(c => bankHeadIds.Contains(c.HeadId) && !c.IsDeleted).OrderByDescending(c => c.LedgerCode).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.CategoryInfoList.ToString())
            {
                dataList = _db.CategoryInfos.ToList().OrderBy(c => c.CategoryName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.UnitInfoList.ToString())
            {
                dataList = _db.UnitInfos.ToList().OrderBy(c => c.UnitName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.ItemInfoList.ToString())
            {
                dataList = _db.ItemInfos.ToList().OrderBy(c => c.ItemName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.RequsitionInfoList.ToString())
            {
                dataList = _db.RequsitionInfos.Include(o => o.Dept)
                                              .Include(x => x.ReqBy).ToList().OrderBy(c => c.ReqDate).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.SupplierInfoList.ToString())
            {
                dataList = _db.SupplierInfos.ToList().OrderBy(c => c.SupplierName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.OrderList.ToString())
            {
                dataList = _db.OrderMsts.ToList().OrderBy(c => c.OrderNo).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.TranList.ToString())
            {
                dataList = _db.TranMsts.ToList().OrderBy(c => c.TranNo).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.ItemCategoryList.ToString())
            {
                dataList = _db.CategoryInfos.ToList().OrderBy(c => c.CategoryName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.OnlineBookingList.ToString())
            {
                dataList = _db.HtOnlineBookings.ToList().OrderBy(c => c.OnlineBookingNumber).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.FoodCategoryList.ToString())
            {
                dataList = _db.RsFoodCategories.ToList().OrderBy(c => c.CategoryName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.CustomerTypeList.ToString())
            {
                dataList = _db.RsCustomerTypes.ToList().OrderBy(c => c.TypeName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.RsCustomerList.ToString())
            {
                dataList = _db.RsCustomers.ToList().OrderBy(c => c.CustomerCode).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.TableList.ToString())
            {
                dataList = _db.RsTables.ToList().OrderBy(c => c.TableNo).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.DutyShiftList.ToString())
            {
                dataList = _db.DutyShifts.ToList().OrderBy(c => c.ShiftName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.HtServiceList.ToString())
            {
                dataList = _db.HtServices.ToList().OrderBy(c => c.ServiceName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.WaiterList.ToString())
            {
                dataList = _db.RsWaiters.ToList().OrderBy(c => c.Name).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.RefundCancelBookingList.ToString())
            {
                var cancelBookingIds = _db.HtBookingPayments.Include(b => b.Booking)
                    .Where(c => c.Booking.BookingStatus == BookingServiceStatusEnum.Canceled && c.PaidAmount > 0 && !c.IsDeleted).Select(x => x.BookingId).ToList();

                dataList = _db.HtBookingServices.Where(c => cancelBookingIds.Contains(c.Id) && !c.IsDeleted).OrderByDescending(c => c.BookingNo).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.FoodItemList.ToString())
            {
                dataList = _db.RsFoodItems.ToList().OrderBy(c => c.ItemCode).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.HallList.ToString())
            {
                dataList = _db.HtHallInfos.ToList().OrderBy(c => c.HallName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.NtfEventList.ToString())
            {
                dataList = _db.NtfEventInfos.ToList().OrderBy(c => c.EventName).ToList() as List<T>;
            }
            else if (cacheListName == CacheEnum.ClientCompanyList.ToString())
            {
                dataList = _db.ClientCompanies.ToList().OrderBy(c => c.Name).ToList() as List<T>;
            }
            return dataList;
        }
    }
}
