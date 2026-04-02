using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Complementary;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IComplementaryService : IService<HtComplementary>
{
    Task<DataTablePagination<HtComplementarySearchVm, HtComplementarySearchVm>>
        SearchAsync(DataTablePagination<HtComplementarySearchVm, HtComplementarySearchVm> model);
}