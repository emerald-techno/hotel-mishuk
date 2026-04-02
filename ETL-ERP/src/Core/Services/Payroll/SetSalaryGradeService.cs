using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.SetSalaryGrade;
using Interface.Repository.Payroll;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Payroll;

public class SetSalaryGradeService : BaseService<SetSalaryGrade>, ISetSalaryGradeService
{
    #region Config
    private ISetSalaryGradeRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public SetSalaryGradeService(ISetSalaryGradeRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
        : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }
    #endregion

    #region Search

    public async Task<DataTablePagination<SetSalaryGradeSearchVm, SetSalaryGradeSearchVm>> SearchAsync(DataTablePagination<SetSalaryGradeSearchVm, SetSalaryGradeSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion
}
