using Domain.Entities.Accounting;
using Domain.Entities.Admin;
using Domain.Entities.Attendance;
using Domain.Entities.HotelManagement;
using Domain.Entities.HouseKeeping;
using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.Entities.Inventory;
using Domain.Entities.Leave;
using Domain.Entities.Notice;
using Domain.Entities.Notification;
using Domain.Entities.Payroll;
using Domain.Entities.Pf;
using Domain.Entities.Restaurant;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Persistence.ContextModel;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long, IdentityUserClaim<long>, ApplicationUserRole, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
{
    #region Config
    public long CurrentUserId { get; set; }
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    #endregion

    #region Db_Entity

    #region Admin

    public DbSet<Department> Departments { get; set; }
    public DbSet<Designation> Designations { get; set; }
    public DbSet<SetCountry> SetCountries { get; set; }
    public DbSet<SetCurrency> SetCurrencies { get; set; }
    public DbSet<SetDistrict> SetDistricts { get; set; }
    public DbSet<SetDivision> SetDivisions { get; set; }
    public DbSet<SetFincYear> SetFincYears { get; set; }
    public DbSet<SetHoliday> SetHolidays { get; set; }
    public DbSet<SetPoliceStation> SetPoliceStations { get; set; }
    public DbSet<SetShift> SetShifts { get; set; }

    #endregion

    #region Hr
    public DbSet<EmpDisciplinary> EmpDisciplinaries { get; set; }
    public DbSet<EmpEducation> EmpEducations { get; set; }
    public DbSet<EmpExperience> EmpExperiences { get; set; }
    public DbSet<EmpJournal> EmpJournals { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmpPosting> EmpPostings { get; set; }
    public DbSet<EmpReference> EmpReferences { get; set; }
    public DbSet<EmpTraining> EmpTraining { get; set; }
    public DbSet<HrSetting> HrSettings { get; set; }

    #endregion

    #region Attendance
    public DbSet<DutyShift> DutyShifts { get; set; }
    public DbSet<ShiftManagement> ShiftManagements { get; set; }
    public DbSet<EmpAttendance> EmpAttendances { get; set; }

    #endregion

    #region Leave

    public DbSet<LeaveType> LeaveTypes { get; set; }
    public DbSet<LeaveSetup> LeaveSetups { get; set; }
    public DbSet<LeaveCf> LeaveCfs { get; set; }
    public DbSet<EmpLeaveApplication> EmpLeaveApplications { get; set; }
    public DbSet<DptLeaveReviewer> DptLeaveReviewers { get; set; }
    public DbSet<EmpLeaveReviewer> EmpLeaveReviewers { get; set; }
    public DbSet<LvAppReviewer> LvAppReviewers { get; set; }
    public DbSet<LvTransferHst> LvTransferHsts { get; set; }

    #endregion

    #region Inventory

    public DbSet<CategoryInfo> CategoryInfos { get; set; }
    public DbSet<ItemInfo> ItemInfos { get; set; }
    public DbSet<NoteInfo> NoteInfos { get; set; }
    public DbSet<OrderDtl> OrderDtls { get; set; }
    public DbSet<OrderMst> OrderMsts { get; set; }
    public DbSet<RequsitionInfo> RequsitionInfos { get; set; }
    public DbSet<RequsitionInfoDtl> RequsitionInfoDtls { get; set; }
    public DbSet<SupplierInfo> SupplierInfos { get; set; }
    public DbSet<TranDtl> TranDtls { get; set; }
    public DbSet<TranMst> TranMsts { get; set; }
    public DbSet<UnitInfo> UnitInfos { get; set; }
    public DbSet<InventoryBillPayment> InventoryBillPayments { get; set; }
    public DbSet<ItemConvertion> ItemConvertions { get; set; }

    #endregion

    #region Payroll

    public DbSet<SetSalaryGrade> SetSalaryGrades { get; set; }
    public DbSet<MonthlyAttendanceSheetMst> MonthlyAttendanceSheetMsts { get; set; }
    public DbSet<MonthlyAttendanceSheetDtl> MonthlyAttendanceSheetDtls { get; set; }
    public DbSet<PrSalaryPart> PrSalaryParts { get; set; }
    public DbSet<PrEmpSalaryPart> PrEmpSalaryParts { get; set; }
    public DbSet<PrSalaryMst> PrSalaryMsts { get; set; }
    public DbSet<PrSalaryDtl> PrSalaryDtls { get; set; }
    public DbSet<PrArrearMst> PrArrearMsts { get; set; }
    public DbSet<PrArrearDtl> PrArrearDtls { get; set; }
    public DbSet<PrGuestSalaryMst> PrGuestSalaryMsts { get; set; }
    public DbSet<PrGuestSalaryDtl> PrGuestSalaryDtls { get; set; }

    #endregion

    #region PF

    public DbSet<PfSetting> PfSettings { get; set; }
    public DbSet<PfBoardMember> PfBoardMembers { get; set; }
    public DbSet<PfFundOpenning> PfFundOpennings { get; set; }
    public DbSet<PfFundMst> PfFundMsts { get; set; }
    public DbSet<PfFundDtl> PfFundDtls { get; set; }
    public DbSet<PfSettlement> PfSettlements { get; set; }
    public DbSet<EmpLoanMst> EmpLoanMsts { get; set; }
    public DbSet<EmpLoanDtl> EmpLoanDtls { get; set; }

    #endregion

    #region Accounting
    public DbSet<AccGroup> AccGroups { get; set; }
    public DbSet<AccHead> AccHeads { get; set; }
    public DbSet<AccLedger> AccLedgers { get; set; }
    public DbSet<AccTranMst> AccTranMsts { get; set; }
    public DbSet<AccTranDtl> AccTranDtls { get; set; }
    public DbSet<AccTranNote> AccTranNotes { get; set; }
    public DbSet<AccTranFile> AccTranFiles { get; set; }
    public DbSet<AccReviewer> AccReviewers { get; set; }

    #endregion

    #region Notice

    public DbSet<NoticeInfo> NoticeInfos { get; set; }

    #endregion

    #region Notification

    public DbSet<NtfEventInfo> NtfEventInfos { get; set; }
    public DbSet<NtfUserSettings> NtfUserSettings { get; set; }
    public DbSet<NtfNotificationMsg> NtfNotificationMsgs { get; set; }

    #endregion

    #region HotelManagement

    public DbSet<HtBedType> HtBedTypes { get; set; }
    public DbSet<HtBookingType> HtBookingTypes { get; set; }
    public DbSet<HtRoomCategory> HtRoomCategories { get; set; }
    public DbSet<HtFloorInfo> HtFloorInfos { get; set; }
    public DbSet<HtRoomFacilityCategory> HtRoomFacilityCategories { get; set; }
    public DbSet<HtRoomFacility> HtRoomFacilities { get; set; }
    public DbSet<HtRoomInfo> HtRoomInfos { get; set; }
    public DbSet<HtRoomFacilityMap> HtRoomFacilityMaps { get; set; }
    public DbSet<HtGuestInfo> HtGuestInfos { get; set; }
    public DbSet<HtComplementary> HtComplementaries { get; set; }
    public DbSet<HtBookingService> HtBookingServices { get; set; }
    public DbSet<HtBookingGuest> HtBookingGuests { get; set; }
    public DbSet<HtBookingRoom> HtBookingRooms { get; set; }
    public DbSet<HtBookingPayment> HtBookingPayments { get; set; }
    public DbSet<HtAdvanceRefund> HtAdvanceRefunds { get; set; }
    public DbSet<HtBilling> HtBillings { get; set; }
    public DbSet<HtBillingDetail> HtBillingDetails { get; set; }
    public DbSet<HtService> HtServices { get; set; }
    public DbSet<HtOnlineBooking> HtOnlineBookings { get; set; }
    public DbSet<HtOnlineBookingDetail> HtOnlineBookingDetails { get; set; }
    public DbSet<HtHallInfo> HtHallInfos { get; set; }
    public DbSet<HtBookingHall> HtBookingHalls { get; set; }
    public DbSet<HtDateBreakfast> HtDateBreakfasts { get; set; }
    public DbSet<HtRoomStatusHistory> HtRoomStatusHistories { get; set; }
    public DbSet<ClientCompany> ClientCompanies { get; set; }
    public DbSet<HtRoomCategoryDiscountMap> HtRoomCategoryDiscountMaps { get; set; }
    public DbSet<HtBusinessDay> HtBusinessDays { get; set; }
    public DbSet<HtRoomDayAudit> HtRoomDayAudits { get; set; }
    public DbSet<HtDailyAuditSummary> HtDailyAuditSummaries { get; set; }

    #endregion

    #region House Keeping
    public DbSet<HkRoomAssign> HkRoomAssigns { get; set; }
    public DbSet<HkTaskAssign> HkTaskAssigns { get; set; }
    public DbSet<HkTaskName> HkTaskNames { get; set; }
    public DbSet<HkTaskRate> HkTaskRates { get; set; }
    public DbSet<HkTaskType> HkTaskTypes { get; set; }
    #endregion

    #region Restaurant

    public DbSet<RsCustomerType> RsCustomerTypes { get; set; }
    public DbSet<RsFoodType> RsFoodTypes { get; set; }
    public DbSet<RsFoodCategory> RsFoodCategories { get; set; }
    public DbSet<RsFoodItem> RsFoodItems { get; set; }
    public DbSet<RsItemTypeMap> RsItemTypeMaps { get; set; }
    public DbSet<RsItemAvailability> RsItemAvailabilities { get; set; }
    public DbSet<RsTable> RsTables { get; set; }
    public DbSet<RsCustomer> RsCustomers { get; set; }
    public DbSet<RsFoodOrder> RsFoodOrders { get; set; }
    public DbSet<RsFoodOrderItem> RsFoodOrderItems { get; set; }
    public DbSet<RsOrderPayments> RsOrderPayments { get; set; }
    public DbSet<RsWaiter> RsWaiters { get; set; }
    public DbSet<RsFoodIngredient> RsFoodIngredients  { get; set; }
    public DbSet<RsFoodIngredientsHst> RsFoodIngredientsHsts { get; set; }
    public DbSet<RsFoodSetItem> RsFoodSetItems { get; set; }

    #endregion

    #endregion

    public IDbConnection Connection => Database.GetDbConnection();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUserRole>(userRole =>
        {
            userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

            userRole.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            userRole.HasOne(ur => ur.User)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        });

        modelBuilder.Entity<RequsitionInfoDtl>()
                .HasOne<RequsitionInfo>(s => s.Req)
                .WithMany(g => g.RequsitionInfoDtls)
                .HasForeignKey(s => s.ReqId);

        modelBuilder.Entity<OrderDtl>()
            .HasOne<OrderMst>(s => s.Order)
            .WithMany(g => g.OrderDtls)
            .HasForeignKey(s => s.OrderId);

        modelBuilder.Entity<OrderDtl>()
                .HasOne<OrderMst>(s => s.LastOrder)
                .WithMany()
                .HasForeignKey(s => s.LastOrderId);

        modelBuilder.Entity<HtRoomInfo>()
            .HasIndex(b => b.RoomNo)
            .IsUnique();

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        //AllDefaultValueSeedData.SetConfig(modelBuilder);
        //modelBuilder.HasDefaultSchema("dbo");
    }


}
