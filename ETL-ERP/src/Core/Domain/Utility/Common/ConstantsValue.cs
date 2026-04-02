namespace Domain.Utility.Common;

public static class ConstantsValue
{
    public static UserRoleName UserRoleName => new UserRoleName();
    public static RolePermission RolePermission => new RolePermission();
}

public static class ExtraBedAmount
{
    public static short ExtraBedSingleCharge => 500;
}

public class UserRoleName
{
    public string SuperAdmin => "SuperAdmin";
    public string Admin => "Admin";
    public string NormalUser => "Employee";
    public string DistrictTrainer => "Teacher";
    public string Entrepreneur => "Student";

}

public class RolePermission
{
    public string Type => "Permission";
    public string Value => "Permissions.SuperAdmin";
}

public static class PrintInfo
{
    public static string CompanyName => "Hotel Mishuk";
    public static string CompanyRestaurantName => "Taste of Nawab";
    public static string CompanyAddress => "Hotel Motel Zone Sea Beach Road, Cox's Bazar, Bangladesh";
    public static string PrincipalName => "";
    public static string PrincipalQualification => "";

}

public static class PrSalaryPartValueType
{
    public static string Amount => "A";
    public static string Percent => "P";
}

public static class PrSalaryPartType
{
    public static string Addition => "A";
    public static string Deduction => "D";
}

public static class PrSalaryPartCol
{
    public static string ColA => "ColA";
    public static string ColB => "ColB";
    public static string ColC => "ColC";
    public static string ColD => "ColD";
    public static string ColE => "ColE";
    public static string ColF => "ColF";
    public static string ColG => "ColG";
    public static string ColH => "ColH";
    public static string ColI => "ColI";
    public static string ColJ => "ColJ";
    public static string ColK => "ColK";
    public static string ColL => "ColL";
    public static string ColM => "ColM";
    public static string ColN => "ColN";
    public static string ColO => "ColO";
    public static string ColP => "ColP";
    public static string ColQ => "ColQ";
    public static string ColR => "ColR";
    public static string ColS => "ColS";
    public static string ColT => "ColT";
    public static string ColU => "ColU";
}

public static class PrSalaryPartLink
{
    public static string Basic => "B";
    public static string Arrear => "A";
    public static string Loan => "L";
    public static string Pf => "P";
    public static string SalaryDeduct => "D";
    public static string GratuityA => "G";
    public static string GratuityD => "R";
    public static string LoanInterest => "I";
    public static string FoodBill => "F";
}

public static class EmpAttendanceStatus
{
    public static string Present => "P";
    public static string Absent => "A";
    public static string Leave => "L";
    public static string Holiday => "H";
    public static string Offday => "O";

}

public static class VoucherType
{
    public static string OpeningVoucher => "O";
    public static string JournalVoucher => "J";
    public static string BankDebitVoucher => "D";
    public static string CashDebitVoucher => "S";
    public static string CashCreditVoucher => "H";
    public static string BankCreditVoucher => "C";
    public static string PettyCash => "P";
}

public static class VoucherTypeCode
{
    public static string OpeningVoucher => "OP";
    public static string JournalVoucher => "JV";
    public static string BankDebitVoucher => "BD";
    public static string CashDebitVoucher => "CD";
    public static string CashCreditVoucher => "CC";
    public static string BankCreditVoucher => "BC";
}

public static class PfReportType
{
    public static string Monthly => "M";
    public static string Interest => "I";
    public static string Profit => "P";
    public static string Others => "O";
}

public static class VcNoteType
{
    public static string Create => "Create";
    public static string Approve => "Approve";
    public static string Audit => "Audit";
    public static string BankClear => "BankClear";
    public static string FinalAttach => "FinalAttach";
    public static string Note => "Note";
    public static string AutoCreate => "AutoCreate";

}

public static class AccHeadCode
{
    //public static string HotelMisukHead => "5.1";

    public static string HotelMisukHead => "1.2.6";
    public static string GuestDueHead => "1.2.4.1";
}

public static class AccLadgerCode
{
    //public static string RoomRentReceive => "L2.1.1.1";
    //public static string RoomAdvanceRentReceive => "L3.2.1.1.1";//"L2.1.1.2";
    //public static string CityBankLtd => "L1.2.5.2.1.1";
    //public static string CashInHand => "L1.2.5.1.1";
    //public static string HotelMisuk => "L5.1.1";
    //public static string RMPurchase => "L4.4.1";

