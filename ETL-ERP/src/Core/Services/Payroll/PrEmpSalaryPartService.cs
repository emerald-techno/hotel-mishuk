using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrEmpSalaryPart;
using Interface.Repository.Hr;
using Interface.Repository.Payroll;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Services.Base;
using System.Data;

namespace Services.Payroll;

public class PrEmpSalaryPartService : BaseService<PrEmpSalaryPart>, IPrEmpSalaryPartService
{
    #region Config

    private IPrEmpSalaryPartRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IEmployeeRepository _iEmployeeRepository;
    private readonly IPrSalaryPartRepository _iPrSalaryPartRepository;

    public PrEmpSalaryPartService(IPrEmpSalaryPartRepository iRepository,
        IMapper iMapper, IUnitOfWork iUnitOfWork,
        IEmployeeRepository iEmployeeRepository, IPrSalaryPartRepository iPrSalaryPartRepository) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iEmployeeRepository = iEmployeeRepository;
        _iPrSalaryPartRepository = iPrSalaryPartRepository;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<PrEmpSalaryPartSearchVm, PrEmpSalaryPartSearchVm>> SearchAsync(DataTablePagination<PrEmpSalaryPartSearchVm, PrEmpSalaryPartSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region GetEmployeePartDataListFromExcel

    private IEnumerable<EmpSalaryPartExcelModel> GetEmployeePartDataListFromExcel(IFormFile importFile)
    {
        if (importFile == null || importFile.Length <= 0) return null;
        var employeePartModels = new List<EmpSalaryPartExcelModel>();

        var dt = Utility.ConvertExcelOrCsVToDataTable(importFile);

        foreach (DataRow dataRow in dt.Rows)
        {
            var model = new EmpSalaryPartExcelModel
            {
                Name = dataRow["Name"].ToString().Trim(),
                Code = dataRow["Code"].ToString().Trim(),
                PartCode = dataRow["PartCode"].ToString().Trim(),
                Value = dataRow["Value"].ToString().Trim()
            };

            if (string.IsNullOrEmpty(model.Name)) continue;
            if (string.IsNullOrEmpty(model.Code)) continue;
            if (string.IsNullOrEmpty(model.PartCode)) continue;
            if (string.IsNullOrEmpty(model.Value)) continue;

            employeePartModels.Add(model);
        }

        return employeePartModels;
    }

    #endregion

    #region Import

    public async Task<bool> ImportAsync(IFormFile importFile)
    {
        if (importFile == null || importFile.Length <= 0) return false;
        var employeePartModels = GetEmployeePartDataListFromExcel(importFile);

        if (employeePartModels.Count() == 0)
            throw new Exception("No Data Found In Excel..!");

        var employeePartList = new List<PrEmpSalaryPart>();

        foreach (var part in employeePartModels)
        {
            var partModel = new PrEmpSalaryPart();

            partModel.ActionById = CurrentUserId;
            partModel.ActionDate = Utility.GetBdDateTimeNow();

            if (string.IsNullOrEmpty(part.Name))
                throw new Exception($"Name Not Found");

            if (string.IsNullOrEmpty(part.Code))
                throw new Exception($"{part.Name} Code Not Found");

            if (string.IsNullOrEmpty(part.PartCode))
                throw new Exception($"{part.PartCode} Part Code Not Found");

            var employee = await _iEmployeeRepository.GetFirstOrDefaultAsync(c => c.Code.Equals(part.Code));
            
            if (employee == null)
                throw new Exception($"{part.Name}-{part.Code} Employee Not Found");
            
            if (!employee.IsEnable)
                throw new Exception($"{employee.Name}-{employee.Code} Not Available..!!");

            var salaryPart = await _iPrSalaryPartRepository.GetFirstOrDefaultAsync(c => c.PartCode.Equals(part.PartCode));
            
            if (salaryPart == null)
                throw new Exception($"{part.PartCode} Part Not Found");
            
            if (!salaryPart.IsEnable)
                throw new Exception($"{salaryPart.PartName} Is Disable..!!");

            if (!salaryPart.IsEmpWise)
                throw new Exception($"{salaryPart.PartName} Is Not Set To Employee Wise..!!");

            if (!string.IsNullOrEmpty(salaryPart.PartLink))
                throw new Exception($"{salaryPart.PartName} Is Fixed Part & Auto Value System..!!");

            partModel.SalaryPartId = salaryPart.Id;
            partModel.EmployeeId = employee.Id;
            partModel.PartType = salaryPart.PartType;
            partModel.ValueType = salaryPart.ValueType;
            partModel.Value = Convert.ToDouble(part.Value);

            var existEmpPart = Repository.GetFirstOrDefault(c => c.EmployeeId == partModel.EmployeeId && c.SalaryPartId == partModel.SalaryPartId && !c.IsDeleted);

            if (existEmpPart != null) continue;

            employeePartList.Add(partModel);
        }

        //var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await Repository.AddRangeAsync(employeePartList);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) return false;

        //ts.Complete();
        return true;

    }

    #endregion
}
