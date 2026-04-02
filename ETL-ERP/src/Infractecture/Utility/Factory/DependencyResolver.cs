using Domain.Utility;
using Interface.Repository.Accounts;
using Interface.Repository.Admin;
using Interface.Repository.Attendance;
using Interface.Repository.Common;
using Interface.Repository.HotelManagement;
using Interface.Repository.HouseKeeping;
using Interface.Repository.Hr;
using Interface.Repository.Inventory;
using Interface.Repository.Leave;
using Interface.Repository.Notification;
using Interface.Repository.Payroll;
using Interface.Repository.Pf;
using Interface.Repository.Restaurant;
using Interface.Services;
using Interface.Services.Accounts;
using Interface.Services.Admin;
using Interface.Services.Attendance;
using Interface.Services.HotelManagement;
using Interface.Services.HouseKeeping;
using Interface.Services.Hr;
using Interface.Services.Inventory;
using Interface.Services.Leave;
using Interface.Services.Notification;
using Interface.Services.Payroll;
using Interface.Services.Pf;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Persistence.DapperModel;
using Repository;
using Repository.Accounts;
using Repository.Admin;
using Repository.Attendance;
using Repository.HotelManagement;
using Repository.HouseKeeping;
using Repository.Hr;
using Repository.Inventory;
using Repository.Leave;
using Repository.Notification;
using Repository.Payroll;
using Repository.Pf;
using Repository.Restaurant;
using Repository.UnitOfWork;
using Services;
using Services.Accounts;
using Services.Admin;
using Services.Attendance;
using Services.HotelManagement;
using Services.HouseKeeping;
using Services.Hr;
using Services.Inventory;
using Services.Leave;
using Services.Notification;
using Services.Payroll;
using Services.Pf;
using Services.Restaurant;
using Utility.CachingUtility;

namespace Utility.Factory;

