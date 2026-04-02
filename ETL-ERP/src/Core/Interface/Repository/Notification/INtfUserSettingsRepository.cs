using Domain.Entities.Notification;
using Domain.Utility.Common;
using Domain.ViewModel.Notification;
using Interface.Base;

namespace Interface.Repository.Notification;

public interface INtfUserSettingsRepository : IRepository<NtfUserSettings>
{
    Task<DataTablePagination<NtfUserSettingSearchVm, NtfUserSettingSearchVm>>
        SearchAsync(DataTablePagination<NtfUserSettingSearchVm, NtfUserSettingSearchVm> vm);
}
