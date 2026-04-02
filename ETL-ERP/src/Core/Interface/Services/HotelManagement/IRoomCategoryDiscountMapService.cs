using Domain.Entities.HotelManagement;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IRoomCategoryDiscountMapService : IService<HtRoomCategoryDiscountMap>
{
    Task<bool> SubmitDiscountSetupDataAsync(List<HtRoomCategoryDiscountMap> discountSetupList);
}
