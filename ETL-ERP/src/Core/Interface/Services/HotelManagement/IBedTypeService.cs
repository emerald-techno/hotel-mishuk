using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.BedType;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IBedTypeService : IService<HtBedType>
{
    Task<DataTablePagination<HtBedTypeSearchVm, HtBedTypeSearchVm>>
        SearchAsync(DataTablePagination<HtBedTypeSearchVm, HtBedTypeSearchVm> model);
}