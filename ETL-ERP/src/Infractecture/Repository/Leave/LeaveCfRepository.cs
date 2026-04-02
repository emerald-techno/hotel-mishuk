using AutoMapper;
using Domain.Entities.Leave;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveCf;
using Interface.Repository.Leave;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.ContextModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.Leave
{
    public class LeaveCfRepository : BaseRepository<LeaveCf>, ILeaveCfRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;
        private readonly ILogger<LeaveCfRepository> _logger;

        public LeaveCfRepository(ApplicationDbContext db, IMapper iMapper, ILogger<LeaveCfRepository> logger) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
            _logger = logger;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<LeaveCfSearchVm, LeaveCfSearchVm>>
            SearchAsync(DataTablePagination<LeaveCfSearchVm, LeaveCfSearchVm> vm)
        {
            var searchResult = Context.LeaveCfs
                .Include(c => c.Employee)
                    .ThenInclude(d => d.Designation)
                .Include(c => c.Employee)
                    .ThenInclude(d => d.Department)
                .Include(l => l.LeaveType)
                .AsQueryable().Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Leave carry forward not found");

            if (model.EmployeeId > 0)
            {
                searchResult = searchResult.Where(c => c.EmployeeId == model.EmployeeId);
            }

            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.LeaveType)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<LeaveCfSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.EmployeeName = filterData?.Employee.Name;
                    searchDto.EmployeeCode = filterData?.Employee.Code;
                    searchDto.DesignationName = filterData?.Employee.Designation.Name;
                    searchDto.DepartmentName = filterData?.Employee.Department.Name;
                    searchDto.LeaveTypeName = filterData?.LeaveType.TypeName;
                }
            }
            return vm;
        }

        #endregion

        public async Task<bool> CalculateDailyCf()
        {
            var isUpdated = false;
            var currentYear = DateTime.Now.Year;
            var currenyMonth = DateTime.Now.Month;
            DateTime firstDay = new DateTime(DateTime.Now.Year, 1, 1);

            var empCfList = await Context.LeaveCfs
                                    .Include(e => e.Employee)
                                    .Where(c => c.LeaveYear == currentYear && c.LeaveTypeId == (short)LeaveTypeEnum.EarnedLeave && !c.IsDeleted)
                                    .ToListAsync();

            var updateCfList = new List<LeaveCf>();

            if (empCfList.Count > 0)
            {
                var totalDays = DU.AppUtility.DaysDiffernce(DateTime.Today, firstDay);
                double perDayLeave = totalDays / 12;
                var leaveBalance = Math.Round(perDayLeave, MidpointRounding.ToEven);

                foreach (var cf in empCfList)
                {
                    if (cf.Employee.JoinDate > firstDay)
                    {
                        var newTotalDays = DU.AppUtility.DaysDiffernce(DateTime.Today, cf.Employee.JoinDate);
                        double newPerDayLeave = totalDays / 12;
                        cf.LeaveBalance = (int)Math.Round(newPerDayLeave, MidpointRounding.ToEven);
                    }
                    else
                    {
                        cf.LeaveBalance = (int)leaveBalance;
                    }

                    updateCfList.Add(cf);
                }

                Context.LeaveCfs.UpdateRange(updateCfList);
                isUpdated = await Context.SaveChangesAsync() > 0;
            }

            return isUpdated;
        }

        public async Task<bool> SetupNewYearCf()
        {
            var isAdded = false;
            var currentDate = DateTime.Now;
            int previousYear = currentDate.Year - 1;

            var employeeList = await Context.Employees.Where(c => c.IsEnable && !c.IsDeleted).ToListAsync();

            var empCfList = await Context.LeaveCfs
                                    .Where(c => c.LeaveYear == previousYear && c.LeaveTypeId == (short)LeaveTypeEnum.EarnedLeave && !c.IsDeleted)
                                    .ToListAsync();

            var addCfList = new List<LeaveCf>();

            if (employeeList.Count > 0)
            {
                foreach (var emp in employeeList)
                {
                    var prevYearCf = empCfList.FirstOrDefault(c => c.EmployeeId == emp.Id);
                    if (prevYearCf == null)
                    {
                        //_logger.LogInformation($"{emp.Name}-{emp.Code} Cf Not Added...!");
                        continue;
                    }
                    var model = new LeaveCf();
                    model.EmployeeId = emp.Id;
                    model.LeaveTypeId = prevYearCf.LeaveTypeId;
                    model.LeaveYear = currentDate.Year;
                    model.LeaveBalance = 0;
                    model.CfBalance = prevYearCf.LeaveBalance + prevYearCf.CfBalance - prevYearCf.LeaveEnjoyed;
                    model.LeaveEnjoyed = 0;
                    model.LeaveSale = prevYearCf.LeaveSale;
                    model.ActionDate = DateTime.Now;
                    model.ActionById = prevYearCf.ActionById;

                    var existCf = Context.LeaveCfs.FirstOrDefault(c => c.EmployeeId == model.EmployeeId && c.LeaveYear == model.LeaveYear);
                    if (existCf != null) continue;

                    addCfList.Add(model);
                }
            }

            Context.LeaveCfs.AddRange(addCfList);
            isAdded = await Context.SaveChangesAsync() > 0;

            return isAdded;
        }

        #region Dispose

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion
    }
}