public class DependencyResolver
{
    public void SetDependencyConfiguration(IServiceCollection services)
    {
        //var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new AppAutoMapperProfile()); });
        //services.AddSingleton(mappingConfig.CreateMapper());

        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IApplicationWriteDbConnection, ApplicationWriteDbConnection>();
        services.AddScoped<IApplicationReadDbConnection, ApplicationReadDbConnection>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        services.AddTransient<IApplicationUserService, ApplicationUserService>();
        services.AddTransient<IApplicationRoleService, ApplicationRoleService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddTransient<DropdownService, DropdownService>();
        services.AddTransient<CacheStoreService, CacheStoreService>();

        services.AddTransient<IDepartmentService, DepartmentService>();
        services.AddTransient<IDepartmentRepository, DepartmentRepository>();

        services.AddTransient<IDesignationService, DesignationService>();
        services.AddTransient<IDesignationRepository, DesignationRepository>();

        services.AddTransient<ISetCurrencyService, SetCurrencyService>();
        services.AddTransient<ISetCurrencyRepository, SetCurrencyRepository>();

        services.AddTransient<ISetHolidayService, SetHolidayService>();
        services.AddTransient<ISetHolidayRepository, SetHolidayRepository>();

        services.AddTransient<ISetFincYearService, SetFincYearService>();
        services.AddTransient<ISetFincYearRepository, SetFincYearRepository>();

        services.AddTransient<ISetCountryService, SetCountryService>();
        services.AddTransient<ISetCountryRepository, SetCountryRepository>();

        services.AddTransient<IEmployeeService, EmployeeService>();
        services.AddTransient<IEmployeeRepository, EmployeeRepository>();

        services.AddTransient<IEmpEducationService, EmpEducationService>();
        services.AddTransient<IEmpEducationRepository, EmpEducationRepository>();

        services.AddTransient<IEmpExperienceService, EmpExperienceService>();
        services.AddTransient<IEmpExperienceRepository, EmpExperienceRepository>();

        services.AddTransient<IEmpReferenceService, EmpReferenceService>();
        services.AddTransient<IEmpReferenceRepository, EmpReferenceRepository>();

        services.AddTransient<IEmpTrainingService, EmpTrainingService>();
        services.AddTransient<IEmpTrainingRepository, EmpTrainingRepository>();

        services.AddTransient<IEmpPostingService, EmpPostingService>();
        services.AddTransient<IEmpPostingRepository, EmpPostingRepository>();

        services.AddTransient<IEmpDisciplinaryRepository, EmpDisciplinaryRepository>();
        services.AddTransient<IEmpDisciplinaryService, EmpDisciplinaryService>();

        services.AddTransient<IEmpJournalService, EmpJournalService>();
        services.AddTransient<IEmpJournalRepository, EmpJournalRepository>();

        services.AddTransient<IHrSettingService, HrSettingService>();
        services.AddTransient<IHrSettingRepository, HrSettingRepository>();

        services.AddTransient<IAutoCodeRepository, AutoCodeRepository>();

        services.AddTransient<IEmpAttendanceService, EmpAttendanceService>();
        services.AddTransient<IEmpAttendanceRepository, EmpAttendanceRepository>();

        services.AddTransient<IMonthlyAttSheetMstService, MonthlyAttSheetMstService>();
        services.AddTransient<IMonthlyAttSheetMstRepository, MonthlyAttSheetMstRepository>();

        services.AddTransient<IMonthlyAttSheetDtlRepository, MonthlyAttSheetDtlRepository>();

        services.AddTransient<ILeaveTypeService, LeaveTypeService>();
        services.AddTransient<ILeaveTypeRepository, LeaveTypeRepository>();

        services.AddTransient<ILeaveSetupService, LeaveSetupService>();
        services.AddTransient<ILeaveSetupRepository, LeaveSetupRepository>();

        services.AddTransient<ILeaveCfRepository, LeaveCfRepository>();
        services.AddTransient<ILeaveCfService, LeaveCfService>();

        services.AddTransient<IEmpLeaveApplicationService, EmpLeaveApplicationService>();
        services.AddTransient<IEmpLeaveApplicationRepository, EmpLeaveApplicationRepository>();

        services.AddTransient<IEmpLeaveReviewerRepository, EmpLeaveReviewerRepository>();
        services.AddTransient<IEmpLeaveReviewerService, EmpLeaveReviewerService>();

        services.AddTransient<IDptLeaveReviewerRepository, DptLeaveReviewerRepository>();
        services.AddTransient<IDptLeaveReviewerService, DptLeaveReviewerService>();

        services.AddTransient<IMonthlyAttSheetMstRepository, MonthlyAttSheetMstRepository>();
        services.AddTransient<IMonthlyAttSheetMstService, MonthlyAttSheetMstService>();

        services.AddTransient<IPrArrearMstRepository, PrArrearMstRepository>();
        services.AddTransient<IPrArrearMstService, PrArrearMstService>();

        services.AddTransient<IPrArrearDtlRepository, PrArrearDtlRepository>();

        services.AddTransient<IPrEmpSalaryPartRepository, PrEmpSalaryPartRepository>();
        services.AddTransient<IPrEmpSalaryPartService, PrEmpSalaryPartService>();

        services.AddTransient<IPrGuestSalaryMstRepository, PrGuestSalaryMstRepository>();
        services.AddTransient<IPrGuestSalaryMstService, PrGuestSalaryMstService>();

        services.AddTransient<IPrSalaryDtlRepository, PrSalaryDtlRepository>();
        services.AddTransient<IPrSalaryDtlService, PrSalaryDtlService>();

        services.AddTransient<IPrSalaryPartRepository, PrSalaryPartRepository>();
        services.AddTransient<IPrSalaryPartService, PrSalaryPartService>();

        services.AddTransient<ISetSalaryGradeRepository, SetSalaryGradeRepository>();
        services.AddTransient<ISetSalaryGradeService, SetSalaryGradeService>();

        services.AddTransient<ILvAppReviewerRepository, LvAppReviewerRepository>();
        services.AddTransient<ILvAppReviewerService, LvAppReviewerService>();

        services.AddTransient<ICategoryInfoRepository, CategoryInfoRepository>();

        services.AddTransient<IPrSalaryPartService, PrSalaryPartService>();
        services.AddTransient<IPrSalaryPartRepository, PrSalaryPartRepository>();

        services.AddTransient<IPrEmpSalaryPartService, PrEmpSalaryPartService>();
        services.AddTransient<IPrEmpSalaryPartRepository, PrEmpSalaryPartRepository>();

        services.AddTransient<IPrSalaryMstService, PrSalaryMstService>();
        services.AddTransient<IPrSalaryMstRepository, PrSalaryMstRepository>();

        services.AddTransient<IPrSalaryDtlService, PrSalaryDtlService>();
        services.AddTransient<IPrSalaryDtlRepository, PrSalaryDtlRepository>();

        services.AddTransient<IPrArrearMstService, PrArrearMstService>();
        services.AddTransient<IPrArrearMstRepository, PrArrearMstRepository>();

        services.AddTransient<IPrArrearDtlRepository, PrArrearDtlRepository>();

        services.AddTransient<IPrGuestSalaryMstService, PrGuestSalaryMstService>();
        services.AddTransient<IPrGuestSalaryMstRepository, PrGuestSalaryMstRepository>();

        services.AddTransient<IEmpLoanMstService, EmpLoanMstService>();
        services.AddTransient<IEmpLoanMstRepository, EmpLoanMstRepository>();

        services.AddTransient<IEmpLoanDtlService, EmpLoanDtlService>();
        services.AddTransient<IEmpLoanDtlRepository, EmpLoanDtlRepository>();

        services.AddTransient<ISetSalaryGradeService, SetSalaryGradeService>();
        services.AddTransient<ISetSalaryGradeRepository, SetSalaryGradeRepository>();

        services.AddTransient<IPfSettingService, PfSettingService>();
        services.AddTransient<IPfSettingRepository, PfSettingRepository>();

        services.AddTransient<IPfFundOpenningService, PfFundOpenningService>();
        services.AddTransient<IPfFundOpenningRepository, PfFundOpenningRepository>();

        services.AddTransient<IPfSettingService, PfSettingService>();
        services.AddTransient<IPfSettingRepository, PfSettingRepository>();

        services.AddTransient<IPfFundMstService, PfFundMstService>();
        services.AddTransient<IPfFundMstRepository, PfFundMstRepository>();

        services.AddTransient<IPfFundDtlRepository, PfFundDtlRepository>();

        services.AddTransient<IPfSettlementRepository, PfSettlementRepository>();
        services.AddTransient<IPfSettlementService, PfSettlementService>();

        services.AddTransient<IAccountReportRepository, AccountReportRepository>();
        services.AddTransient<IPfReportRepository, PfReportRepository>();

        services.AddTransient<IAccHeadService, AccHeadService>();
        services.AddTransient<IAccHeadRepository, AccHeadRepository>();

        services.AddTransient<IAccGroupRepository, AccGroupRepository>();
        services.AddTransient<IAccGroupService, AccGroupService>();

        services.AddTransient<IAccLedgerService, AccLedgerService>();
        services.AddTransient<IAccLedgerRepository, AccLedgerRepository>();

        services.AddTransient<IAccTranMstService, AccTranMstService>();
        services.AddTransient<IAccTranMstRepository, AccTranMstRepository>();

        services.AddTransient<IAccTranDtlService, AccTranDtlService>();
        services.AddTransient<IAccTranDtlRepository, AccTranDtlRepository>();

        services.AddTransient<IAccTranFileService, AccTranFileService>();
        services.AddTransient<IAccTranFileRepository, AccTranFileRepository>();

        services.AddTransient<IAccTranNoteService, AccTranNoteService>();
        services.AddTransient<IAccTranNoteRepository, AccTranNoteRepository>();

        services.AddTransient<ICategoryInfoService, CategoryInfoService>();
        services.AddTransient<ICategoryInfoRepository, CategoryInfoRepository>();

        services.AddTransient<IItemInfoService, ItemInfoService>();
        services.AddTransient<IItemInfoRepository, ItemInfoRepository>();

        services.AddTransient<IOrderDtlService, OrderDtlService>();
        services.AddTransient<IOrderDtlRepository, OrderDtlRepository>();

        services.AddTransient<IOrderRepository, OrderRepository>();
        services.AddTransient<IOrderService, OrderService>();

        services.AddTransient<IRequsitionInfoDtlService, RequsitionInfoDtlService>();
        services.AddTransient<IRequsitionInfoDtlRepository, RequsitionInfoDtlRepository>();

        services.AddTransient<IRequsitionInfoService, RequsitionInfoService>();
        services.AddTransient<IRequsitionInfoRepository, RequsitionInfoRepository>();

        services.AddTransient<ISupplierInfoService, SupplierInfoService>();
        services.AddTransient<ISupplierInfoRepository, SupplierInfoRepository>();

        services.AddTransient<ITranService, TranService>();
        services.AddTransient<ITranRepository, TranRepository>();

        services.AddTransient<ITranDtlRepository, TranDtlRepository>();

        services.AddTransient<IUnitInfoService, UnitInfoService>();
        services.AddTransient<IUnitInfoRepository, UnitInfoRepository>();

        services.AddTransient<IInventoryReportRepository, InventoryReportRepository>();

        services.AddTransient<IRoomCategoryService, RoomCategoryService>();
        services.AddTransient<IRoomCategoryRepository, RoomCategoryRepository>();

        services.AddTransient<IRoomCategoryDiscountMapService, RoomCategoryDiscountMapService>();
        services.AddTransient<IRoomCategoryDiscountMapRepository, RoomCategoryDiscountMapRepository>();

        services.AddTransient<IFloorInfoService, FloorInfoService>();
        services.AddTransient<IFloorInfoRepository, FloorInfoRepository>();

        services.AddTransient<IBedTypeService, BedTypeService>();
        services.AddTransient<IBedTypeRepository, BedTypeRepository>();

        services.AddTransient<IRoomFacilityService, RoomFacilityService>();
        services.AddTransient<IRoomFacilityRepository, RoomFacilityRepository>();

        services.AddTransient<IRoomFacilityCategoryService, RoomFacilityCategoryService>();
        services.AddTransient<IRoomFacilityCategoryRepository, RoomFacilityCategoryRepository>();

        services.AddTransient<IRoomInfoService, RoomInfoService>();
        services.AddTransient<IRoomInfoRepository, RoomInfoRepository>();

        services.AddTransient<IHallInfoService, HallInfoService>();
        services.AddTransient<IHallInfoRepository, HallInfoRepository>();

        services.AddTransient<IBookingServiceService, BookingServiceService>();
        services.AddTransient<IBookingServiceRepository, BookingServiceRepository>();

        services.AddTransient<IBookingRoomRepository, BookingRoomRepository>();

        services.AddTransient<IBookingHallRepository, BookingHallRepository>();

        services.AddTransient<IBookingGuestRepository, BookingGuestRepository>();

        services.AddTransient<IBookingPaymentRepository, BookingPaymentRepository>();

        services.AddTransient<IAdvanceRefundService, AdvanceRefundService>();
        services.AddTransient<IAdvanceRefundRepository, AdvanceRefundRepository>();

        services.AddTransient<IComplementaryService, ComplementaryService>();
        services.AddTransient<IComplementaryRepository, ComplementaryRepository>();

        services.AddTransient<IGuestInfoService, GuestInfoService>();
        services.AddTransient<IGuestInfoRepository, GuestInfoRepository>();

        services.AddTransient<IRoomFacilityMapRepository, RoomFacilityMapRepository>();

        services.AddTransient<IHtServiceService, HtServiceService>();
        services.AddTransient<IServiceRepository, ServiceRepository>();

        services.AddTransient<ITaskTypeService, TaskTypeService>();
        services.AddTransient<ITaskTypeRepository, TaskTypeRepository>();

        services.AddTransient<ITaskNameService, TaskNameService>();
        services.AddTransient<ITaskNameRepository, TaskNameRepository>();

        services.AddTransient<ITaskRateService, TaskRateService>();
        services.AddTransient<ITaskRateRepository, TaskRateRepository>();

        services.AddTransient<IBillService, BillService>();
        services.AddTransient<IBillingRepository, BillingRepository>();

        services.AddTransient<IRoomAssignService, RoomAssignService>();
        services.AddTransient<IRoomAssignRepository, RoomAssignRepository>();

        services.AddTransient<ITaskAssignService, TaskAssignService>();
        services.AddTransient<ITaskAssignRepository, TaskAssignRepository>();
        services.AddTransient<IHotelManagementRepository, HotelManagementRepository>();

        services.AddTransient<IInventoryReportService, InventoryReportService>();

        services.AddTransient<IOnlineBookingService, OnlineBookingService>();
        services.AddTransient<IOnlineBookingRepository, OnlineBookingRepository>();

        services.AddTransient<IDutyShiftService, DutyShiftService>();
        services.AddTransient<IDutyShiftRepository, DutyShiftRepository>();

        services.AddTransient<IBillingDetailRepository, BillingDetailRepository>();

        services.AddTransient<IInventoryBillPaymentRepository, InventoryBillPaymentRepository>();
        services.AddTransient<IInventoryBillPaymentService, InventoryBillPaymentService>();

        services.AddTransient<IShiftManagementRepository, ShiftManagementRepository>();
        services.AddTransient<IShiftManagementService, ShiftManagementService>();

        services.AddTransient<IItemConvertionRepository, ItemConvertionRepository>();
        services.AddTransient<IItemConvertionService, ItemConvertionService>();

        services.AddTransient<IClientCompanyRepository, ClientCompanyRepository>();
        services.AddTransient<IClientCompanyService, ClientCompanyService>();

        #region Restaurant

        services.AddTransient<IFoodCategoryService, FoodCategoryService>();
        services.AddTransient<IFoodCategoryRepository, FoodCategoryRepository>();

        services.AddTransient<IFoodItemService, FoodItemService>();
        services.AddTransient<IFoodItemRepository, FoodItemRepository>();

        services.AddTransient<IFoodOrderService, FoodOrderService>();
        services.AddTransient<IFoodOrderRepository, FoodOrderRepository>();

        services.AddTransient<IFoodOrderItemRepository, FoodOrderItemRepository>();

        services.AddTransient<ICustomerTypeRepository, CustomerTypeRepository>();

        services.AddTransient<ICustomerService, CustomerService>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();

        services.AddTransient<IRsOrderPaymentRepository, RsOrderPaymentRepository>();

        services.AddTransient<ITableService, TableService>();
        services.AddTransient<ITableRepository, TableRepository>();

        services.AddTransient<IWaiterRepository, WaiterRepository>();
        services.AddTransient<IWaiterService, WaiterService>();

        services.AddTransient<IFoodIngredientRepository, FoodIngredientRepository>();
        services.AddTransient<IFoodIngredientService, FoodIngredientService>();

        #endregion

        services.AddTransient<IDateBreakfastRepository, DateBreakfastRepository>();
        services.AddTransient<IDateBreakfastService, DateBreakfastService>();

        #region Notification

        services.AddTransient<INtfEventInfoRepository, NtfEventInfoRepository>();

        services.AddTransient<INtfUserSettingsRepository, NtfUserSettingsRepository>();
        services.AddTransient<INtfUserSettingsService, NtfUserSettingsService>();

        services.AddTransient<INtfNotificationMsgRepository, NtfNotificationMsgRepository>();
        services.AddTransient<INtfNotificationMsgService, NtfNotificationMsgService>();

        #endregion

        services.AddTransient<IRoomStatusHistoryRepository, RoomStatusHistoryRepository>();
        services.AddTransient<IFoodSetItemRepository, FoodSetItemRepository>();

        //EmailSender
        services.AddTransient<IAppEmailSender, AppEmailSender>();

        //UrlHelper
        services.AddTransient<IUrlHelperService, UrlHelperService>();

        services.AddTransient<IAutoVoucherService, AutoVoucherService>();

        #region NightAudit

        services.AddTransient<IBusinessDayRepository, BusinessDayRepository>();

        services.AddTransient<IRoomDayAuditService, RoomDayAuditService>();
        services.AddTransient<IRoomDayAuditRepository, RoomDayAuditRepository>();
        
        services.AddTransient<IDailyAuditSummaryRepository, DailyAuditSummaryRepository>();

        #endregion
    }
}