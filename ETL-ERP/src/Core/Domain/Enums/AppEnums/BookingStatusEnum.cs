using System.ComponentModel;

namespace Domain.Enums.AppEnums;

public enum BookingStatusEnum
{
    Available,
    Booked
}

public enum PayModeEnum
{
    [Description("Cash")]
    Cash,
    [Description("Cheque")]
    Bank,
    [Description("M-Banking")]
    Bkash,
    [Description("Card")]
    Card
}