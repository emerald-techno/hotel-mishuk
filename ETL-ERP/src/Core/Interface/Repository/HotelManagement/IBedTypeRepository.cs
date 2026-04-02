using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.BedType;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IBedTypeRepository : IRepository<HtBedType>
{
    Task<DataTablePagination<HtBedTypeSearchVm, HtBedTypeSearchVm>>
       SearchAsync(DataTablePagination<HtBedTypeSearchVm, HtBedTypeSearchVm> vm);
}
