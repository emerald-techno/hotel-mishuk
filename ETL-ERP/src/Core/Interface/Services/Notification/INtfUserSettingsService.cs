using Domain.Entities.Notification;
using Domain.Utility.Common;
using Domain.ViewModel.Notification;
using Interface.Base;

namespace Interface.Services.Notification;

public interface INtfUserSettingsService : IService<NtfUserSettings>
{
    Task<DataTablePagination<NtfUserSettingSearchVm, NtfUserSettingSearchVm>>
                  SearchAsync(DataTablePagination<NtfUserSettingSearchVm, NtfUserSettingSearchVm> model);
}