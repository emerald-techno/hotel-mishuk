using AutoMapper;
using Domain.Entities.Pf;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfFund;
using Domain.ViewModel.Report;
using Interface.Repository.Pf;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Pf
{
    public class PfFundDtlRepository : BaseRepository<PfFundDtl>, IPfFundDtlRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public PfFundDtlRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }


        #endregion

        #region Search

        public async Task<DataTablePagination<EmployeePfSummaryReportVm, EmployeePfSummaryReportVm>> SearchAsync(DataTablePagination<EmployeePfSummaryReportVm, EmployeePfSummaryReportVm> vm)
        {
            var searchResult = Context.PfFundDtls
                                      .Include(c => c.FundMst)
                                      .Include(c => c.Employee)
                                        .ThenInclude(d => d.Designation)
                                      .Include(c => c.Employee)
                                        .ThenInclude(d => d.Department)
                                      .AsQueryable()
                                      .Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Attendance not found");

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Employee.Name.ToString().ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();

            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var totalData = await searchResult.ToListAsync();

                var data = totalData.DistinctBy(c => c.EmployeeId)
                                             .OrderBy(c => c.Employee.Designation.Code)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToList();

                var dataList = new List<EmployeePfSummaryReportVm>();

                var sl = vm.Start;

                foreach (var dtl in data)
                {
                    var searchDto = new EmployeePfSummaryReportVm();
                    var empFundOpen = Context.PfFundOpennings.FirstOrDefault(c => c.EmployeeId == dtl.EmployeeId);

                    searchDto.SerialNo = ++sl;
                    searchDto.EmployeeId = dtl.EmployeeId;
                    searchDto.EmployeeName = dtl.Employee.Name;
                    searchDto.EmployeeCode = dtl.Employee.Code;
                    searchDto.EmpDesignation = dtl.Employee.Designation.Name;
                    searchDto.EmpDepartment = dtl.Employee.Department.Name;
                    searchDto.TotalEmpCon = totalData.Where(c => c.EmployeeId == dtl.EmployeeId).Sum(c => c.EmpCon);
                    searchDto.TotalCompCon = totalData.Where(c => c.EmployeeId == dtl.EmployeeId).Sum(c => c.CompCon);
                    searchDto.TotalInterest = totalData.Where(c => c.EmployeeId == dtl.EmployeeId).Sum(c => c.Interest);
                    searchDto.TotalPfAmount = totalData.Where(c => c.EmployeeId == dtl.EmployeeId).Sum(c => c.TotalAmount);

                    if (empFundOpen != null)
                    {
                        searchDto.TotalEmpCon = searchDto.TotalEmpCon + empFundOpen.EmpCon;
                        searchDto.TotalCompCon = searchDto.TotalCompCon + empFundOpen.CompCon;
                        searchDto.TotalInterest = searchDto.TotalInterest + empFundOpen.Interest;
                        searchDto.TotalPfAmount = searchDto.TotalPfAmount + empFundOpen.TotalAmount;
                    }

                    dataList.Add(searchDto);
                }

                vm.data = dataList;
            }

            return vm;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion

        #region GetEmployeePfInfo

        public async Task<EmpPfSummaryDtlReportVm> GetEmployeePfInfo(long employeeId)
        {
            if (employeeId == 0) throw new Exception("Employee Not Found..!");
            var employee = await Context.Employees
                .Include(d => d.Designation)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(c => c.Id == employeeId && c.EmployeeStatus == (short)EmployeeStatusEnum.Permanent && !c.IsDeleted);

            if (employee == null) throw new Exception("Eligable Employee Not Found..!");

            var fundOpen = await Context.PfFundOpennings.FirstOrDefaultAsync(c => c.EmployeeId == employeeId && !c.IsDeleted);

            var model = new EmpPfSummaryDtlReportVm();

            model.EmployeeId = employee.Id;
            model.EmployeeName = employee.Name;
            model.EmployeeCode = employee.Code;
            model.EmpDepartment = employee.Department.Name;
            model.EmpDesignation = employee.Designation.Name;

            var dtlDataList = Context.PfFundDtls.Where(c => c.EmployeeId == employeeId && !c.IsDeleted).ToList();
            model.PfFundDtlVms = _iMapper.Map<List<PfFundDtlVm>>(dtlDataList);

            model.EmpCon = model.PfFundDtlVms.Sum(c => c.EmpCon);
            model.CompCon = model.PfFundDtlVms.Sum(c => c.CompCon);
            model.Interest = model.PfFundDtlVms.Sum(c => c.Interest);
            model.TotalAmount = model.PfFundDtlVms.Sum(c => c.TotalAmount);

            if (fundOpen != null)
            {
                model.EmpCon = model.EmpCon + fundOpen.EmpCon;
                model.CompCon = model.CompCon + fundOpen.CompCon;
                model.Interest = model.Interest + fundOpen.Interest;
                model.TotalAmount = model.TotalAmount + fundOpen.TotalAmount;
            }

            return model;
        }

        #endregion
    }
}