    //public static string RestaurantLedger => "L5.1.2";
    //public static string AmariResortLedger => "L5.1.3";
    //public static string StaffKitchenLedger => "L5.1.4";


    public static string RoomRentReceive => "L2.1.1.1";
    public static string HallRentReceive => "L2.1.1.19";
    public static string RoomAdvanceRentReceive => "L3.2.1.1.1";//"L2.1.1.2";
    public static string HallAdvanceRentReceive => "L3.2.1.2.1";//"L2.1.1.2";
    public static string CityBankLtd => "L1.2.5.2.1.1";
    public static string CashInHand => "L1.2.5.1.1";

    public static string FoodPaymentReceive => "L2.1.3.1";

    public static string RMPurchase => "L4.4.1";

    public static string HotelMisuk => "L1.2.6.1";
    public static string RestaurantLedger => "L1.2.6.2";
    public static string AmariResortLedger => "L1.2.6.3";
    public static string StaffKitchenLedger => "L1.2.6.4";
    public static string AdvanceMadeLedger => "L1.2.2.1.1";
    public static string AdvanceRecoveredLedger => "L1.2.2.4.1";
    public static string SalaryPaidStaffs => "L4.1.2.1";
    public static string SalaryDeductedStaffs => "L4.1.2.3";
    public static string ExtraBedChargeLedger => "L2.1.1.15"; // NTS : Need To Setup for each new project
    public static string DiscountBillPaidLedger => "L4.2.37"; // NTS : Need To Setup for each new project
}

public static class CountryCode
{
    public static string LocalCountry => "BD";
}

public static class CourseCode
{
    public static string MBBS => "MBBS";
    public static string BDS => "BDS";
}

public static class TransType
{
    public const string Receive = "R";
    public const string Issue = "I";
    public const string Opening = "O";
    public const string Scrap = "S";
    public const string Return = "U";
    public const string IssueReturn = "E";
    public const string Consumption = "C";
    public const string LeftOver = "L";
}

public static class HtServiceCode
{
    public static string RoomRent => "SR000001";
    public static string FoodService => "SR000002";
    public static string HallRent => "SR000004";
    public static string ExtraBed => "SR000009";
}

public static class RsCustomerTypeCode
{
    public static string Hotel => "Hotel";
    public static string WalkIn => "Walk-In";
    public static string Employee => "Employee";
    public static string Online => "Online";
    public static string Complementary => "Complementary";
    public static string MD_Sir => "MD";
    public static string GM_Sir => "GM";
}

public static class DepartmentCode
{
    public static string FrontDesk => "DPT-01";
    public static string HouseKeeper => "DPT-02";
    public static string Resturant => "DPT-03";
    public static string Accounting => "DPT-04";
    public static string HR => "DPT-05";
    public static string Store => "DPT-06";
    public static string StaffKitchen => "DPT-07";
    public static string AmariResort => "DPT-08";
    public static string HotelMishukAdmin => "DPT-09";
    public static string Security => "DPT-10";
}

public static class DesignationCode
{
    public static string HouseKeeper => "DES-01";
    public static string Waiter => "DES-02";
}

public static class BookingType
{
    public static string Room => "R";
    public static string Hall => "H";
}

public static class NotificationEventCode
{
    public static string RequsitionReviewNtf => "REQREVNTF";
    public static string RequsitionApproveNtf => "REQAPPNTF";
    public static string OrderReviewNtf => "ORDREVNTF";
    public static string OrderApproveNtf => "ORDAPPNTF";
    public static string ReceiveItemNtf => "RCVNTF";
    public static string IssueItemNtf => "ISUNTF";
    public static string BookingNtf => "BOKNTF";
    public static string CheckInNtf => "CKINNTF";
    public static string CheckOutNtf => "CKOUTNTF";
}

public static class AdvanceReportType
{
    public static string Reservation => "RESERVATION";
    public static string RoomAdvance => "ROOM ADVANCE";
    public static string HallReservation => "HALL RESERVATION";
    public static string HallAdvance => "HALL ADVANCE";
}

public static class RsTableName
{
    public static string RoomService => "ROOM SERVICE";
    public static string TableService => "TABLE SERVICE";
}

public static class UserCode
{
    public static string Administrator => "Administrator";
    public static string Development => "Development";
}

public static class HKTaskName
{
    public static string RoomCleaning => "Room Clean";
}