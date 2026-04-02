
using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.ClientCompany;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.HotelManagement;

public class ClientCompanyService : BaseService<ClientCompany>, IClientCompanyService
{
    #region Config
    private IClientCompanyRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public ClientCompanyService(IClientCompanyRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion


    #region Create

    public async Task<bool> AddAsync(ClientCompanyVm vm)
    {
        var Model = _iMapper.Map<ClientCompany>(vm);

        Model.ActionById = CurrentUserId;
        Model.ActionDate = Utility.GetBdDateTimeNow();

        await _iRepository.AddAsync(Model);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        return true;
    }
    #endregion

    public async Task<DataTablePagination<ClientCompanySearchVm, ClientCompanySearchVm>> SearchAsync(DataTablePagination<ClientCompanySearchVm, ClientCompanySearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

}
