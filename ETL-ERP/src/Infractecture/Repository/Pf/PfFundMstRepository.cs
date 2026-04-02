using AutoMapper;
using Domain.Entities.Pf;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfFund;
using Interface.Repository.Pf;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.Pf
{
    public class PfFundMstRepository : BaseRepository<PfFundMst>, IPfFundMstRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public PfFundMstRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }


        #endregion

        #region GetPfFundData

        public async Task<PfFundMstVm> GetPfFundData(PfFundMstVm vm)
        {
            if (vm == null) throw new Exception("No Data Found..!");

            if (!(vm.Year > 2000) && !(vm.Month > 0)) throw new Exception("Please Select Month And Year");

            DateTime firstDate = new DateTime(vm.Year, vm.Month, 1);
            DateTime lastDate = firstDate.AddMonths(1).AddSeconds(-1);

            var payroll = await Context.PrSalaryMsts
                .Include(c => c.PrSalaryDtls)
                .FirstOrDefaultAsync(c => c.Year == vm.Year && c.Month == vm.Month);

            if (payroll == null) throw new Exception("Payroll Not Created Yet");

            var pfSettings = Context.PfSettings.FirstOrDefault();

            if (pfSettings == null) throw new Exception("No Provident Fund Setup Found..!!");

            //var pfOpenEmp = Context.PfFundOpennings
            //    .Include(e => e.Employee)
            //        .ThenInclude(d => d.Designation)
            //    .Include(e => e.Employee)
            //        .ThenInclude(d => d.Department)
            //    .Where(c => firstDate <= c.PfStartDate.Date && lastDate >= c.PfStartDate.Date && !c.IsDeleted).ToList();

            var pfEmployees = Context.Employees
                .Include(d => d.Designation)
                .Include(d => d.Department)
                .Where(c => c.EmployeeStatus == (short)EmployeeStatusEnum.Permanent && c.ConfirmationDate != null && c.ConfirmationDate < firstDate).ToList();

            pfEmployees = pfEmployees.OrderBy(c => c.Designation.Code).ToList();

            var model = new PfFundMstVm();

            model.Year = vm.Year;
            model.Month = vm.Month;
            model.FundFromDate = firstDate;
            model.FundToDate = lastDate;
            model.EntryDate = vm.EntryDate;
            model.EmpConPer = pfSettings.EmpCon;
            model.CompConPer = pfSettings.CompCon;
            model.PrMstId = payroll.Id;

            var pfFundDtlList = new List<PfFundDtlVm>();

            if (pfEmployees != null && pfEmployees.Count > 0)
            {
                foreach (var employee in pfEmployees)
                {
                    var filterPayrollEmp = payroll.PrSalaryDtls.FirstOrDefault(c => c.EmployeeId == employee.Id);
                    if (filterPayrollEmp == null) continue;

                    var dtlModel = new PfFundDtlVm();

                    var perValue = filterPayrollEmp?.GrossSalary;
                    if (pfSettings.PfSource == "B")
                    {
                        perValue = filterPayrollEmp?.BasicSalary;
                    }

                    dtlModel.EmployeeId = employee.Id;
                    dtlModel.EmployeeName = employee.Name;
                    dtlModel.EmployeeCode = employee.Code;
                    dtlModel.EmpDepartment = employee.Department.Name;
                    dtlModel.EmpDesignation = employee.Designation.Name;
                    dtlModel.EmpCon = DU.Utility.PercentCalculation(pfSettings.EmpCon, perValue ?? 0);
                    dtlModel.CompCon = DU.Utility.PercentCalculation(pfSettings.CompCon, perValue ?? 0);

                    //var perValueInt = (dtlModel.EmpCon + pfSettings.CompCon);

                    //dtlModel.Interest = pfSettings.P > 0 ? DU.Utility.PercentCalculation(dtl.Interest, perValueInt) : DU.Utility.PercentCalculation(dtl.Interest, perValueInt);

                    dtlModel.TotalAmount = dtlModel.EmpCon + dtlModel.CompCon + dtlModel.Interest;
                    dtlModel.EmpSalary = filterPayrollEmp?.GrossSalary;

                    if (filterPayrollEmp?.IsPaid == true)
                    {
                        pfFundDtlList.Add(dtlModel);
                    }
                }
            }

            model.PfFundDtls = pfFundDtlList;

            if (pfFundDtlList.Count > 0)
            {
                model.TotalEmpCon = pfFundDtlList.Sum(c => c.EmpCon);
                model.TotalCompCon = pfFundDtlList.Sum(c => c.CompCon);
                model.Interest = pfFundDtlList.Sum(c => c.Interest);
                model.PfAmount = pfFundDtlList.Sum(c => c.TotalAmount);
            }

            return model;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<PfFundMstSearchVm, PfFundMstSearchVm>> SearchAsync(DataTablePagination<PfFundMstSearchVm, PfFundMstSearchVm> vm)
        {
            var searchResult = Context.PfFundMsts
                                      .Include(c => c.PrMst)
                                      .Include(c => c.PfFundDtls)
                                      .AsQueryable()
                                      .Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Attendance not found");

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.FundType.ToString().ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();

            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.FundToDate)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<PfFundMstSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                }
            }
            return vm;
        }

        #endregion

        #region GetPf

        public async Task<PfFundMst> GetPfByIdAsync(long id)
        {
            var data = await Context.PfFundMsts
                .Include(c => c.PfFundDtls)
                    .ThenInclude(e => e.Employee)
                        .ThenInclude(d => d.Designation)
                .Include(c => c.PfFundDtls)
                    .ThenInclude(e => e.Employee)
                        .ThenInclude(dp => dp.Department)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (data == null) throw new Exception("No Provident Fund Data Found");

            return data;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion
    }
}
