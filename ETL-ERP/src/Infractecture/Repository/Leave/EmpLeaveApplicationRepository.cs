using AutoMapper;
using Domain.Entities.Leave;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.EmpLeaveApplication;
using Interface.Repository.Leave;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.Leave
{
    public class EmpLeaveApplicationRepository : BaseRepository<EmpLeaveApplication>, IEmpLeaveApplicationRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public EmpLeaveApplicationRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<EmpLeaveApplicationSearchVm, EmpLeaveApplicationSearchVm>>
            SearchAsync(DataTablePagination<EmpLeaveApplicationSearchVm, EmpLeaveApplicationSearchVm> vm)
        {
            var searchResult = Context.EmpLeaveApplications
                .Include(c => c.Employee)
                .Include(c => c.LeaveType)
                .Include(c => c.SubmitBy)
                .AsQueryable()
                .Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Department Leave Revierws not found");

            if (model.EmployeeId > 0)
            {
                searchResult = searchResult.Where(c => c.EmployeeId == model.EmployeeId);
            }

            if (model.LeaveTypeId > 0)
            {
                searchResult = searchResult.Where(c => c.LeaveTypeId == model.LeaveTypeId);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Employee.Name.ToLower().Contains(value));
            }
            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.SubmitDate)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<EmpLeaveApplicationSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.EmployeeName = filterData?.Employee.Name;
                    searchDto.LeaveTypeName = filterData?.LeaveType.TypeName;
                    searchDto.SubmitByName = filterData?.SubmitBy.FullName;
                }
            }
            return vm;
        }

        #endregion

        #region GetEmployeeLeaveApplication

        public async Task<EmpLeaveApplicationVm> GetEmployeeAppByIdAsync(long id)
        {
            var dataModel = await Context.EmpLeaveApplications
                .Include(c => c.Employee)
                .Include(c => c.LeaveType)
                .Include(c => c.SubmitBy)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (dataModel == null) throw new Exception("Application Not Found...!!");

            var model = _iMapper.Map<EmpLeaveApplicationVm>(dataModel);

            model.EmployeeName = dataModel?.Employee?.Name;
            model.EmployeeCode = dataModel?.Employee?.Code;
            model.EmpPhotoUrl = dataModel?.Employee?.PhotoUrl;
            model.LeaveTypeName = dataModel?.LeaveType?.TypeName;
            model.LeaveTypeBalance = dataModel?.LeaveType?.Balance;

            var takenLeave = await Context.EmpLeaveApplications.Where(c => c.EmployeeId == dataModel.EmployeeId && c.ToDate.Year == dataModel.ToDate.Year && c.Status == (short)EmpLeaveAppStatusEnum.FINALAPPROVE && !c.IsDeleted).ToListAsync();

            model.LeaveTaken = takenLeave.Sum(c => c.TotalApprovalLeave);

            return model;
        }

        #endregion

        #region GetLeaveReport

        public async Task<List<EmpLeaveReportVm>> GetLeaveReportData(EmpLeaveReportVm vm)
        {
            var searchResult = Context.EmpLeaveApplications
                .Include(c => c.Employee)
                    .ThenInclude(d => d.Designation)
                .Include(c => c.Employee)
                    .ThenInclude(dp => dp.Department)
                .Include(c => c.LeaveType)
                .AsQueryable().Where(c => !c.IsDeleted && c.Status == (short)EmpLeaveAppStatusEnum.FINALAPPROVE);

            if (vm == null) throw new Exception("Employee Leave Application not found");

            if (!string.IsNullOrEmpty(vm.FormDateStr))
            {
                var formDate = !string.IsNullOrEmpty(vm.FormDateStr) ? DU.Utility.ConvertStrToDate(vm.FormDateStr) : null;
                searchResult = searchResult.Where(c => c.FromDate >= formDate);
            }

            if (!string.IsNullOrEmpty(vm.ToDateStr))
            {
                var toDate = !string.IsNullOrEmpty(vm.ToDateStr) ? DU.Utility.ConvertStrToDate(vm.ToDateStr) : null;
                searchResult = searchResult.Where(c => c.FromDate <= toDate);
            }
            else
            {
                searchResult = searchResult.Where(c => c.FromDate <= DateTime.Now);
            }
            if (vm.EmpDepatmentId > 0)
            {
                searchResult = searchResult.Where(c => c.Employee.DepartmentId == vm.EmpDepatmentId);
            }

            var data = await searchResult.OrderBy(c => c.FromDate).ToListAsync();

            var employeeData = data.DistinctBy(c => c.EmployeeId).ToList();

            var dataList = new List<EmpLeaveReportVm>();

            if (employeeData != null && employeeData.Count > 0)
            {
                foreach (var emp in employeeData)
                {
                    var model = new EmpLeaveReportVm();

                    model.EmployeeId = emp.EmployeeId;
                    model.EmployeeName = emp.Employee.Name;
                    model.EmployeeCode = emp.Employee.Code;
                    model.EmpJoinDate = emp.Employee.JoinDate;
                    model.EmpDesignation = emp.Employee.Designation.Name;
                    model.EmpDepartment = emp.Employee.Department.Name;
                    model.MaturnityLeave = data.Where(c => c.EmployeeId == emp.EmployeeId && c.LeaveTypeId == (long)LeaveTypeEnum.MaternityLeave).Sum(c => c.TotalApprovalLeave);
                    model.CasualLeave = data.Where(c => c.EmployeeId == emp.EmployeeId && c.LeaveTypeId == (long)LeaveTypeEnum.CasualLeave).Sum(c => c.TotalApprovalLeave);
                    model.MedicalLeave = data.Where(c => c.EmployeeId == emp.EmployeeId && c.LeaveTypeId == (long)LeaveTypeEnum.MedicalLeave).Sum(c => c.TotalApprovalLeave);
                    model.EarnLeave = data.Where(c => c.EmployeeId == emp.EmployeeId && c.LeaveTypeId == (long)LeaveTypeEnum.EarnedLeave).Sum(c => c.TotalApprovalLeave);
                    model.LeaveWithoutPay = data.Where(c => c.EmployeeId == emp.EmployeeId && c.LeaveTypeId == (long)LeaveTypeEnum.LeaveWithoutPay).Sum(c => c.TotalApprovalLeave);
                    model.StudyLeave = data.Where(c => c.EmployeeId == emp.EmployeeId && c.LeaveTypeId == (long)LeaveTypeEnum.StudyLeave).Sum(c => c.TotalApprovalLeave);
                    model.TotalLeave = data.Where(c => c.EmployeeId == emp.EmployeeId).Sum(c => c.TotalApprovalLeave);

                    dataList.Add(model);
                }
            }

            return dataList;

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
