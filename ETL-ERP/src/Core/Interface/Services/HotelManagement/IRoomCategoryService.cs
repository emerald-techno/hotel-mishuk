using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomCategory;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IRoomCategoryService : IService<HtRoomCategory>
{
    Task<bool> RoomCategoryAddAsync(HtRoomCategoryVm vm);

    Task<DataTablePagination<HtRoomCategorySearchVm, HtRoomCategorySearchVm>>
        SearchAsync(DataTablePagination<HtRoomCategorySearchVm, HtRoomCategorySearchVm> model);
    Task<List<HtRoomCategory>> GetRoomCategoryByRoomAvailibity(DateTime? arrivedDate, DateTime? depatureDate, short? roomCount = 1);
    Task<List<HtRoomCategoryVm>> RoomCategoryPublicData();
    //Task<List<HtRoomCategoryDiscountSetUpVm>> DiscountSetUP();
    Task<List<HtRoomCategoryDiscountSetUpVm>> GetDiscountedCategories(DateTime fromDate, DateTime toDate, DiscountTypeEnum discountType, double discountAmount);
}
