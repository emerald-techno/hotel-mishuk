using AutoMapper;
using Domain.Entities;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrEmpSalaryPart;
using Interface.Repository.Payroll;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Payroll
{
    public class PrEmpSalaryPartRepository : BaseRepository<PrEmpSalaryPart>, IPrEmpSalaryPartRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public PrEmpSalaryPartRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<PrEmpSalaryPartSearchVm, PrEmpSalaryPartSearchVm>> SearchAsync(DataTablePagination<PrEmpSalaryPartSearchVm, PrEmpSalaryPartSearchVm> vm)
        {
            var searchResult = Context.PrEmpSalaryParts
                .Include(s => s.SalaryPart)
                .Include(c => c.Employee)
                    .ThenInclude(d => d.Department)
                .Include(c => c.Employee)
                    .ThenInclude(d => d.Designation)
                .AsQueryable().Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Employee Parts not found");

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Employee.Name.ToLower().Contains(value) || c.Employee.Code.ToLower().Contains(value));
            }

            var resultData = await searchResult.ToListAsync();

            var result = resultData.GroupBy(c => c.EmployeeId).Select(x => x).ToList();

            var totalRecords = result.Count();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = resultData.GroupBy(c => c.EmployeeId)
                                             .Select(c => new PrEmpSalaryPartSearchVm { EmployeeId = c.Key, EmpPartCount = c.ToList().Count })
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToList();

                vm.data = _iMapper.Map<List<PrEmpSalaryPartSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = resultData.Where(c => c.EmployeeId == searchDto.EmployeeId).FirstOrDefault();

                    searchDto.EmployeeName = filterData?.Employee.Name;
                    searchDto.EmployeeCode = filterData?.Employee.Code;
                    searchDto.EmpDepartment = filterData?.Employee?.Department?.Name;
                    searchDto.EmpDesignation = filterData?.Employee?.Designation?.Name;
                    searchDto.EmpDesignationCode = filterData?.Employee?.Designation?.Code;
                    searchDto.SalaryPartsName = string.Join(",", resultData.Where(c => c.EmployeeId == searchDto.EmployeeId).Select(s => s.SalaryPart.PartName).ToList());
                }

                vm.data = vm.data.OrderBy(c => c.EmpDesignationCode).ToList();

                foreach (var item in vm.data)
                {
                    item.SerialNo = ++sl;
                }
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
    }
}
