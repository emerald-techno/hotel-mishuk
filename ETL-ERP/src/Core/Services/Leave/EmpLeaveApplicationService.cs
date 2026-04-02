using AutoMapper;
using Domain.Entities.Attendance;
using Domain.Entities.Leave;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.EmpLeaveApplication;
using Domain.ViewModel.Leave.LvAppReviewer;
using Interface.Repository.Attendance;
using Interface.Repository.Common;
using Interface.Repository.Hr;
using Interface.Repository.Leave;
using Interface.Services.Leave;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.Leave
{
    public class EmpLeaveApplicationService : BaseService<EmpLeaveApplication>, IEmpLeaveApplicationService
    {
        #region Config
        private readonly IEmpLeaveApplicationRepository Repository;
        private readonly IEmpAttendanceRepository _iEmpAttendanceRepository;
        private readonly IEmployeeRepository _iEmployeeRepository;
        private readonly ILeaveTypeRepository _iLeaveTypeRepository;
        private readonly ILeaveSetupRepository _iLeaveSetupRepository;
        private readonly ILeaveCfRepository _iLeaveCfRepository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IAutoCodeRepository _iAutoCodeRepository;
        private readonly IEmpLeaveReviewerRepository _iEmpLeaveReviewerRepository;
        private readonly IDptLeaveReviewerRepository _iDptLeaveReviewerRepository;
        private readonly ILvAppReviewerRepository _iLvAppReviewerRepository;

        public EmpLeaveApplicationService(IEmpLeaveApplicationRepository iRepository, IMapper iMapper,
            IUnitOfWork iUnitOfWork, IEmployeeRepository iEmployeeRepository, ILeaveTypeRepository iLeaveTypeRepository,
            IEmpAttendanceRepository iEmpAttendanceRepository, ILeaveSetupRepository iLeaveSetupRepository, ILeaveCfRepository iLeaveCfRepository, IAutoCodeRepository iAutoCodeRepository, IEmpLeaveReviewerRepository iEmpLeaveReviewerRepository, IDptLeaveReviewerRepository iDptLeaveReviewerRepository, ILvAppReviewerRepository iLvAppReviewerRepository) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
            _iEmpAttendanceRepository = iEmpAttendanceRepository;
            _iEmployeeRepository = iEmployeeRepository;
            _iLeaveTypeRepository = iLeaveTypeRepository;
            _iLeaveSetupRepository = iLeaveSetupRepository;
            _iLeaveCfRepository = iLeaveCfRepository;
            _iAutoCodeRepository = iAutoCodeRepository;
            _iEmpLeaveReviewerRepository = iEmpLeaveReviewerRepository;
            _iDptLeaveReviewerRepository = iDptLeaveReviewerRepository;
            _iLvAppReviewerRepository = iLvAppReviewerRepository;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<EmpLeaveApplicationSearchVm, EmpLeaveApplicationSearchVm>>
            SearchAsync(DataTablePagination<EmpLeaveApplicationSearchVm, EmpLeaveApplicationSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion

        #region PreviewData

        public async Task<EmpLeaveApplicationVm> GetLeaveAppPreviewData(EmpLeaveApplicationVm modelVm)
        {
            if (!(modelVm.EmployeeId > 0) || !(modelVm.LeaveTypeId > 0)) throw new Exception("No Employee Info Found...!");

            var empLeaveInfo = await EmployeeWiseLeaveBalance(modelVm.EmployeeId, DateTime.Now.Year);
            if (empLeaveInfo == null) throw new Exception("No Employee Info Found...!");

            var leaveType = _iLeaveTypeRepository.GetFirstOrDefault(c => c.Id == modelVm.LeaveTypeId && !c.IsDeleted);
            if (leaveType == null) throw new Exception("Leave Info Not Found...!");

            var model = new EmpLeaveApplicationVm();
            model.EmployeeId = empLeaveInfo.EmployeeId;
            model.EmployeeName = empLeaveInfo.EmployeeName;
            model.EmpDesignation = empLeaveInfo.EmpDesignation;
            model.EmpDepartment = empLeaveInfo.EmpDepartment;
            model.EmpPhotoUrl = empLeaveInfo.EmpPhotoUrl;
            model.Code = empLeaveInfo.Code;
            model.JoinDate = empLeaveInfo.JoinDate;
            model.EmployeeStatus = empLeaveInfo.EmployeeStatus;
            model.Gender = empLeaveInfo.Gender;

            model.FromDate = modelVm.FromDate;
            model.ToDate = modelVm.ToDate;
            model.Reason = modelVm.Reason;
            model.LeaveTypeId = modelVm.LeaveTypeId;
            model.LeaveTypeName = leaveType.TypeName;

            model.EmpLeaveInfos = empLeaveInfo.LeaveBalanceVms;

            var empLeaveAppList = await Repository.GetAsync(c => c.EmployeeId == model.EmployeeId && c.Status == (short)EmpLeaveAppStatusEnum.FINALAPPROVE && DateTime.Now.Year == c.FromDate.Date.Year, x => x.LeaveType);

            model.EmpLeaveApps = _iMapper.Map<List<EmpLeaveApplicationVm>>(empLeaveAppList);

            foreach (var type in model.EmpLeaveApps)
            {
                var filterData = empLeaveAppList.FirstOrDefault(c => c.Id == type.Id);

                type.LeaveTypeName = filterData.LeaveType.TypeName;
            }

            return model;
        }

        #endregion

        #region LeaveAdd

        public async Task<bool> LeaveApplyAsync(EmpLeaveApplicationVm vm)
        {
            var model = _iMapper.Map<EmpLeaveApplication>(vm);

            model.Status = (short)EmpLeaveAppStatusEnum.SUBMISSION;
            //model.FromDate = (DateTime)(!string.IsNullOrEmpty(vm.FromDateStr) ? DU.Utility.ConvertStrToDate(vm.FromDateStr) : model.FromDate);
            //model.ToDate = (DateTime)(!string.IsNullOrEmpty(vm.ToDateStr) ? DU.Utility.ConvertStrToDate(vm.ToDateStr) : model.ToDate);

            model.ApplicationNo = await GetEmpLeaveAppCode();
            model.SubmitById = CurrentUserId;
            model.SubmitDate = Utility.GetBdDateTimeNow();

            model.ActionById = CurrentUserId;
            model.ActionDate = Utility.GetBdDateTimeNow();

            var employeeInfo = await _iEmployeeRepository.GetFirstOrDefaultAsync(c => c.Id == vm.EmployeeId && c.IsEnable && !c.IsDeleted);
            if (employeeInfo == null) throw new Exception("Employee Info Not Found...!");

            var nextReviewer = new LvAppReviewer();

            var empAppReviewer = await _iEmpLeaveReviewerRepository.GetFirstOrDefaultAsync(c => c.EmployeeId == vm.EmployeeId && c.SlNo == (short)ReviewerSlNoEnum.FirstApprover && c.ReviewFor == (short)ReviewForEnum.Leave);

            if (empAppReviewer == null)
            {
                var dptAppReviewer = await _iDptLeaveReviewerRepository.GetFirstOrDefaultAsync(c => c.DepartmentId == employeeInfo.DepartmentId && c.SlNo == (short)ReviewerSlNoEnum.FirstApprover && c.ReviewFor == (short)ReviewForEnum.Leave);

                if (dptAppReviewer == null) throw new Exception("No Application Reviewer Found...!!");

                nextReviewer.ReviewerId = dptAppReviewer.ReviewerId;
                nextReviewer.AltReviewerId = dptAppReviewer.AltReviewerId;
                nextReviewer.SlNo = dptAppReviewer.SlNo;
                nextReviewer.IsFinalReviewer = dptAppReviewer.IsFinalReviewer;
            }
            else
            {
                nextReviewer.ReviewerId = empAppReviewer.ReviewerId;
                nextReviewer.AltReviewerId = empAppReviewer.AltReviewerId;
                nextReviewer.SlNo = empAppReviewer.SlNo;
                nextReviewer.IsFinalReviewer = empAppReviewer.IsFinalReviewer;
            }

            nextReviewer.Status = (short)LvAppReviewerStatusEnum.NotResponse;
            nextReviewer.ReceiveTime = DateTime.Now;
            nextReviewer.Remarks = $"Application Submit By {employeeInfo.Name} On Date: {model.SubmitDate.ToString("d")}";

            using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await Repository.AddAsync(model);
            var isAdded = await _iUnitOfWork.CompleteAsync();

            if (nextReviewer != null && nextReviewer.ReviewerId > 0)
            {
                nextReviewer.LeaveAppId = model.Id;
                await _iLvAppReviewerRepository.AddAsync(nextReviewer);
                var isReviewAdded = await _iUnitOfWork.CompleteAsync();
                if (!isReviewAdded) return false;
            }

            if (!isAdded) return false;
            ts.Complete();
            return true;
        }

        #endregion

        #region LeaveAppData

        public async Task<EmpLeaveApplicationVm> GetLeaveAppDataById(long appId)
        {
            if (appId > 0 is false) throw new Exception("No Application Index Info Found...!");

            var application = await Repository.GetFirstOrDefaultAsync(c => c.Id == appId && !c.IsDeleted, e => e.Employee);
            if (application == null) throw new Exception("No Application Info Found...!");

            var empLeaveInfo = await EmployeeWiseLeaveBalance(application.EmployeeId, DateTime.Now.Year);
            if (empLeaveInfo == null) throw new Exception("No Employee Info Found...!");

            var leaveType = _iLeaveTypeRepository.GetFirstOrDefault(c => c.Id == application.LeaveTypeId && !c.IsDeleted);
            if (leaveType == null) throw new Exception("Leave Info Not Found...!");

            //var model = new EmpLeaveApplicationVm();
            var model = _iMapper.Map<EmpLeaveApplicationVm>(application);
            model.EmployeeId = empLeaveInfo.EmployeeId;
            model.EmployeeName = empLeaveInfo.EmployeeName;
            model.EmpDesignation = empLeaveInfo.EmpDesignation;
            model.EmpDepartment = empLeaveInfo.EmpDepartment;
            model.EmpPhotoUrl = empLeaveInfo.EmpPhotoUrl;
            model.Code = empLeaveInfo.Code;
            model.JoinDate = empLeaveInfo.JoinDate;
            model.EmployeeStatus = empLeaveInfo.EmployeeStatus;
            model.Gender = empLeaveInfo.Gender;

            model.FromDate = application.FromDate;
            model.ToDate = application.ToDate;
            model.Reason = application.Reason;
            model.LeaveTypeId = application.LeaveTypeId;
            model.LeaveTypeName = leaveType.TypeName;

            model.EmpLeaveInfos = empLeaveInfo.LeaveBalanceVms;

            var empLeaveAppList = await Repository.GetAsync(c => c.EmployeeId == model.EmployeeId && c.Status == (short)EmpLeaveAppStatusEnum.FINALAPPROVE && DateTime.Now.Year == c.FromDate.Date.Year, x => x.LeaveType);

            model.EmpLeaveApps = _iMapper.Map<List<EmpLeaveApplicationVm>>(empLeaveAppList);

            foreach (var type in model.EmpLeaveApps)
            {
                var filterData = empLeaveAppList.FirstOrDefault(c => c.Id == type.Id);

                type.LeaveTypeName = filterData.LeaveType.TypeName;
            }

            var empAppReviewers = await _iEmpLeaveReviewerRepository.GetAsync(c => c.EmployeeId == application.EmployeeId && c.ReviewFor == (short)ReviewForEnum.Leave && !c.IsDeleted, r => r.Reviewer);

            if (empAppReviewers.Count > 0)
            {
                model.LeaveReviewerVms = _iMapper.Map<List<LeaveReviewerVm>>(empAppReviewers);

                foreach (var item in model.LeaveReviewerVms)
                {
                    var filterData = empAppReviewers.FirstOrDefault(c => c.Id == item.Id);

                    item.ReviewerName = filterData.Reviewer.FullName;
                }
            }
            else
            {
                var dptAppReviewers = await _iDptLeaveReviewerRepository.GetAsync(c => c.DepartmentId == application.Employee.DepartmentId && c.ReviewFor == (short)ReviewForEnum.Leave && !c.IsDeleted, r => r.Reviewer);

                if (!(dptAppReviewers.Count > 0)) throw new Exception("No Application Reviewer Found...!!");

                model.LeaveReviewerVms = _iMapper.Map<List<LeaveReviewerVm>>(dptAppReviewers);

                foreach (var item in model.LeaveReviewerVms)
                {
                    var filterData = dptAppReviewers.FirstOrDefault(c => c.Id == item.Id);

                    item.ReviewerName = filterData.Reviewer.FullName;
                }
            }

            var leaveAppReviewers = await _iLvAppReviewerRepository.GetAsync(c => c.LeaveAppId == application.Id && !c.IsDeleted, r => r.Reviewer);
            if (!(leaveAppReviewers.Count > 0)) throw new Exception("No Application Reviewer Found...!!");

            model.LvAppReviewerVms = _iMapper.Map<List<LvAppReviewerVm>>(leaveAppReviewers);

            var currentReviewer = model.LvAppReviewerVms.FirstOrDefault(c => c.Status == (short)LvAppReviewerStatusEnum.NotResponse);
            if (currentReviewer != null)
            {
                var filterData = leaveAppReviewers.FirstOrDefault(c => c.Id == currentReviewer.Id);

                currentReviewer.ReviewerName = filterData.Reviewer.FullName;
                model.CurrentReviewer = currentReviewer;

                model.IsReviewer = currentReviewer.ReviewerId == CurrentUserId ? true : false;
            }
            else
            {
                model.IsReviewer = false;
            }

            if (model.SubmitById == CurrentUserId && DateTime.Now.Date < model.AprToDate?.Date)
                model.CanCancel = true;

            return model;
        }

        #endregion

        #region EmployeeWiseLeaveBalance

        public async Task<EmpLeaveBalanceReportVm> EmployeeWiseLeaveBalance(long employeeId, int? searchYear)
        {
            var employee = await _iEmployeeRepository.GetEmployeeByIdAsync(employeeId);
            if (!employee.IsEnable) throw new Exception("Not Current Employee..!");
            if (employee == null) throw new Exception("Employee Not Found..!");

            var model = new EmpLeaveBalanceReportVm();
            model.EmployeeId = employeeId;
            model.EmployeeName = employee.Name.ToUpper();
            model.EmpDesignation = employee.DesignationName;
            model.EmpDepartment = employee.DepartmentName;
            model.EmpPhotoUrl = employee.PhotoUrl;
            model.Code = employee.Code;
            model.JoinDate = employee.JoinDate;
            model.EmployeeStatus = employee.EmployeeStatus;
            model.Gender = employee.Gender;


            var dtlList = new List<LeaveBalanceVm>();

            var leaveTypeList = _iLeaveTypeRepository.Get(c => !c.IsDeleted).ToList();

            if (employee.Gender == "M")
            {
                leaveTypeList = leaveTypeList.Where(c => c.Gender != "F").ToList();
            }

            if (leaveTypeList.Count > 0)
            {
                foreach (var leaveType in leaveTypeList)
                {
                    var leaveModel = new LeaveBalanceVm();
                    leaveModel.LeaveTypeId = leaveType.Id;
                    leaveModel.LeaveTypeName = leaveType.TypeName;

                    var leaveSetup = _iLeaveSetupRepository.GetFirstOrDefault(c => c.LeaveTypeId == leaveType.Id && c.IsActive);
                    if (leaveSetup == null) throw new Exception("Leave Type Setup Not Found..!");


                    var leaveBalance = leaveSetup.LeaveBalance;
                    leaveModel.CurrentYearBalance = leaveSetup.LeaveBalance;

                    if (leaveSetup.IsCarryForward)
                    {
                        var leaveCfSetup = _iLeaveCfRepository.GetFirstOrDefault(c => c.EmployeeId == employee.Id && c.LeaveTypeId == leaveType.Id && c.LeaveYear == searchYear);
                        //if (leaveCfSetup == null) throw new Exception("Leave Carry Forward Setup Not Found..!");
                        if (leaveCfSetup == null) continue;
                        leaveModel.CarryForwardBalance = leaveCfSetup.CfBalance;
                        leaveModel.CurrentYearBalance = leaveCfSetup.LeaveBalance;

                        leaveBalance = leaveCfSetup.LeaveBalance + leaveModel.CarryForwardBalance;
                    }

                    leaveModel.LeaveBalance = leaveBalance;

                    if (employee.JoinDate.Year == DateTime.Now.Year)
                    {
                        int year = employee.JoinDate.Year;
                        int month = 12;
                        DateTime lastDT = new DateTime(year, month, 1);

                        var monthDiff = AppUtility.YearMonthDiff(employee.JoinDate, DateTime.Now);

                        double leavePerMonth = leaveBalance / month;
                        var totalMonths = monthDiff.Item1;

                        var result = leavePerMonth * totalMonths;

                        leaveModel.LeaveBalance = Convert.ToInt32(Math.Ceiling(result));
                    }

                    var empLeaveAppList = await Repository.GetAsync(c => c.LeaveTypeId == leaveType.Id && c.EmployeeId == employee.Id && c.Status == (short)EmpLeaveAppStatusEnum.FINALAPPROVE);

                    if (searchYear > 0)
                    {
                        empLeaveAppList = empLeaveAppList.Where(c => c.FromDate.Year == searchYear).ToList();
                    }

                    leaveModel.LeaveTaken = empLeaveAppList.Sum(c => c.TotalApprovalLeave);
                    leaveModel.LeaveRemain = leaveModel.LeaveBalance - leaveModel.LeaveTaken;

                    dtlList.Add(leaveModel);
                }

                model.LeaveBalanceVms = dtlList;
            }

            return model;
        }

        #endregion

        #region GetEmpLeaveAppCode

        public async Task<string> GetEmpLeaveAppCode()
        {
            var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.EmpLeaveApplications.ToString(), "ApplicationNo", "LVAPP", 6);
            return data;
        }

        #endregion

        #region GetLeaveReportData

        public async Task<List<EmpLeaveReportVm>> GetLeaveReportData(EmpLeaveReportVm vm)
        {
            var data = await Repository.GetLeaveReportData(vm);
            return data;
        }

        #endregion

        #region ReportHtml

        public async Task<string> LeaveReportHtml(EmpLeaveReportVm vm)
        {
            var fullHtml = "";
            var data = await Repository.GetLeaveReportData(vm);

            fullHtml += "<table class='table table-bordered' id='LeaveReportTablePrint' style='width:100%;text-align: center'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th style='width:3%'>Sl.</th>";
            fullHtml += "<th style='width:10%'>Employee </th>";
            fullHtml += "<th style='width:10%'>Department</th>";
            fullHtml += "<th style='width:10%'>Designation</th>";
            fullHtml += "<th style='width:10%'>Join Date</th>";
            fullHtml += "<th style='width:8%'>Casual Leave</th>";
            fullHtml += "<th style='width:8%'>Medical Leave</th>";
            fullHtml += "<th style='width:8%'>Maturnity Leave</th>";
            fullHtml += "<th style='width:8%'>Earn Leave</th>";
            fullHtml += "<th style='width:9%'>Leave Without Pay</th>";
            fullHtml += "<th style='width:8%'>Study Leave</th>";
            fullHtml += "<th style='width:8%'>Total Leave</th>";
            fullHtml += "</tr>";

            fullHtml += "</thead>";

            if (data.Count > 0)
            {
                foreach (var (v, i) in data.GetItemWithIndex())
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td>{i + 1}</td>";
                    fullHtml += $@"<td class='text-start'><b>{v.EmployeeName}</b><br/>{v.EmployeeCode}</td>";
                    fullHtml += $@"<td class='text-center'>{v.EmpDepartment}</td>";
                    fullHtml += $@"<td class='text-center'>{v.EmpDesignation}</td>";
                    fullHtml += $@"<td class='text-center'>{v.EmpJoinDate.ToString("d")}</td>";
                    fullHtml += $@"<td class='text-center'>{v.CasualLeave}</td>";
                    fullHtml += $@"<td class='text-center'>{v.MedicalLeave}</ td >";
                    fullHtml += $@"<td class='text-center'>{v.MaturnityLeave}</ td >";
                    fullHtml += $@"<td class='text-center'>{v.EarnLeave}</ td >";
                    fullHtml += $@"<td class='text-center'>{v.LeaveWithoutPay}</ td >";
                    fullHtml += $@"<td class='text-center'>{v.StudyLeave}</ td >";
                    fullHtml += $@"<td class='text-center'>{v.TotalLeave}</ td >";

                    fullHtml += "</tr>";
                }
            }

            fullHtml += "</table>";

            return fullHtml;
        }

        public async Task<string> EmpLeaveStatementHtml(EmpLeaveBalanceReportVm model)
        {
            if (model.EmployeeId == 0) throw new Exception("EmployeeId Not Found..!");
            var result = await EmployeeWiseLeaveBalance(model.EmployeeId, model.SelectYear);

            var fullHtml = "";
            fullHtml += "<div style='padding-bottom: 10px;'>";

            fullHtml += $@"<table>
                                <tbody>
                                    <tr>
                                        <td style='width:10%; font-size:12px'><b>Name : </b></td>
                                        <td style='width:15%; font-size:12px'>{result.EmployeeName}</td>
                                        <td style='width:10%; padding-top:5px; font-size:12px'><b>Designation : </b></td>
                                        <td style='width:15%; padding-top:5px; font-size:12px'>{result.EmpDesignation}</td>
                                        <td style='width:10%; padding-top:5px; font-size:12px'><b>Status : </b></td>
                                        <td style='width:15%; padding-top:5px; font-size:12px'>{result.EmployeeStatusText}</td>
                                        <td style='width:10%; padding-top:5px; font-size:12px'><b>Join Date : </b></td>
                                        <td style='width:15%; padding-top:5px; font-size:12px'>{Utility.ConvertDateToStr(result.JoinDate)}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:10%; font-size:12px'><b>Code : </b></td>
                                        <td style='width:15%; font-size:12px'>{result.Code}</td>
                                        <td style='width:10%; padding-top:5px; font-size:12px'><b>Department : </b></td>
                                        <td style='width:15%; padding-top:5px; font-size:12px'>{result.EmpDepartment}</td>
                                        <td style='width:10%; padding-top:5px; font-size:12px'><b>Leave Year : </b></td>
                                        <td style='width:15%; padding-top:5px; font-size:12px'>{DateTime.Now.Year}</td>
                                        <td colspan='1' style='width:10%; padding-top:5px; font-size:12px'><b>Report Date : </b></td>
                                        <td style='width:15%; padding-top:5px; font-size:12px'>{DateTime.Now.ConvertDateToStr()}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:10%; font-size:12px'><b>Gender : </b></td>
                                        <td style='width:15%; font-size:12px'>{result.GenderText}</td>
                                    </tr>
                                </tbody>
                            </table>";

            fullHtml += "</div>";

            fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center' style='repeat-header:yes;'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th style='width:5%'>Sl.</th>";
            fullHtml += "<th style='width:20%'>Leave Type</th>";
            fullHtml += "<th style='width:20%'>Current Year Balance</th>";
            fullHtml += "<th style='width:10%'>Carry Forward</th>";
            fullHtml += "<th style='width:10%'>Total</th>";
            fullHtml += "<th style='width:10%'>Taken</th>";
            fullHtml += "<th style='width:10%'>Remaining</th>";
            fullHtml += "</tr>";

            fullHtml += "</thead>";
            var totalLeaveBalance = 0;
            var totalLeaveTaken = 0;
            var totalLeaveRemain = 0;
            var totalCFBalance = 0;
            var totalSum = 0;
            fullHtml += "<tbody>";
            var i = 1;
            if (result.LeaveBalanceVms.Count > 0)
            {
                foreach (var v in result.LeaveBalanceVms)
                {
                    fullHtml += "<tr>";
                    fullHtml += $@"<td class='text-center'>{i++}</td>";
                    fullHtml += $@"<td class='text-start'>{v.LeaveTypeName}</td>";
                    fullHtml += $@"<td class='text-end'>{v.LeaveBalance}</td>";
                    fullHtml += $@"<td class='text-end'>{v.CarryForwardBalance}</td>";
                    fullHtml += $@"<td class='text-end'>{v.LeaveBalance + v.CarryForwardBalance}</td>";
                    fullHtml += $@"<td class='text-end'>{v.LeaveTaken}</td>";
                    fullHtml += $@"<td class='text-end'>{v.LeaveRemain}</td>";

                    fullHtml += "</tr>";
                    totalLeaveBalance += v.LeaveBalance;
                    totalCFBalance += v.CarryForwardBalance;
                    totalSum = totalSum + (v.LeaveBalance + v.CarryForwardBalance);
                    totalLeaveTaken += v.LeaveTaken;
                    totalLeaveRemain += v.LeaveRemain;
                }
            }

            fullHtml += "</tbody>";

            fullHtml += "<tfoot>";
            fullHtml += "<tr class='text-align: center'>";
            fullHtml += "<th colspan='2' class='text-end'>Total</th>";
            fullHtml += $@"<td class='text-end'><b>{totalLeaveBalance}</b></td>";
            fullHtml += $@"<td class='text-end'><b>{totalCFBalance}</b></td>";
            fullHtml += $@"<td class='text-end'><b>{totalSum}</b></td>";
            fullHtml += $@"<td class='text-end'><b>{totalLeaveTaken}</b></td>";
            fullHtml += $@"<td class='text-end'><b>{totalLeaveRemain}</b></td>";
            fullHtml += "</tr>";
            fullHtml += "</tfoot>";

            fullHtml += "</table>";

            return fullHtml;
        }

        #endregion

        #region CancelLeave

        public async Task<bool> CancelLeaveApp(long appId)
        {
            if (appId == 0) throw new Exception("Application Id Not Found..!");
            var application = await Repository.GetFirstOrDefaultAsync(c => c.Id == appId && !c.IsDeleted);
            if (application == null) throw new Exception("Application Not Found..!");

            application.Status = (short)EmpLeaveAppStatusEnum.SELFCANCEL;

            if (application.FromDate.Date <= DateTime.Now.Date) throw new Exception("Leave Date Is Already Over");
            var leaveDates = DateRange(application.FromDate, application.ToDate);

            var leaveTypeSetup = await _iLeaveSetupRepository.GetFirstOrDefaultAsync(c => c.LeaveTypeId == application.LeaveTypeId);
            if (leaveTypeSetup == null) throw new Exception("Leave Setup Not Found..!!");

            var deleteLeaveList = new List<EmpAttendance>();

            if (leaveDates.Count() > 0)
            {
                foreach (var leave in leaveDates)
                {
                    var existAtt = _iEmpAttendanceRepository.GetFirstOrDefault(c => c.EmployeeId == application.EmployeeId && c.AttendDate.Date == leave.Date);

                    if (existAtt != null && (existAtt.Status == EmpAttendanceStatus.Present || existAtt.Status == EmpAttendanceStatus.Holiday || existAtt.Status == EmpAttendanceStatus.Offday)) continue;

                    deleteLeaveList.Add(existAtt);
                }
            }

            LeaveCf earnLeave = null;

            if (leaveTypeSetup.IsCarryForward)
            {
                var empCf = await _iLeaveCfRepository.GetFirstOrDefaultAsync(c => c.EmployeeId == application.EmployeeId && c.LeaveYear == application.SubmitDate.Year);
                if (empCf == null) throw new Exception("Employee Earn Leave Setup Not Found...!");

                empCf.LeaveEnjoyed = empCf.LeaveEnjoyed - application.TotalApprovalLeave;

                earnLeave = empCf;
            }

            using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await Repository.UpdateAsync(application);
            if (earnLeave != null) await _iLeaveCfRepository.UpdateAsync(earnLeave);
            if (deleteLeaveList.Count > 0) _iEmpAttendanceRepository.RemoveRange(deleteLeaveList);

            var isUpdated = await _iUnitOfWork.CompleteAsync();
            if (!isUpdated) return false;
            ts.Complete();

            return true;

        }

        #endregion

        #region DateRange

        private IEnumerable<DateTime> DateRange(DateTime startingDate, DateTime endingDate)
        {
            if (endingDate < startingDate)
            {
                throw new ArgumentException("endingDate should be after startingDate");
            }
            var ts = endingDate - startingDate;
            for (int i = 0; i <= ts.TotalDays; i++)
            {
                yield return startingDate.AddDays(i);
            }
        }

        #endregion

        #region LeaveAdd

        public async Task<bool> GeneralLeaveApplyAsync(EmpLeaveApplicationVm vm)
        {
            var model = _iMapper.Map<EmpLeaveApplication>(vm);
            model.FromDate = (DateTime)(!string.IsNullOrEmpty(vm.FromDateStr) ? DU.Utility.ConvertStrToDate(vm.FromDateStr) : model.FromDate);
            model.ToDate = (DateTime)(!string.IsNullOrEmpty(vm.ToDateStr) ? DU.Utility.ConvertStrToDate(vm.ToDateStr) : model.ToDate);

            model.Status = (short)EmpLeaveAppStatusEnum.FINALAPPROVE;
            model.ApplicationNo = await GetEmpLeaveAppCode();
            model.SubmitById = CurrentUserId;
            model.SubmitDate = Utility.GetBdDateTimeNow();
            model.AprFromDate = model.FromDate;
            model.AprToDate = model.ToDate;
            model.ActualFromDate = model.FromDate;
            model.ActualToDate = model.ToDate;
            model.TotalApprovalLeave = (short)DU.AppUtility.DaysDiffernceOnlyDate((DateTime)model.AprToDate, (DateTime)model.AprFromDate);

            model.ActionById = CurrentUserId;
            model.ActionDate = Utility.GetBdDateTimeNow();

            var employeeInfo = await _iEmployeeRepository.GetFirstOrDefaultAsync(c => c.Id == vm.EmployeeId && c.IsEnable && !c.IsDeleted);
            if (employeeInfo == null) throw new Exception("Employee Info Not Found...!");

            using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await Repository.AddAsync(model);
            var isAdded = await _iUnitOfWork.CompleteAsync();

            if (!isAdded) return false;
            ts.Complete();
            return true;
        }

        #endregion

    }
}
