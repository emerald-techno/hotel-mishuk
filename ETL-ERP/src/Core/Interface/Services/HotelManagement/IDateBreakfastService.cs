using Domain.Entities.HotelManagement;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IDateBreakfastService : IService<HtDateBreakfast>
{
    Task<bool> TodayCbfAdded();
    Task<bool> CheckCbfByDate(DateTime cbfDate);
}