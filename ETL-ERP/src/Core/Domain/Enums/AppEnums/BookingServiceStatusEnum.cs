namespace Domain.Enums.AppEnums;

public enum BookingServiceStatusEnum
{
    Booked = 1,
    CheckIn,
    CheckOut,
    NoShow,
    Canceled = 9
}

public enum BookingReportStatusEnum
{
    Pending = 1,
    Approved,
    CheckIn,
    CheckOut,
    Canceled = 9
}

public enum OnlineBookingStatusEnum
{
    Pending = 0,
    Approved = 1,
    Canceled = 2
}

public enum BookingDayStatusEnum
{
    HalfDay = 1,
    DayUse = 2
}

public enum HallBookingStatusEnum
{
    Pending = 1,
    Booked,
    Completed,
    Canceled = 9
}

public enum HallBookingShiftEnum
{
    DayShift = 1,
    NightShift,
    Both,
}

public enum BookingConfirmEnum
{
    Waiting,
    Confirm
}