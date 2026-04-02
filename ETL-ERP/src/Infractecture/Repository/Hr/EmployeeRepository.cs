using AutoMapper;
using Domain.Entities.HR;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Hr.Employees;
using Interface.Repository.Hr;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Hr
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository, IDisposable
    {
        #region Config

        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public EmployeeRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<EmployeeSearchVm, EmployeeSearchVm>> SearchAsync(DataTablePagination<EmployeeSearchVm, EmployeeSearchVm> vm)
        {
            var searchResult = Context.Employees
                .Include(d => d.Department)
                .Include(a => a.AcademicDpt)
                .Include(ds => ds.Designation)
                .AsQueryable()
                .Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Employee not found");

            /* For Advance Search */

            if (!string.IsNullOrEmpty(model.Name))
            {
                searchResult = searchResult.Where(c => c.Name == model.Name);
            }
            if (!string.IsNullOrEmpty(model.Code))
            {
                searchResult = searchResult.Where(c => c.Code == model.Code);
            }
            if (!string.IsNullOrEmpty(model.Email))
            {
                searchResult = searchResult.Where(c => c.Email == model.Email);
            }
            if (!string.IsNullOrEmpty(model.Mobile))
            {
                searchResult = searchResult.Where(c => c.Mobile == model.Mobile);
            }

            if (model.DesignationId > 0)
            {
                searchResult = searchResult.Where(c => c.DesignationId == model.DesignationId);
            }

            if (model.DepartmentId > 0)
            {
                searchResult = searchResult.Where(c => c.DepartmentId == model.DepartmentId);
            }

            if (model.AcademicDptId > 0)
            {
                searchResult = searchResult.Where(c => c.AcademicDptId == model.AcademicDptId);
            }

            if (model.IsTeacher)
            {
                var designationIds = Context.Designations.Where(c => c.IsTeacher && !c.IsDeleted).Select(c => c.Id).ToList();
                searchResult = searchResult.Where(c => designationIds.Contains(c.DesignationId));
            }

            if (model.EnableStatus > 0)
            {
                if (model.EnableStatus == (short)EmployeeIsEnableEnum.Current)
                {
                    searchResult = searchResult.Where(c => c.IsEnable);
                }
                else if (model.EnableStatus == (short)EmployeeIsEnableEnum.Disable)
                {
                    searchResult = searchResult.Where(c => !c.IsEnable);
                }
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Name.ToLower().Contains(value) || c.Code.ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.Designation.Code)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<EmployeeSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.DesignationName = filterData?.Designation?.Name;
                    searchDto.DepartmentName = filterData?.Department?.Name;
                    searchDto.AcademicDepartmentName = filterData?.AcademicDpt?.Name;
                }
            }
            return vm;
        }

        #endregion

        #region SearchAll

        public async Task<DataTablePagination<EmployeeSearchVm, EmployeeSearchVm>> SearchAllAsync(DataTablePagination<EmployeeSearchVm, EmployeeSearchVm> vm)
        {
            var searchResult = Context.Employees
                .Include(d => d.Department)
                .Include(a => a.AcademicDpt)
                .Include(ds => ds.Designation)
                .AsQueryable()
                .Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Employee not found");

            /* For Advance Search */

            if (model.DesignationId > 0)
            {
                searchResult = searchResult.Where(c => c.DesignationId == model.DesignationId);
            }

            if (model.DepartmentId > 0)
            {
                searchResult = searchResult.Where(c => c.DepartmentId == model.DepartmentId);
            }

            if (model.AcademicDptId > 0)
            {
                searchResult = searchResult.Where(c => c.AcademicDptId == model.AcademicDptId);
            }

            if (model.EnableStatus > 0)
            {
                if (model.EnableStatus == (short)EmployeeIsEnableEnum.Current)
                {
                    searchResult = searchResult.Where(c => c.IsEnable);
                }
                else if (model.EnableStatus == (short)EmployeeIsEnableEnum.Disable)
                {
                    searchResult = searchResult.Where(c => !c.IsEnable);
                }
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Name.ToLower().Contains(value) || c.Code.ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.Designation.Code)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<EmployeeSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.DesignationName = filterData?.Designation?.Name;
                    searchDto.DepartmentName = filterData?.Department?.Name;
                    searchDto.AcademicDepartmentName = filterData?.AcademicDpt?.Name;
                }
            }
            return vm;
        }

        #endregion

        #region GetEmployee

        public async Task<EmployeeVm> GetEmployeeByIdAsync(long id)
        {
            var dataModel = await Context.Employees
                .Include(c => c.Designation)
                .Include(c => c.Department)
                .Include(a => a.AcademicDpt)
                .Include(a => a.DutyShift)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            var model = _iMapper.Map<EmployeeVm>(dataModel);
            model.AcademicDepartmentName = dataModel?.AcademicDpt?.Name;
            model.DepartmentName = dataModel?.Department?.Name;
            model.DesignationName = dataModel?.Designation?.Name;
            model.PermanentDutyShiftName = dataModel?.DutyShift?.ShiftName;

            return model;
        }

        #endregion

        #region GetEmpIdByUser
        public async Task<long> GetEmpIdByUserId(long userId)
        {
            if (userId > 0 is false)
                throw new ArgumentException("User Id Not Found...!");

            var employee = await Context.Employees.FirstOrDefaultAsync(c => c.UserId == userId);

            if (employee == null) throw new Exception("Employee Not Found For This User");

            return employee.Id;
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
