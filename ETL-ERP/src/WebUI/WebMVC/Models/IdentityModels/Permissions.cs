namespace WebMVC.Models.IdentityModels
{
    public static class Permissions
    {
        public const string SuperAdmin = "Permissions.SuperAdmin";

        public static class Module
        {
            public const string AdminModule = "Permissions.Module.AdminModule";
            public const string HrModule = "Permissions.Module.HrModule";
            public const string LeaveModule = "Permissions.Module.LeaveModule";
            public const string PayrollModule = "Permissions.Module.PayrollModule";
            public const string PfModule = "Permissions.Module.PfModule";
            public const string FeeModule = "Permissions.Module.FeeModule";
            public const string AcademicModule = "Permissions.Module.AcademicModule";
            public const string AccountsModule = "Permissions.Module.AccountsModule";
            public const string InventoryModule = "Permissions.Module.InventoryModule";
            public const string HotelManagementModule = "Permissions.Module.HotelManagementModule";
            public const string HouseKeepingModule = "Permissions.Module.HouseKeepingModule";
            public const string RestaurantModule = "Permissions.Module.RestaurantModule";
            public const string MisModule = "Permissions.Module.MisModule";
        }

        public static class Users
        {
            public const string ListView = "Permissions.Users.ListView";
            //public const string DetailsView = "Permissions.Users.DetailsView";
            public const string Create = "Permissions.Users.Create";
            //public const string Edit = "Permissions.Users.Edit";
            //public const string Delete = "Permissions.Users.Delete";
            //public const string ResetPassword = "Permissions.Users.ResetPassword";
            //public const string ActiveInactive = "Permissions.Users.ActiveInactive";
        }

        public static class HotelManagement
        {
            public const string Dasboard = "Permissions.HotelManagement.Dasboard";
            public static class Setup
            {
                public const string View = "Permissions.HotelManagement.Setup.View";
                public const string BedType = "Permissions.HotelManagement.Setup.BedType";
                public const string FloorList = "Permissions.HotelManagement.Setup.FloorList";
                public const string RoomFacilityCategory = "Permissions.HotelManagement.Setup.RoomFacilityCategory";
                public const string RoomFacilitiesList = "Permissions.HotelManagement.Setup.RoomFacilitiesList";
                public const string RoomCategoryList = "Permissions.HotelManagement.Setup.RoomCategoryList";
                public const string HallList = "Permissions.HotelManagement.Setup.HallList";
                public const string ClientCompanyList = "Permissions.HotelManagement.Setup.ClientCompanyList";
            }

            public const string RoomList = "Permissions.HotelManagement.RoomList";
            public const string GuestList = "Permissions.HotelManagement.GuestList";
            public const string ComplementaryList = "Permissions.HotelManagement.ComplementaryList";
            public const string ServiceList = "Permissions.HotelManagement.ServiceList";
            public const string OnlineBookingList = "Permissions.HotelManagement.OnlineBookingList";
            public const string Booking = "Permissions.HotelManagement.Booking";
            public const string BookingUpdate = "Permissions.HotelManagement.BookingUpdate";
            public const string BookingList = "Permissions.HotelManagement.BookingList";
            public const string NewHallBooking = "Permissions.HotelManagement.NewHallBooking";
            public const string HallBookingList = "Permissions.HotelManagement.HallBookingList";
            public const string BillList = "Permissions.HotelManagement.BillList";
            public const string NightAudit = "Permissions.HotelManagement.NightAudit";
            public const string RefundCancelBooking = "Permissions.HotelManagement.RefundCancelBooking";


            public static class Reports
            {
                public const string View = "Permissions.HotelManagement.Reports.View";
                public const string ForecastReport = "Permissions.HotelManagement.Reports.ForecastReport";
                public const string ReservationReport = "Permissions.HotelManagement.Reports.ReservationReport";
                public const string CheckAvailability = "Permissions.HotelManagement.Reports.CheckAvailability";
                public const string CheckHallAvailability = "Permissions.HotelManagement.Reports.CheckHallAvailability";
                public const string RoomBookingSchedule = "Permissions.HotelManagement.Reports.RoomBookingSchedule";
                public const string ArrivalReport = "Permissions.HotelManagement.Reports.ArrivalReport";
                public const string ExpectedCheckoutReport = "Permissions.HotelManagement.Reports.ExpectedCheckoutReport";
                public const string CheckoutReport = "Permissions.HotelManagement.Reports.CheckoutReport";
                public const string InHouseGuestReport = "Permissions.HotelManagement.Reports.InHouseGuestReport";
                public const string InHouseGuestLedger = "Permissions.HotelManagement.Reports.InHouseGuestLedger";
                public const string CancelReport = "Permissions.HotelManagement.Reports.CancelReport";
                public const string DailySalesReport = "Permissions.HotelManagement.Reports.DailySalesReport";
                public const string ExtraServiceReport = "Permissions.HotelManagement.Reports.ExtraServiceReport";
                public const string GuestDueReport = "Permissions.HotelManagement.Reports.GuestDueReport";
                public const string GuestAdvanceReport = "Permissions.HotelManagement.Reports.GuestAdvanceReport";
                public const string PaymentTransaction = "Permissions.HotelManagement.Reports.PaymentTransaction";
                public const string RoomOccupancy = "Permissions.HotelManagement.Reports.RoomOccupancy";
                public const string AuditReportDay = "Permissions.HotelManagement.Reports.AuditReportDay";
                public const string NightAuditSummary = "Permissions.HotelManagement.Reports.NightAuditSummary";
                public const string RefundReport = "Permissions.HotelManagement.Reports.RefundReport";
            }
        }

        public static class UserRoles
        {
            public const string ListView = "Permissions.UserRoles.ListView";
            public const string CreateOrEdit = "Permissions.UserRoles.CreateOrEdit";
        }

        public static class AcademicExams
        {
            public const string ListView = "Permissions.AcademicExams.ListView";
            public const string Create = "Permissions.AcademicExams.Create";
            public const string Edit = "Permissions.AcademicExams.Edit";
        }

        public static class AccGroups
        {
            public const string ListView = "Permissions.AccGroups.ListView";
            public const string Create = "Permissions.AccGroups.Create";
            public const string Edit = "Permissions.AccGroups.Edit";
            public const string Delete = "Permissions.AccGroups.Delete";
            public const string ChartOfAcc = "Permissions.AccGroups.ChartOfAcc";
        }

        public static class AccHeads
        {
            public const string ListView = "Permissions.AccHeads.ListView";
            public const string Create = "Permissions.AccHeads.Create";
            public const string Edit = "Permissions.AccHeads.Edit";
            public const string Delete = "Permissions.AccHeads.Delete";
        }

        public static class AccLedgers
        {
            public const string ListView = "Permissions.AccLedgers.ListView";
            public const string Create = "Permissions.AccLedgers.Create";
            public const string Edit = "Permissions.AccLedgers.Edit";
            public const string Delete = "Permissions.AccLedgers.Delete";
        }

        public static class AccTranMsts
        {
            public const string ListView = "Permissions.AccTranMsts.ListView";
            public const string DetailsView = "Permissions.AccTranMsts.DetailsView";
            public const string JournalCreate = "Permissions.AccTranMsts.JournalCreate";
            public const string AccOpeningCreate = "Permissions.AccTranMsts.AccOpeningCreate";
            public const string BankDebitVoucher = "Permissions.AccTranMsts.BankDebitVoucher";
            public const string BankCreditVoucher = "Permissions.AccTranMsts.BankCreditVoucher";
            public const string CashDebitVoucher = "Permissions.AccTranMsts.CashDebitVoucher";
            public const string CashCreditVoucher = "Permissions.AccTranMsts.CashCreditVoucher";
            public const string Edit = "Permissions.AccTranMsts.Edit";
            public const string ReportView = "Permissions.AccTranMsts.ReportView";
            public const string VoucherSearch = "Permissions.AccTranMsts.VoucherSearch";
        }

        //public static class Assignments
        //{
        //    public const string ListView = "Permissions.Assignments.ListView";
        //    public const string Create = "Permissions.Assignments.Create";
        //    public const string Edit = "Permissions.Assignments.Edit";
        //    public const string Delete = "Permissions.Assignments.Delete";
        //}

        public static class Batches
        {
            public const string ListView = "Permissions.Batches.ListView";
            public const string Create = "Permissions.Batches.Create";
            public const string Edit = "Permissions.Batches.Edit";
            public const string Delete = "Permissions.Batches.Delete";
        }

        public static class DutyShifts
        {
            public const string ListView = "Permissions.DutyShifts.ListView";
            public const string Create = "Permissions.DutyShifts.Create";
            public const string Edit = "Permissions.DutyShifts.Edit";
            public const string Delete = "Permissions.DutyShifts.Delete";
        }

        public static class ShiftManagements
        {
            public const string ListView = "Permissions.ShiftManagements.ListView";
            public const string Create = "Permissions.ShiftManagements.Create";
        }

        public static class ClassTypes
        {
            public const string ListView = "Permissions.ClassTypes.ListView";
            public const string Create = "Permissions.ClassTypes.Create";
            public const string Edit = "Permissions.ClassTypes.Edit";
        }

        public static class ClinicalDisciplines
        {
            public const string ListView = "Permissions.ClinicalDisciplines.ListView";
            public const string Create = "Permissions.ClinicalDisciplines.Create";
            public const string Edit = "Permissions.ClinicalDisciplines.Edit";
        }

        public static class Courses
        {
            public const string ListView = "Permissions.Courses.ListView";
            public const string Create = "Permissions.Courses.Create";
            public const string Edit = "Permissions.Courses.Edit";
            public const string Delete = "Permissions.Courses.Delete";
        }

        public static class Departments
        {
            public const string ListView = "Permissions.Departments.ListView";
            public const string ListViewAcademicDepartment = "Permissions.Departments.ListViewAcademicDepartment";
            public const string Create = "Permissions.Departments.Create";
            public const string CreateAcademicDepartment = "Permissions.Departments.CreateAcademicDepartment";
            public const string Edit = "Permissions.Departments.Edit";
            public const string EditAcademicDepartment = "Permissions.Departments.EditAcademicDepartment";
            public const string Delete = "Permissions.Departments.Delete";
            public const string DeleteAcademicDepartment = "Permissions.Departments.DeleteAcademicDepartment";
        }

        public static class DptLeaveReviewers
        {
            public const string Create = "Permissions.DptLeaveReviewers.Create";
            public const string Delete = "Permissions.DptLeaveReviewers.Delete";
        }

        public static class Designations
        {
            public const string ListView = "Permissions.Designations.ListView";
            public const string Create = "Permissions.Designations.Create";
            public const string Edit = "Permissions.Designations.Edit";
            public const string Delete = "Permissions.Designations.Delete";
        }

        public static class SetFincYear
        {
            public const string ListView = "Permissions.SetFincYear.ListView";
            public const string Create = "Permissions.SetFincYear.Create";
            public const string Edit = "Permissions.SetFincYear.Edit";
            public const string Delete = "Permissions.SetFincYear.Delete";
        }

        public static class DiscountInfos
        {
            public const string ListView = "Permissions.DiscountInfos.ListView";
            public const string Create = "Permissions.DiscountInfos.Create";
            public const string Edit = "Permissions.DiscountInfos.Edit";
            public const string Delete = "Permissions.DiscountInfos.Delete";
        }

        public static class EmpAttendances
        {
            public const string Create = "Permissions.EmpAttendances.Create";
            public const string ReportView = "Permissions.EmpAttendances.ReportView";
        }

        public static class EmpLeaveApplications
        {
            public const string ListView = "Permissions.EmpLeaveApplications.ListView";
            public const string DetailsView = "Permissions.EmpLeaveApplications.DetailsView";
            public const string Create = "Permissions.EmpLeaveApplications.Create";
            public const string Edit = "Permissions.EmpLeaveApplications.Edit";
            public const string Delete = "Permissions.EmpLeaveApplications.Delete";
            public const string ReportView = "Permissions.EmpLeaveApplications.ReportView";
            public const string StatementView = "Permissions.EmpLeaveApplications.StatementView";
        }

        public static class EmpLeaveReviewers
        {
            public const string Create = "Permissions.EmpLeaveReviewers.Create";
            public const string Delete = "Permissions.EmpLeaveReviewers.Delete";
        }

        public static class EmpLoanMsts
        {
            public const string ListView = "Permissions.EmpLoanMsts.ListView";
            public const string DetailsView = "Permissions.EmpLoanMsts.DetailsView";
            public const string Create = "Permissions.EmpLoanMsts.Create";
        }

        public static class Employees
        {
            public const string ListView = "Permissions.Employees.ListView";
            public const string DetailsView = "Permissions.Employees.DetailsView";
            public const string Create = "Permissions.Employees.Create";
            public const string Edit = "Permissions.Employees.Edit";
            public const string ReportView = "Permissions.Employees.ReportView";
            public const string Disable = "Permissions.Employees.Disable";
        }

        public static class ExamResult
        {
            public const string Create = "Permissions.ExamResult.Create";
        }
        public static class FeeSetups
        {
            public const string ListView = "Permissions.FeeSetups.ListView";
            public const string Create = "Permissions.FeeSetups.Create";
            public const string Edit = "Permissions.FeeSetups.Edit";
            public const string Delete = "Permissions.FeeSetups.Delete";
            public const string ReportView = "Permissions.FeeSetups.ReportView";
        }

        public static class FeesReceiveMsts
        {
            public const string ListView = "Permissions.FeesReceiveMsts.ListView";
            public const string Create = "Permissions.FeesReceiveMsts.Create";
            public const string DetailsView = "Permissions.FeesReceiveMsts.DetailsView";
            public const string ReportView = "Permissions.FeesReceiveMsts.ReportView";
        }

        public static class FeeTypes
        {
            public const string ListView = "Permissions.FeeTypes.ListView";
            public const string Create = "Permissions.FeeTypes.Create";
            public const string Edit = "Permissions.FeeTypes.Edit";
            public const string Delete = "Permissions.FeeTypes.Delete";
        }

        public static class LeaveSetups
        {
            public const string ListView = "Permissions.LeaveSetups.ListView";
            public const string Create = "Permissions.LeaveSetups.Create";
            public const string Edit = "Permissions.LeaveSetups.Edit";
            public const string Delete = "Permissions.LeaveSetups.Delete";
        }

        public static class HostelFee
        {
            public const string ListView = "Permissions.HostelFee.ListView";
            public const string Create = "Permissions.HostelFee.Create";
            public const string Edit = "Permissions.HostelFee.Edit";
            public const string Delete = "Permissions.HostelFee.Delete";
        }

        //public static class Home
        //{
        //    public const string Index = "Permissions.Home.Index";
        //}

        public static class HrSettings
        {
            public const string Edit = "Permissions.HrSettings.Edit";
            public const string DetailsView = "Permissions.HrSettings.DetailsView";
        }

        public static class LeaveTypes
        {
            public const string ListView = "Permissions.LeaveTypes.ListView";
            public const string Create = "Permissions.LeaveTypes.Create";
            public const string Edit = "Permissions.LeaveTypes.Edit";
            public const string Delete = "Permissions.LeaveTypes.Delete";
        }

        public static class LeaveApplicationReviewers
        {
            public const string MyReviewApps = "Permissions.LeaveApplicationReviewers.MyReviewApps";
            public const string ListApp = "Permissions.LeaveApplicationReviewers.ListApp";
            public const string RejectApp = "Permissions.LeaveApplicationReviewers.RejectApp";
            public const string ReviewApp = "Permissions.LeaveApplicationReviewers.ReviewApp";
            public const string ApproveApp = "Permissions.LeaveApplicationReviewers.ApproveApp";
        }

        public static class CfLeave
        {
            public const string ListView = "Permissions.CfLeave.ListView";
            public const string Create = "Permissions.CfLeave.Create";
            public const string Edit = "Permissions.CfLeave.Edit";
            public const string Delete = "Permissions.CfLeave.Delete";
        }

        public static class MonthlyAttendanceSheetMsts
        {
            public const string Create = "Permissions.MonthlyAttendanceSheetMsts.Create";
            public const string ListView = "Permissions.MonthlyAttendanceSheetMsts.ListView";
            public const string DetailsView = "Permissions.MonthlyAttendanceSheetMsts.DetailsView";
            public const string ReportView = "Permissions.MonthlyAttendanceSheetMsts.ReportView";
        }

        public static class PfFundMsts
        {
            public const string ReportGenerate = "Permissions.PfFundMsts.ReportGenerate";
            public const string ListView = "Permissions.PfFundMsts.ListView";
            public const string DetailsView = "Permissions.PfFundMsts.DetailsView";
            public const string ReportView = "Permissions.PfFundMsts.ReportView";
        }

        public static class PfFundOpennings
        {
            public const string ListView = "Permissions.PfFundOpennings.ListView";
            public const string Create = "Permissions.PfFundOpennings.Create";
        }

        public static class PfSettings
        {
            public const string Edit = "Permissions.PfSettings.Edit";
            public const string DetailsView = "Permissions.PfSettings.DetailsView";
        }

        public static class PfSettlements
        {
            public const string ListView = "Permissions.PfSettlements.ListView";
            public const string Create = "Permissions.PfSettlements.Create";
            public const string Edit = "Permissions.PfSettlements.Edit";
        }

        public static class PrArrearMsts
        {
            public const string ListView = "Permissions.PrArrearMsts.ListView";
            public const string DetailsView = "Permissions.PrArrearMsts.DetailsView";
            public const string Create = "Permissions.PrArrearMsts.Create";
            public const string Delete = "Permissions.PrArrearMsts.Delete";
            public const string Approve = "Permissions.PrArrearMsts.Approve";
        }

        public static class PrEmpSalaryParts
        {
            public const string ListView = "Permissions.PrEmpSalaryParts.ListView";
            public const string DetailsView = "Permissions.PrEmpSalaryParts.DetailsView";
            public const string Create = "Permissions.PrEmpSalaryParts.Create";
        }

        public static class PrGuestSalaryMsts
        {
            public const string ListView = "Permissions.PrGuestSalaryMsts.ListView";
            public const string DetailsView = "Permissions.PrGuestSalaryMsts.DetailsView";
            public const string Create = "Permissions.PrGuestSalaryMsts.Create";
            public const string Approve = "Permissions.PrGuestSalaryMsts.Approve";
        }

        public static class PrSalaryMsts
        {
            public const string ListView = "Permissions.PrSalaryMsts.ListView";
            public const string DetailsView = "Permissions.PrSalaryMsts.DetailsView";
            public const string Payroll = "Permissions.PrSalaryMsts.Payroll";
            public const string SetPayroll = "Permissions.PrSalaryMsts.SetPayroll";
            public const string Approve = "Permissions.PrSalaryMsts.Approve";
            public const string Payslip = "Permissions.PrSalaryMsts.Payslip";
            public const string PayMultiSalary = "Permissions.PrSalaryMsts.PayMultiSalary";
            public const string PayPayslip = "Permissions.PrSalaryMsts.PayPayslip";
        }

        public static class PrSalaryParts
        {
            public const string ListView = "Permissions.PrSalaryParts.ListView";
            public const string Edit = "Permissions.PrSalaryParts.Edit";
        }

        public static class Semesters
        {
            public const string ListView = "Permissions.Semesters.ListView";
            public const string DetailsView = "Permissions.Semesters.DetailsView";
            public const string Create = "Permissions.Semesters.Create";
            public const string Edit = "Permissions.Semesters.Edit";
        }

        public static class SemesterSubjectMaps
        {
            public const string ListView = "Permissions.SemesterSubjectMaps.ListView";
            public const string Create = "Permissions.SemesterSubjectMaps.Create";
            public const string SearchPhaseSubject = "Permissions.SemesterSubjectMaps.SearchPhaseSubject";
        }

        public static class Sessions
        {
            public const string ListView = "Permissions.Sessions.ListView";
            public const string Create = "Permissions.Sessions.Create";
            public const string Edit = "Permissions.Sessions.Edit";
            public const string Delete = "Permissions.Sessions.Delete";
        }

        public static class SetCurrencies
        {
            public const string ListView = "Permissions.SetCurrencies.ListView";
            public const string Create = "Permissions.SetCurrencies.Create";
            public const string Edit = "Permissions.SetCurrencies.Edit";
            public const string Delete = "Permissions.SetCurrencies.Delete";
        }

        public static class SetHolidays
        {
            public const string ListView = "Permissions.SetHolidays.ListView";
            public const string Create = "Permissions.SetHolidays.Create";
            public const string Edit = "Permissions.SetHolidays.Edit";
            public const string Delete = "Permissions.SetHolidays.Delete";
        }

        public static class SetSalaryGrades
        {
            public const string ListView = "Permissions.SetSalaryGrades.ListView";
            public const string Create = "Permissions.SetSalaryGrades.Create";
            public const string Edit = "Permissions.SetSalaryGrades.Edit";
            public const string Delete = "Permissions.SetSalaryGrades.Delete";
        }

        public static class StuAcademicHsts
        {
            public const string ListView = "Permissions.StuAcademicHsts.ListView";
            public const string Create = "Permissions.StuAcademicHsts.Create";
            public const string Edit = "Permissions.StuAcademicHsts.Edit";
        }

        public static class StudentClerkships
        {
            public const string ListView = "Permissions.StudentClerkships.ListView";
            public const string Create = "Permissions.StudentClerkships.Create";
            public const string Edit = "Permissions.StudentClerkships.Edit";
        }

        public static class StudentInfos
        {
            public const string ListView = "Permissions.StudentInfos.ListView";
            public const string Create = "Permissions.StudentInfos.Create";
            public const string DetailsView = "Permissions.StudentInfos.DetailsView";
            public const string Edit = "Permissions.StudentInfos.Edit";
        }

        public static class StudentTranscripts
        {
            public const string ListView = "Permissions.StudentTranscripts.ListView";
            public const string Create = "Permissions.StudentTranscripts.Create";
            public const string DetailsView = "Permissions.StudentTranscripts.DetailsView";
            public const string Edit = "Permissions.StudentTranscripts.Edit";
        }

        public static class StudentTestimonial
        {
            public const string ReportView = "Permissions.StudentTranscripts.ReportView";
        }

        public static class SubjectClassSetups
        {
            public const string Create = "Permissions.SubjectClassSetups.Create";
        }

        public static class Subjects
        {
            public const string ListView = "Permissions.Subjects.ListView";
            public const string Create = "Permissions.Subjects.Create";
            public const string Edit = "Permissions.Subjects.Edit";
            public const string Delete = "Permissions.Subjects.Delete";
        }

        public static class Settings
        {
            public const string ListView = "Permissions.Settings.ListView";
            public const string Create = "Permissions.Settings.Create";
            public const string Edit = "Permissions.Settings.Edit";
            public const string Delete = "Permissions.Settings.Delete";
        }

        public static class SystemSettings
        {
            public const string BackupDB = "Permissions.SystemSettings.BackupDB";
        }

        public static class RoomCategories
        {
            public const string ListView = "Permissions.RoomCategories.ListView";
            public const string Create = "Permissions.RoomCategories.Create";
            public const string DetailView = "Permissions.RoomCategories.DetailView";
            public const string Edit = "Permissions.RoomCategories.Edit";
            public const string Delete = "Permissions.RoomCategories.Delete";
        }

        public static class FloorInfos
        {
            public const string ListView = "Permissions.FloorInfos.ListView";
            public const string Create = "Permissions.FloorInfos.Create";
            public const string DetailView = "Permissions.FloorInfos.DetailView";
            public const string Edit = "Permissions.FloorInfos.Edit";
            public const string Delete = "Permissions.FloorInfos.Delete";
        }

        public static class RoomFacilities
        {
            public const string ListView = "Permissions.RoomFacilities.ListView";
            public const string Create = "Permissions.RoomFacilities.Create";
            public const string DetailView = "Permissions.RoomFacilities.DetailView";
            public const string Edit = "Permissions.RoomFacilities.Edit";
            public const string Delete = "Permissions.RoomFacilities.Delete";
        }

        public static class RoomFacilityCategories
        {
            public const string ListView = "Permissions.RoomFacilityCategories.ListView";
            public const string Create = "Permissions.RoomFacilityCategories.Create";
            public const string DetailView = "Permissions.RoomFacilityCategories.DetailView";
            public const string Edit = "Permissions.RoomFacilityCategories.Edit";
            public const string Delete = "Permissions.RoomFacilityCategories.Delete";
        }

        public static class RoomInfos
        {
            public const string ListView = "Permissions.RoomInfos.ListView";
            public const string Create = "Permissions.RoomInfos.Create";
            public const string Edit = "Permissions.RoomInfos.Edit";
            public const string RoomAssign = "Permissions.RoomInfos.RoomAssign";
            public const string Details = "Permissions.RoomInfos.Details";
        }

        public static class BookingServices
        {
            public const string ListView = "Permissions.BookingServices.ListView";
            public const string Create = "Permissions.BookingServices.Create";
            public const string Details = "Permissions.BookingServices.Details";
            public const string CheckIn = "Permissions.BookingServices.CheckIn";
            public const string CheckOut = "Permissions.BookingServices.CheckOut";
            public const string Payment = "Permissions.BookingServices.Payment";
        }

        public static class TaskTypes
        {
            public const string ListView = "Permissions.TaskTypes.ListView";
            public const string Create = "Permissions.TaskTypes.Create";
            public const string Edit = "Permissions.TaskTypes.Edit";
            public const string Delete = "Permissions.TaskTypes.Delete";
        }

        public static class TaskNames
        {
            public const string ListView = "Permissions.TaskNames.ListView";
            public const string Create = "Permissions.TaskNames.Create";
            public const string Edit = "Permissions.TaskNames.Edit";
            public const string Delete = "Permissions.TaskNames.Delete";
        }

        public static class RoomAssigns
        {
            public const string ListView = "Permissions.RoomAssigns.ListView";
            public const string Create = "Permissions.RoomAssigns.Create";
            public const string AssignRoom = "Permissions.RoomAssigns.AssignRoom";
            public const string UnassignRoom = "Permissions.RoomAssigns.UnassignRoom";
            public const string AssignSingleRoom = "Permissions.RoomAssigns.AssignSingleRoom";
            public const string UnassignSingleRoom = "Permissions.RoomAssigns.UnassignSingleRoom";
            public const string CleaningStatusChange = "Permissions.RoomAssigns.CleaningStatusChange";
            public const string AvailabilityStatusChange = "Permissions.RoomAssigns.AvailabilityStatusChange";
            public const string GetAssignRooms = "Permissions.RoomAssigns.GetAssignRooms";
        }

        public static class TaskAssigns
        {
            public const string ListView = "Permissions.TaskAssigns.ListView";
            public const string Create = "Permissions.TaskAssigns.Create";
            public const string MyTask = "Permissions.TaskAssigns.MyTask";
        }

        public static class FoodCategories
        {
            public const string ListView = "Permissions.FoodCategories.ListView";
            public const string Create = "Permissions.FoodCategories.Create";
            public const string DetailView = "Permissions.FoodCategories.DetailView";
            public const string Edit = "Permissions.FoodCategories.Edit";
            public const string Delete = "Permissions.FoodCategories.Delete";
        }

        public static class FoodItems
        {
            public const string ListView = "Permissions.FoodItems.ListView";
            public const string Create = "Permissions.FoodItems.Create";
            public const string DetailView = "Permissions.FoodItems.DetailView";
            public const string Edit = "Permissions.FoodItems.Edit";
            public const string Delete = "Permissions.FoodItems.Delete";
        }

        public static class FoodOrders
        {
            public const string ListView = "Permissions.FoodOrders.ListView";
            public const string Create = "Permissions.FoodOrders.Create";
            public const string Details = "Permissions.FoodOrders.Details";
            public const string OrderBillPrint = "Permissions.FoodOrders.OrderBillPrint";
            public const string OrderKotPrint = "Permissions.FoodOrders.OrderKotPrint";
            public const string FoodOrderStatusChange = "Permissions.FoodOrders.FoodOrderStatusChange";
            public const string PayOrder = "Permissions.FoodOrders.PayOrder";
        }

        public static class Tables
        {
            public const string ListView = "Permissions.Tables.ListView";
            public const string Create = "Permissions.Tables.Create";
            public const string DetailView = "Permissions.Tables.DetailView";
            public const string Edit = "Permissions.Tables.Edit";
            public const string Delete = "Permissions.Tables.Delete";
        }

        public static class BedTypes
        {
            public const string ListView = "Permissions.BedTypes.ListView";
            public const string Create = "Permissions.BedTypes.Create";
            public const string Edit = "Permissions.BedTypes.Edit";
            public const string Delete = "Permissions.BedTypes.Delete";
        }

        public static class Customers
        {
            public const string ListView = "Permissions.Customers.ListView";
            public const string Create = "Permissions.Customers.Create";
            public const string Edit = "Permissions.Customers.Edit";
            public const string Delete = "Permissions.Customers.Delete";
        }

        public static class Complementary
        {
            public const string ListView = "Permissions.Complementary.ListView";
            public const string Create = "Permissions.Complementary.Create";
            public const string Edit = "Permissions.Complementary.Edit";
            public const string Delete = "Permissions.Complementary.Delete";
        }

        public static class GuestInfos
        {
            public const string ListView = "Permissions.GuestInfos.ListView";
            public const string Create = "Permissions.GuestInfos.Create";
            public const string Edit = "Permissions.GuestInfos.Edit";
            public const string Delete = "Permissions.GuestInfos.Delete";
        }

        public static class Services
        {
            public const string ListView = "Permissions.Services.ListView";
            public const string Create = "Permissions.Services.Create";
            public const string Edit = "Permissions.Services.Edit";
            public const string Delete = "Permissions.Services.Delete";
        }

        public static class CategoryInfo
        {
            public const string ListView = "Permissions.CategoryInfo.ListView";
            public const string Create = "Permissions.CategoryInfo.Create";
            public const string Edit = "Permissions.CategoryInfo.Edit";
            public const string Delete = "Permissions.CategoryInfo.Delete";
        }

        public static class ItemInfo
        {
            public const string ListView = "Permissions.ItemInfo.ListView";
            public const string Create = "Permissions.ItemInfo.Create";
            public const string Edit = "Permissions.ItemInfo.Edit";
            public const string Delete = "Permissions.ItemInfo.Delete";
        }

        public static class UnitInfo
        {
            public const string ListView = "Permissions.UnitInfo.ListView";
            public const string Create = "Permissions.UnitInfo.Create";
            public const string Edit = "Permissions.UnitInfo.Edit";
            public const string Delete = "Permissions.UnitInfo.Delete";
        }

        public static class SupplierInfo
        {
            public const string ListView = "Permissions.SupplierInfo.ListView";
            public const string Create = "Permissions.SupplierInfo.Create";
            public const string Edit = "Permissions.SupplierInfo.Edit";
            public const string Delete = "Permissions.SupplierInfo.Delete";
            public const string Detail = "Permissions.SupplierInfo.Detail";
        }

        public static class InventoryOpening
        {
            public const string CreateOrEdit = "Permissions.InventoryOpening.CreateOrEdit";
            public const string Detail = "Permissions.InventoryOpening.Detail";
        }

        public static class Consumption
        {
            public const string ListView = "Permissions.Consumption.ListView";
            public const string Create = "Permissions.Consumption.Create";
            public const string Detail = "Permissions.Consumption.Detail";
        }

        public static class HkConsumption
        {
            public const string ListView = "Permissions.HkConsumption.ListView";
            public const string Create = "Permissions.HkConsumption.Create";
            public const string Detail = "Permissions.HkConsumption.Detail";
        }

        public static class RestaurantConsumption
        {
            public const string ListView = "Permissions.RestaurantConsumption.ListView";
            public const string Create = "Permissions.RestaurantConsumption.Create";
            public const string Detail = "Permissions.RestaurantConsumption.Detail";
        }

        public static class Order
        {
            public const string ListView = "Permissions.Order.ListView";
            public const string Create = "Permissions.Order.Create";
            public const string Detail = "Permissions.Order.Detail";
            public const string PayBill = "Permissions.Order.PayBill";
        }

        public static class Receive
        {
            public const string ListView = "Permissions.Receive.ListView";
            public const string Create = "Permissions.Receive.Create";
            public const string Detail = "Permissions.Receive.Detail";
        }

        public static class Issue
        {
            public const string ListView = "Permissions.Issue.ListView";
            public const string Create = "Permissions.Issue.Create";
            public const string Detail = "Permissions.Issue.Detail";
        }

        public static class Requisition
        {
            public const string ListView = "Permissions.Requisition.ListView";
            public const string Create = "Permissions.Requisition.Create";
            public const string Detail = "Permissions.Requisition.Detail";
            public const string DepartmentWiseCreate = "Permissions.Requisition.DepartmentWiseCreate";
            public const string DepartmentWiseList = "Permissions.Requisition.DepartmentWiseList";
        }

        public static class InventoryReport
        {
            public const string View = "Permissions.InventoryReport.View";
        }

        public static class Bills
        {
            public const string ListView = "Permissions.Bills.ListView";
            public const string Details = "Permissions.Bills.Details";
            public const string PayBills = "Permissions.Bills.PayBills";
        }

        public static class OnlineBooking
        {
            public const string ListView = "Permissions.OnlineBooking.ListView";
            public const string Approve = "Permissions.OnlineBooking.Approve";
            public const string Reject = "Permissions.OnlineBooking.Reject";
            public const string Delete = "Permissions.OnlineBooking.Delete";
        }

        public static class EmpDisciplinary
        {
            public const string Create = "Permissions.EmpDisciplinary.Create";
            public const string GetEmployeeDiscipline = "Permissions.EmpDisciplinary.GetEmployeeDiscipline";
            public const string Delete = "Permissions.EmpDisciplinary.Delete";
        }

        public static class EmpEducation
        {
            public const string Create = "Permissions.EmpEducation.Create";
            public const string GetEmployeeEducation = "Permissions.EmpEducation.GetEmployeeEducation";
            public const string Delete = "Permissions.EmpEducation.Delete";
        }

        public static class EmpExperience
        {
            public const string Create = "Permissions.EmpExperience.Create";
            public const string GetEmployeeExperience = "Permissions.EmpExperience.GetEmployeeExperience";
            public const string Delete = "Permissions.EmpExperience.Delete";
        }

        public static class EmpJournal
        {
            public const string Create = "Permissions.EmpJournal.Create";
            public const string GetEmployeeJournal = "Permissions.EmpJournal.GetEmployeeJournal";
            public const string Delete = "Permissions.EmpJournal.Delete";
        }

        public static class EmpPosting
        {
            public const string Create = "Permissions.EmpPosting.Create";
            public const string GetEmployeePosting = "Permissions.EmpPosting.GetEmployeePosting";
            public const string Delete = "Permissions.EmpPosting.Delete";
        }

        public static class EmpReference
        {
            public const string Create = "Permissions.EmpReference.Create";
            public const string GetEmployeeReference = "Permissions.EmpReference.GetEmployeeReference";
            public const string Delete = "Permissions.EmpReference.Delete";
        }

        public static class EmpTraining
        {
            public const string Create = "Permissions.EmpTraining.Create";
            public const string GetEmployeeTraining = "Permissions.EmpTraining.GetEmployeeTraining";
            public const string Delete = "Permissions.EmpTraining.Delete";
        }

        public static class InventoryBillPayment
        {
            public const string ListView = "Permissions.InventoryBillPayment.ListView";
            public const string Detail = "Permissions.InventoryBillPayment.Detail";
        }
    }
}