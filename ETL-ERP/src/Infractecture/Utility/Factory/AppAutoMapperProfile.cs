using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Entities.Admin;
using Domain.Entities.Attendance;
using Domain.Entities.HotelManagement;
using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.Entities.Inventory;
using Domain.Entities.Leave;
using Domain.Entities.Payroll;
using Domain.Entities.Pf;
using Domain.ViewModel.Accounting.AccGroup;
using Domain.ViewModel.Accounting.AccHead;
using Domain.ViewModel.Accounting.AccLedger;
using Domain.ViewModel.Accounting.AccTranDtl;
using Domain.ViewModel.Accounting.AccTranFiles;
using Domain.ViewModel.Accounting.AccTranMst;
using Domain.ViewModel.Accounting.AccTranNote;
using Domain.ViewModel.Admin.Department;
using Domain.ViewModel.Admin.Designation;
using Domain.ViewModel.Admin.SetCurrency;
using Domain.ViewModel.Admin.SetHoliday;
using Domain.ViewModel.Attendance.EmpAttendence;
using Domain.ViewModel.Attendance.MonthlyAttSheet;
using Domain.ViewModel.HotelManagement.BedType;
using Domain.ViewModel.HotelManagement.Complementary;
using Domain.ViewModel.HotelManagement.FloorInfo;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Domain.ViewModel.HotelManagement.GuestInfo;
using Domain.ViewModel.HotelManagement.RoomCategory;
using Domain.ViewModel.HotelManagement.RoomFacility;
using Domain.ViewModel.HotelManagement.RoomFacilityCategory;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Domain.ViewModel.Hr.EmpDisciplinary;
using Domain.ViewModel.Hr.EmpEducation;
using Domain.ViewModel.Hr.EmpExperience;
using Domain.ViewModel.Hr.EmpJournal;
using Domain.ViewModel.Hr.Employees;
using Domain.ViewModel.Hr.EmpPosting;
using Domain.ViewModel.Hr.EmpReference;
using Domain.ViewModel.Hr.EmpTraining;
using Domain.ViewModel.Hr.HrSetting;
using Domain.ViewModel.Inventory.CategoryInfo;
using Domain.ViewModel.Inventory.ItemInfo;
using Domain.ViewModel.Inventory.Order;
using Domain.ViewModel.Inventory.Receive;
using Domain.ViewModel.Inventory.RequsitionInfo;
using Domain.ViewModel.Inventory.Transaction;
using Domain.ViewModel.Inventory.UnitInfo;
using Domain.ViewModel.Leave.EmpLeaveApplication;
using Domain.ViewModel.Leave.LeaveCf;
using Domain.ViewModel.Leave.LeaveSetup;
using Domain.ViewModel.Leave.LeaveType;
using Domain.ViewModel.Leave.LvAppReviewer;
using Domain.ViewModel.Payroll.PrArrearMst;
using Domain.ViewModel.Payroll.PrEmpSalaryPart;
using Domain.ViewModel.Payroll.PrGuestSalary;
using Domain.ViewModel.Payroll.PrSalaryDtl;
using Domain.ViewModel.Payroll.PrSalaryMst;
using Domain.ViewModel.Payroll.PrSalaryPart;
using Domain.ViewModel.Payroll.SetSalaryGrade;
using Domain.ViewModel.Pf.PfFund;
using Domain.ViewModel.Pf.PfFundOpenning;
using Domain.ViewModel.Pf.PfSetting;
using Domain.ViewModel.Pf.PfSettlement;
using Domain.ViewModel.Role;
using Domain.ViewModel.User;
using Domain.ViewModel.HotelManagement.Booking;
using Domain.ViewModel.HotelManagement.Service;
using Domain.Entities.HouseKeeping;
using Domain.ViewModel.HouseKeeping.TaskType;
using Domain.ViewModel.HouseKeeping.TaskName;
using Domain.ViewModel.HotelManagement.Billing;
using Domain.ViewModel.HouseKeeping.RoomAssign;
using Domain.ViewModel.HouseKeeping.TaskAssign;
using Domain.ViewModel.Accounting.ChartOfAcc;
using Domain.ViewModel.Inventory.SupplierInfo;
using Domain.ViewModel.HotelManagement.OnlineBooking;
using Domain.ViewModel.Website;
using Domain.ViewModel.Restaurant.FoodCategory;
using Domain.ViewModel.Restaurant.FoodItem;
using Domain.ViewModel.Restaurant.FoodOrder;
using Domain.ViewModel.Restaurant.OrderPayment;
using Domain.ViewModel.Restaurant.Customer;
using Domain.ViewModel.Restaurant.Table;
using Domain.ViewModel.Admin.FinancialYear;
using Domain.ViewModel.Attendance.DutyShift;
using Domain.ViewModel.Inventory.BillPayment;
using Domain.ViewModel.Attendance.ShiftManagement;
using Domain.ViewModel.Leave.DptLeaveReviewer;
using Domain.ViewModel.HotelManagement.AdvanceRefund;
using Domain.ViewModel.Restaurant.Waiter;
using Domain.ViewModel.Notification;
using Domain.Entities.Notification;
using Domain.ViewModel.Pf.EmpLoan;
using Domain.Entities.Restaurant;
using Domain.ViewModel.Restaurant.FoodIngredient;
using Domain.ViewModel.Inventory.ItemConversion;
using Domain.ViewModel.HotelManagement.ClientCompany;
using Domain.ViewModel.HotelManagement.RoomDayAudit;
using Domain.ViewModel.HotelManagement.BusinessDaySummary;

