using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.SetHoliday;
using Interface.Base;

namespace Interface.Services.Admin
{
    public interface ISetHolidayService : IService<SetHoliday>
    {
        Task<DataTablePagination<SetHolidaySearchVm, SetHolidaySearchVm>>
                                    SearchAsync(DataTablePagination<SetHolidaySearchVm, SetHolidaySearchVm> model);
    }
}
