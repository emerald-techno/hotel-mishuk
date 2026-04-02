using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrSalaryPart;
using Interface.Repository.Payroll;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Payroll;

public class PrSalaryPartService : BaseService<PrSalaryPart>, IPrSalaryPartService
{
    #region Config

    private IPrSalaryPartRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public PrSalaryPartService(IPrSalaryPartRepository iRepository,
        IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<PrSalaryPartSearchVm, PrSalaryPartSearchVm>>
        SearchAsync(DataTablePagination<PrSalaryPartSearchVm, PrSalaryPartSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion
}