namespace Utility.Factory;

public class AppAutoMapperProfile : Profile
{
    public AppAutoMapperProfile()
    {
        #region Config

        CreateMap<ApplicationUser, UserSearchVm>();

        CreateMap<ApplicationRole, RoleSearchVm>();

        CreateMap<Department, DepartmentVm>();
        CreateMap<DepartmentVm, Department>();
        CreateMap<Department, DepartmentSearchVm>();
        CreateMap<DepartmentSearchVm, Department>();

        CreateMap<DesignationVm, Designation>();
        CreateMap<Designation, DesignationVm>();
        CreateMap<Designation, DesignationSearchVm>();

        CreateMap<SetCurrencyVm, SetCurrency>();
        CreateMap<SetCurrency, SetCurrencyVm>();
        CreateMap<SetCurrency, SetCurrencySearchVm>();

        CreateMap<SetFincYearVm, SetFincYear>();
        CreateMap<SetFincYear, SetFincYearVm>();
        CreateMap<SetFincYear, SetFincYearSearchVm>();

        CreateMap<SetHolidayVm, SetHoliday>();
        CreateMap<SetHoliday, SetHolidayVm>();
        CreateMap<SetHoliday, SetHolidaySearchVm>();

        CreateMap<HrSetting, HrSettingVm>();
        CreateMap<HrSettingVm, HrSetting>();

        CreateMap<EmployeeVm, Employee>();
        CreateMap<Employee, EmployeeVm>();
        CreateMap<Employee, EmployeeSearchVm>();

        CreateMap<EmpEducationVm, EmpEducation>();
        CreateMap<EmpEducation, EmpEducationVm>();

        CreateMap<EmpExperienceVm, EmpExperience>();
        CreateMap<EmpExperience, EmpExperienceVm>();

        CreateMap<EmpReferenceVm, EmpReference>();
        CreateMap<EmpReference, EmpReferenceVm>();

        CreateMap<EmpTrainingVm, EmpTraining>();
        CreateMap<EmpTraining, EmpTrainingVm>();

        CreateMap<EmpPostingVm, EmpPosting>();
        CreateMap<EmpPosting, EmpPostingVm>();

        CreateMap<EmpDisciplinaryVm, EmpDisciplinary>();
        CreateMap<EmpDisciplinary, EmpDisciplinaryVm>();

        CreateMap<EmpJournalVm, EmpJournal>();
        CreateMap<EmpJournal, EmpJournalVm>();

        #region Attendance

        CreateMap<EmpAttendanceVm, EmpAttendance>();
        CreateMap<EmpAttendance, EmpAttendanceVm>();
        CreateMap<EmpAttendance, EmpAttendanceSearchVm>();

        CreateMap<MonthlyAttSheetMstVm, MonthlyAttendanceSheetMst>();
        CreateMap<MonthlyAttendanceSheetMst, MonthlyAttSheetMstVm>();
        CreateMap<MonthlyAttendanceSheetMst, MonthlyAttSheetSearchVm>();

        CreateMap<MonthlyAttSheetDtlVm, MonthlyAttendanceSheetDtl>();
        CreateMap<MonthlyAttendanceSheetDtl, MonthlyAttSheetDtlVm>();

        CreateMap<DutyShiftVm, DutyShift>();
        CreateMap<DutyShift, DutyShiftVm>();
        CreateMap<DutyShift, DutyShiftSearchVm>();

        CreateMap<ShiftManagementVm, ShiftManagement>();
        CreateMap<ShiftManagement, ShiftManagementVm>();
        CreateMap<ShiftManagement, ShiftManagementSearchVm>();

        #endregion

        #region Leave

        CreateMap<LeaveTypeVm, LeaveType>();
        CreateMap<LeaveType, LeaveTypeVm>();
        CreateMap<LeaveType, LeaveTypeSearchVm>();

        CreateMap<LeaveCf, LeaveCfVm>();
        CreateMap<LeaveCfVm, LeaveCf>();
        CreateMap<LeaveCf, LeaveCfSearchVm>();

        CreateMap<LeaveSetupVm, LeaveSetup>();
        CreateMap<LeaveSetup, LeaveSetupVm>();
        CreateMap<LeaveSetup, LeaveSetupSearchVm>();

        CreateMap<EmpLeaveApplicationVm, EmpLeaveApplication>();
        CreateMap<EmpLeaveApplication, EmpLeaveApplicationVm>();
        CreateMap<EmpLeaveApplication, EmpLeaveApplicationSearchVm>();

        CreateMap<LvAppReviewer, LvAppReviewerVm>();
        CreateMap<LvAppReviewerVm, LvAppReviewer>();
        CreateMap<LvAppReviewer, LvAppReviewerSearchVm>();

        CreateMap<DptLeaveReviewer, LeaveReviewerVm>();
        CreateMap<EmpLeaveReviewer, LeaveReviewerVm>();

        CreateMap<DptLeaveReviewer, DptLeaveReviewerVm>();
        CreateMap<DptLeaveReviewerVm, DptLeaveReviewer>();

        #endregion

        #region Payroll

        CreateMap<PrSalaryMstVm, PrSalaryMst>();
        CreateMap<PrSalaryMst, PrSalaryMstVm>();
        CreateMap<PrSalaryMst, PrSalaryMstSearchVm>();

        CreateMap<PrSalaryDtlVm, PrSalaryDtl>();
        CreateMap<PrSalaryDtl, PrSalaryDtlVm>();

        CreateMap<PayslipVm, PrSalaryDtl>();
        CreateMap<PrSalaryDtl, PayslipVm>();

        CreateMap<PrSalaryPart, PrSalaryPartSearchVm>();
        CreateMap<PrSalaryPartSearchVm, PrSalaryPart>();
        CreateMap<PrSalaryPartVm, PrSalaryPart>();
        CreateMap<PrSalaryPart, PrSalaryPartVm>();

        CreateMap<PrEmpSalaryPart, PrEmpSalaryPartSearchVm>();
        CreateMap<PrEmpSalaryPartVm, PrEmpSalaryPart>();
        CreateMap<PrEmpSalaryPart, PrEmpSalaryPartVm>();

        CreateMap<PrArrearMstVm, PrArrearMst>();
        CreateMap<PrArrearMst, PrArrearMstVm>();
        CreateMap<PrArrearMst, PrArrearMstSearchVm>();

        CreateMap<PrArrearDtlVm, PrArrearDtl>();
        CreateMap<PrArrearDtl, PrArrearDtlVm>();

        CreateMap<PrGuestSalaryMstVm, PrGuestSalaryMst>();
        CreateMap<PrGuestSalaryMst, PrGuestSalaryMstVm>();
        CreateMap<PrGuestSalaryMst, PrGuestSalaryMstSearchVm>();

        CreateMap<PrGuestSalaryDtlVm, PrGuestSalaryDtl>();
        CreateMap<PrGuestSalaryDtl, PrGuestSalaryDtlVm>();

        CreateMap<SetSalaryGradeVm, SetSalaryGrade>();
        CreateMap<SetSalaryGrade, SetSalaryGradeVm>();
        CreateMap<SetSalaryGrade, SetSalaryGradeSearchVm>();

        #endregion

        #region PF

        CreateMap<PfSetting, PfSettingVm>();
        CreateMap<PfSettingVm, PfSetting>();

        CreateMap<PfFundOpenningVm, PfFundOpenning>();
        CreateMap<PfFundOpenning, PfFundOpenningVm>();
        CreateMap<PfFundOpenning, PfFundOpenningSearchVm>();

        CreateMap<PfFundMst, PfFundMstVm>();
        CreateMap<PfFundMstVm, PfFundMst>();
        CreateMap<PfFundMst, PfFundMstSearchVm>();

        CreateMap<PfFundDtl, PfFundDtlVm>();
        CreateMap<PfFundDtlVm, PfFundDtl>();

        CreateMap<PfSettlement, PfSettlementVm>();
        CreateMap<PfSettlementVm, PfSettlement>();
        CreateMap<PfSettlement, PfSettlementSearchVm>();

        #endregion

        #region Accounting

        CreateMap<AccHead, AccHeadVm>();
        CreateMap<AccHeadVm, AccHead>();
        CreateMap<AccHead, AccHeadSearchVm>();

        CreateMap<AccGroupVm, AccGroup>();
        CreateMap<AccGroup, AccGroupVm>();
        CreateMap<AccGroup, AccGroupSearchVm>();

        CreateMap<AccLedgerVm, AccLedger>();
        CreateMap<AccLedger, AccLedgerVm>();
        CreateMap<AccLedger, AccLedgerSearchVm>();

        CreateMap<AccTranMst, AccTranMstVm>();
        CreateMap<AccTranMstVm, AccTranMst>();
        CreateMap<AccTranMst, AccTranMstSearchVm>();

        CreateMap<AccTranDtlVm, AccTranDtl>();
        CreateMap<AccTranDtl, AccTranDtlVm>();
        CreateMap<AccTranDtl, AccTranDtlSearchVm>();

        CreateMap<AccTranFileVm, AccTranFile>();
        CreateMap<AccTranFile, AccTranFileVm>();

        CreateMap<AccTranNoteVm, AccTranNote>();
        CreateMap<AccTranNote, AccTranNoteVm>();

        CreateMap<JournalMstVm, AccTranMst>();
        CreateMap<AccTranMst, JournalMstVm>();

        CreateMap<AccGroup, ChartAccGroup>();
        CreateMap<ChartAccGroup, AccGroup>();

        CreateMap<AccHead, ChartAccHead>();
        CreateMap<ChartAccHead, AccHead>();

        CreateMap<AccLedger, ChartAccLedger>();
        CreateMap<ChartAccLedger, AccLedger>();

        #endregion

        #region Inventory

        CreateMap<CategoryInfoVm, CategoryInfo>();
        CreateMap<CategoryInfo, CategoryInfoVm>();
        CreateMap<CategoryInfo, CategoryInfoSearchVm>();

        CreateMap<ItemInfoVm, ItemInfo>();
        CreateMap<ItemInfo, ItemInfoVm>();
        CreateMap<ItemInfo, ItemInfoSearchVm>();

        CreateMap<SupplierInfoVm, SupplierInfo>();
        CreateMap<SupplierInfo, SupplierInfoVm>();
        CreateMap<SupplierInfo, SupplierInfoSearchVm>();

        CreateMap<RequsitionInfoVm, RequsitionInfo>();
        CreateMap<RequsitionInfo, RequsitionInfoVm>();
        CreateMap<RequsitionInfo, RequsitionInfoSearchVm>();

        CreateMap<RequsitionInfoDtlVm, RequsitionInfoDtl>();
        CreateMap<RequsitionInfoDtl, RequsitionInfoDtlVm>();

        CreateMap<OrderDtlVm, OrderDtl>();
        CreateMap<OrderDtl, OrderDtlVm>();

        CreateMap<OrderVm, OrderMst>();
        CreateMap<OrderMst, OrderVm>();
        CreateMap<OrderMst, OrderSearchVm>();

        CreateMap<ReceiveDtlVm, TranDtl>();
        CreateMap<TranDtl, ReceiveDtlVm>();

        CreateMap<TransactionDtlVm, TranDtl>();
        CreateMap<TranDtl, TransactionDtlVm>();

        CreateMap<ReceiveVm, TranMst>();
        CreateMap<TranMst, ReceiveVm>();
        CreateMap<TranMst, ReceiveSearchVm>();

        CreateMap<TransactionVm, TranMst>();
        CreateMap<TranMst, TransactionVm>();
        CreateMap<TranMst, TransactionSearchVm>();

        CreateMap<UnitInfoVm, UnitInfo>();
        CreateMap<UnitInfo, UnitInfoVm>();
        CreateMap<UnitInfo, UnitInfoSearchVm>();

        CreateMap<InventoryBillPaymentVm, InventoryBillPayment>();
        CreateMap<InventoryBillPayment, InventoryBillPaymentVm>();
        CreateMap<InventoryBillPayment, InventoryBillPaymentSearchVm>();

        CreateMap<ItemConvertionVm, ItemConvertion>();
        CreateMap<ItemConvertion, ItemConvertionVm>();
        CreateMap<ItemConvertion, ItemConvertionSearchVm>();

        #endregion

        #region Hotel Management

        CreateMap<HtBedType, HtBedTypeVm>();
        CreateMap<HtBedTypeVm, HtBedType>();
        CreateMap<HtBedType, HtBedTypeSearchVm>();

        CreateMap<HtRoomCategory, HtRoomCategoryVm>();
        CreateMap<HtRoomCategoryVm, HtRoomCategory>();
        CreateMap<HtRoomCategory, HtRoomCategorySearchVm>();

        CreateMap<HtFloorInfo, HtFloorInfoVm>();
        CreateMap<HtFloorInfoVm, HtFloorInfo>();
        CreateMap<HtFloorInfo, HtFloorInfoSearchVm>();

        CreateMap<HtRoomFacility, HtRoomFacilityVm>();
        CreateMap<HtRoomFacilityVm, HtRoomFacility>();
        CreateMap<HtRoomFacility, HtRoomFacilitySearchVm>();

        CreateMap<HtRoomFacilityCategory, HtRoomFacilityCategoryVm>();
        CreateMap<HtRoomFacilityCategoryVm, HtRoomFacilityCategory>();
        CreateMap<HtRoomFacilityCategory, HtRoomFacilityCategorySearchVm>();

        CreateMap<HtRoomInfo, HtRoomInfoVm>();
        CreateMap<HtRoomInfoVm, HtRoomInfo>();
        CreateMap<HtRoomInfo, HtRoomInfoSearchVm>();

        CreateMap<HtHallInfo, HtHallInfoVm>();
        CreateMap<HtHallInfoVm, HtHallInfo>();
        CreateMap<HtHallInfo, HtHallInfoSearchVm>();

        CreateMap<HtBookingService, HtBookingServiceVm>();
        CreateMap<HtBookingServiceVm, HtBookingService>();
        CreateMap<HtBookingService, BookingServiceSearchVm>();

        CreateMap<BookingServiceSearchDto, BookingServiceSearchVm>();

        CreateMap<HtBookingGuest, HtBookingGuestVm>();
        CreateMap<HtBookingGuestVm, HtBookingGuest>();

        CreateMap<HtBookingRoom, HtBookingRoomVm>();
        CreateMap<HtBookingRoomVm, HtBookingRoom>();

        CreateMap<HtBookingHall, HtBookingHallVm>();
        CreateMap<HtBookingHallVm, HtBookingHall>();

        CreateMap<HtBookingPayment, HtBookingPaymentVm>();
        CreateMap<HtBookingPaymentVm, HtBookingPayment>();

        CreateMap<HtAdvanceRefund, AdvanceRefundVm>();
        CreateMap<AdvanceRefundVm, HtAdvanceRefund>();

        CreateMap<HtBookingRoomDto, HtBookingRoomVm>();

        CreateMap<HtComplementary, HtComplementaryVm>();
        CreateMap<HtComplementaryVm, HtComplementary>();
        CreateMap<HtComplementary, HtComplementarySearchVm>();

        CreateMap<HtGuestInfo, HtGuestInfoVm>();
        CreateMap<HtGuestInfoVm, HtGuestInfo>();
        CreateMap<HtGuestInfo, HtGuestInfoSearchVm>();

        CreateMap<HtService, ServiceVm>();
        CreateMap<ServiceVm, HtService>();
        CreateMap<HtService, ServiceSearchVm>();

        CreateMap<HtBilling, BillingVm>();
        CreateMap<BillingVm, HtBilling>();
        CreateMap<HtBilling, BillingSearchVm>();

        CreateMap<BillingDetailVm, HtBillingDetail>();
        CreateMap<HtBillingDetail, BillingDetailVm>();

        CreateMap<SaveBillingDetailVm, HtBillingDetail>();
        CreateMap<HtBillingDetail, SaveBillingDetailVm>();

        CreateMap<PayBillVm, HtBookingPayment>();
        CreateMap<BookingPaymentDto, HtBookingPayment>();

        CreateMap<HtOnlineBooking, OnlineBookingVm>();
        CreateMap<OnlineBookingVm, HtOnlineBooking>();
        CreateMap<HtOnlineBooking, OnlineBookingSearchVm>();

        CreateMap<OnlineBookingDetailVm, HtOnlineBookingDetail>();
        CreateMap<HtOnlineBookingDetail, OnlineBookingDetailVm>();

        CreateMap<ClientCompany, ClientCompanyVm>();
        CreateMap<ClientCompanyVm, ClientCompany>();
        CreateMap<ClientCompany, ClientCompanySearchVm>();

        #endregion

        #region House Keeping

        CreateMap<HkTaskType, HkTaskTypeVm>();
        CreateMap<HkTaskTypeVm, HkTaskType>();
        CreateMap<HkTaskType, HkTaskTypeSearchVm>();

        CreateMap<HkTaskName, HkTaskNameVm>();
        CreateMap<HkTaskNameVm, HkTaskName>();
        CreateMap<HkTaskName, HkTaskNameSearchVm>();

        CreateMap<HkRoomAssign, RoomAssignVm>();
        CreateMap<RoomAssignVm, HkRoomAssign>();
        CreateMap<HkRoomAssign, RoomAssignSearchVm>();

        CreateMap<SingleRoomAssignVm, HkRoomAssign>();

        CreateMap<HkTaskAssign, TaskAssignSaveVm>();
        CreateMap<TaskAssignSaveVm, HkTaskAssign>();
        CreateMap<HkTaskAssign, TaskAssignSearchVm>();

        #endregion

        #region Restaurant

        CreateMap<RsFoodCategory, FoodCategoryVm>();
        CreateMap<FoodCategoryVm, RsFoodCategory>();
        CreateMap<RsFoodCategory, FoodCategorySearchVm>();

        CreateMap<RsFoodItem, FoodItemVm>();
        CreateMap<FoodItemVm, RsFoodItem>();
        CreateMap<RsFoodItem, FoodItemSearchVm>();

        CreateMap<FoodOrderVm, RsFoodOrder>();
        CreateMap<RsFoodOrder, FoodOrderVm>();
        CreateMap<RsFoodOrder, FoodOrderSearchVm>();

        CreateMap<FoodOrderItemVm, RsFoodOrderItem>();
        CreateMap<RsFoodOrderItem, FoodOrderItemVm>();

        CreateMap<RsOrderPaymentVm, RsOrderPayments>();
        CreateMap<RsOrderPayments, RsOrderPaymentVm>();

        CreateMap<RsCustomer, CustomerVm>();
        CreateMap<CustomerVm, RsCustomer>();
        CreateMap<RsCustomer, CustomerSearchVm>();

        CreateMap<RsTableVm, RsTable>();
        CreateMap<RsTable, RsTableVm>();
        CreateMap<RsTable, RsTableSearchVm>();

        CreateMap<RsWaiter, WaiterVm>();
        CreateMap<WaiterVm, RsWaiter>();
        CreateMap<RsWaiter, WaiterSearchVm>();

        CreateMap<PayRsOrderVm, RsOrderPayments>();

        CreateMap<SaveFoodOrderItemDto, RsFoodOrderItem>();

        CreateMap<RsFoodIngredient, FoodIngredientVM>();
        CreateMap<FoodIngredientVM, RsFoodIngredient>();

        CreateMap<RsFoodSetItem, FoodSetItemVm>();
        CreateMap<FoodSetItemVm, RsFoodSetItem>();

        #endregion

        #region Website

        CreateMap<MakeReservationVm, HtOnlineBooking>();
        CreateMap<HtOnlineBooking, MakeReservationVm>();

        CreateMap<HtOnlineBooking, CheckOutVm>();

        #endregion

        #region Notification

        CreateMap<NtfNotificationMsgVm, NtfNotificationMsg>();
        CreateMap<NtfNotificationMsg, NtfNotificationMsgVm>();

        CreateMap<NtfUserSettingVm, NtfUserSettings>();
        CreateMap<NtfUserSettings, NtfUserSettingVm>();
        CreateMap<NtfUserSettings, NtfUserSettingSearchVm>();

        #endregion

        #region EmployeeLoan
        CreateMap<EmpLoanMstVm, EmpLoanMst>();
        CreateMap<EmpLoanMst, EmpLoanMstVm>();
        CreateMap<EmpLoanMst, EmpLoanMstSearchVm>();

        CreateMap<EmpLoanDtlVm, EmpLoanDtl>();
        CreateMap<EmpLoanDtl, EmpLoanDtlVm>();
        #endregion

        #region NightAudit

        CreateMap<RoomDayAuditVm, HtRoomDayAudit>();
        CreateMap<HtRoomDayAudit, RoomDayAuditVm>();

        CreateMap<BusinessDaySummaryVm, HtDailyAuditSummary>();
        CreateMap<HtDailyAuditSummary, BusinessDaySummaryVm>();

        #endregion

        #endregion
    }
}
