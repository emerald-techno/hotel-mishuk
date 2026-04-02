using AutoMapper;
using Domain.Entities.Pf;
using Interface.Repository.Pf;
using Interface.Services.Pf;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Pf;

public class EmpLoanDtlService : BaseService<EmpLoanDtl>, IEmpLoanDtlService
{
    #region Config
    private readonly IEmpLoanDtlRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public EmpLoanDtlService(IEmpLoanDtlRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }
    #endregion
}
