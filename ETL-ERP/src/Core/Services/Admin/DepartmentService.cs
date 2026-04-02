using AutoMapper;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.Department;
using Interface.Repository.Admin;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Admin;

public class DepartmentService : BaseService<Department>, IDepartmentService
{
    #region Config
    private IDepartmentRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public DepartmentService(IDepartmentRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
        : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }
    #endregion

    #region Search

    public async Task<DataTablePagination<DepartmentSearchVm, DepartmentSearchVm>>
        SearchAsync(DataTablePagination<DepartmentSearchVm, DepartmentSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region GetLedgerCodeByDptCode

    public string GetLedgerCodeByDptCode(string code)
    {
        var ledgerCode = "";

        if (!string.IsNullOrEmpty(code))
        {
            if(code == DepartmentCode.HotelMishukAdmin || code == DepartmentCode.FrontDesk 
                || code == DepartmentCode.Accounting || code == DepartmentCode.HouseKeeper
                || code == DepartmentCode.HR || code == DepartmentCode.Store || code == DepartmentCode.Security)
            {
                ledgerCode = AccLadgerCode.HotelMisuk;
            }
            else if (code == DepartmentCode.Resturant)
            {
                ledgerCode = AccLadgerCode.RestaurantLedger;
            }
            else if (code == DepartmentCode.StaffKitchen)
            {
                ledgerCode = AccLadgerCode.StaffKitchenLedger;
            }
            else if (code == DepartmentCode.AmariResort)
            {
                ledgerCode = AccLadgerCode.AmariResortLedger;
            }
        }

        return ledgerCode;
    }

    #endregion
}
