namespace Domain.Enums.AppEnums;

public enum RsOrderStatusEnum
{
    Pending,
    Served,
    Canceled
}
public enum RsOrderPaymentStatusEnum
{
    Pending,
    PartialPayment,
    FullPayment,
    Refund
}

public enum RsOrderTypeEnum
{
    Order,
    Reservation
}

public enum RsOrderPaymentTypeEnum
{
    Receive,
    Refund
}