using System.ComponentModel;

namespace Domain.Enums.AppEnums
{
    public enum StockStatusEnum
    {
        [Description("In-Stock")]
        InStock = 1,
        [Description("Out-of-Stock")]
        OutOfStock
    }
}
