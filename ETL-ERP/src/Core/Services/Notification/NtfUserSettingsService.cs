using AutoMapper;
using Domain.Entities.Notification;
using Domain.Utility.Common;
using Domain.ViewModel.Notification;
using Interface.Repository.Notification;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Microsoft.Extensions.Logging;
using Services.Base;

namespace Services.Notification;

public class NtfUserSettingsService : BaseService<NtfUserSettings>, INtfUserSettingsService
{
    #region Config

    private INtfUserSettingsRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly ILogger<NtfNotificationMsgService> _iLogger;

    public NtfUserSettingsService(INtfUserSettingsRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        ILogger<NtfNotificationMsgService> iLogger) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iLogger = iLogger;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<NtfUserSettingSearchVm, NtfUserSettingSearchVm>>
        SearchAsync(DataTablePagination<NtfUserSettingSearchVm, NtfUserSettingSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion
}
