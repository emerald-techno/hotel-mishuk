using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.SetHoliday;
using Interface.Base;

namespace Interface.Repository.Admin
{
    public interface ISetHolidayRepository : IRepository<SetHoliday>
    {
        Task<DataTablePagination<SetHolidaySearchVm, SetHolidaySearchVm>>
            SearchAsync(DataTablePagination<SetHolidaySearchVm, SetHolidaySearchVm> model);
        Task<bool> IsHoliday(DateTime date);
    }
}
