using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Complementary;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IComplementaryRepository : IRepository<HtComplementary>
{
    Task<DataTablePagination<HtComplementarySearchVm, HtComplementarySearchVm>>
       SearchAsync(DataTablePagination<HtComplementarySearchVm, HtComplementarySearchVm> vm);
}