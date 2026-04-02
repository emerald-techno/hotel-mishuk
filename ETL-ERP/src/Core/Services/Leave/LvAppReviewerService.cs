using AutoMapper;
using Domain.Entities.Attendance;
using Domain.Entities.Leave;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LvAppReviewer;
using Interface.Repository.Attendance;
using Interface.Repository.Leave;
using Interface.Services.Leave;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.Leave
{
    public class LvAppReviewerService : BaseService<LvAppReviewer>, ILvAppReviewerService
    {
        #region Config
        private ILvAppReviewerRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IEmpLeaveApplicationRepository _iEmpLeaveApplicationRepository;
        private readonly IEmpLeaveReviewerRepository _iEmpLeaveReviewerRepository;
        private readonly IDptLeaveReviewerRepository _iDptLeaveReviewerRepository;
        private readonly IEmpAttendanceRepository _iEmpAttendanceRepository;
        private readonly ILeaveSetupRepository _iLeaveSetupRepository;
        private readonly ILeaveCfRepository _iLeaveCfRepository;

        public LvAppReviewerService(ILvAppReviewerRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IEmpLeaveReviewerRepository iEmpLeaveReviewerRepository, IDptLeaveReviewerRepository iDptLeaveReviewerRepository, IEmpLeaveApplicationRepository iEmpLeaveApplicationRepository, IEmpAttendanceRepository iEmpAttendanceRepository, ILeaveSetupRepository iLeaveSetupRepository, ILeaveCfRepository iLeaveCfRepository)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
            _iEmpLeaveReviewerRepository = iEmpLeaveReviewerRepository;
            _iDptLeaveReviewerRepository = iDptLeaveReviewerRepository;
            _iEmpLeaveApplicationRepository = iEmpLeaveApplicationRepository;
            _iEmpAttendanceRepository = iEmpAttendanceRepository;
            _iLeaveSetupRepository = iLeaveSetupRepository;
            _iLeaveCfRepository = iLeaveCfRepository;
        }
        #endregion

        #region AppReview

        public async Task<bool> LeaveAppReview(LvAppReviewerVm vm)
        {
            if (!(vm.Id > 0)) throw new Exception("No Reviewer Info Found...!");

            var leaveReviewer = await Repository.GetFirstOrDefaultAsync(c => c.Id == vm.Id && c.Status == (short)LvAppReviewerStatusEnum.NotResponse && !c.IsDeleted);
            if (leaveReviewer == null) throw new Exception("No Reviewer Found...!");

            var application = await _iEmpLeaveApplicationRepository.GetFirstOrDefaultAsync(c => c.Id == leaveReviewer.LeaveAppId && !c.IsDeleted, e => e.Employee);
            if (application == null) throw new Exception("No Application Info Found...!");

            var employeeInfo = application.Employee;

            leaveReviewer.Status = vm.Status;
            leaveReviewer.ResponseTime = DU.Utility.GetBdDateTimeNow();
            leaveReviewer.Remarks = vm.Remarks;

            var nextReviewer = new LvAppReviewer();

            if (!leaveReviewer.IsFinalReviewer)
            {
                var nextSl = (short)(leaveReviewer.SlNo + 1);

                var empAppReviewerList = await _iEmpLeaveReviewerRepository.GetAsync(c => c.EmployeeId == application.EmployeeId && c.ReviewFor == (short)ReviewForEnum.Leave && !c.IsDeleted);

                var empAppReviewer = empAppReviewerList.FirstOrDefault(c => c.SlNo == nextSl);
                if (empAppReviewer == null) { empAppReviewer = empAppReviewerList.FirstOrDefault(c => c.SlNo == (short)ReviewerSlNoEnum.FINALAPPROVER); }

                if (empAppReviewer == null)
                {
                    var dptAppReviewerList = await _iDptLeaveReviewerRepository.GetAsync(c => c.DepartmentId == employeeInfo.DepartmentId && c.ReviewFor == (short)ReviewForEnum.Leave && !c.IsDeleted);

                    var dptAppReviewer = dptAppReviewerList.FirstOrDefault(c => c.SlNo == nextSl);
                    if (dptAppReviewer == null) { dptAppReviewer = dptAppReviewerList.FirstOrDefault(c => c.SlNo == (short)ReviewerSlNoEnum.FINALAPPROVER); }

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
            }

            var appStage = leaveReviewer.IsFinalReviewer ? leaveReviewer.SlNo : nextReviewer.SlNo;

            application.Status = leaveReviewer.SlNo;

            var empLeaves = new List<EmpAttendance>();
            var updateEmpLeaves = new List<EmpAttendance>();

            if (leaveReviewer.IsFinalReviewer)
            {
                application.AprFromDate = !string.IsNullOrEmpty(vm.AprFromDateStr) ? DU.Utility.ConvertStrToDate(vm.AprFromDateStr) : application.FromDate;
                application.AprToDate = !string.IsNullOrEmpty(vm.AprToDateStr) ? DU.Utility.ConvertStrToDate(vm.AprToDateStr) : application.ToDate;
                application.TotalApprovalLeave = (short)DU.AppUtility.DaysDiffernce((DateTime)application.AprToDate, (DateTime)application.AprFromDate);

                #region AttendanceCalculate

                var leaveDates = DU.AppUtility.DateRangeList((DateTime)application.AprFromDate, (DateTime)application.AprToDate);

                if (leaveDates.Count() > 0)
                {
                    foreach (var leave in leaveDates)
                    {
                        var existAtt = _iEmpAttendanceRepository.GetFirstOrDefault(c => c.EmployeeId == application.EmployeeId && c.AttendDate.Date == leave.Date);

                        if (existAtt != null && (existAtt.Status == EmpAttendanceStatus.Present || existAtt.Status == EmpAttendanceStatus.Leave)) continue;

                        if (existAtt != null && (existAtt.Status == EmpAttendanceStatus.Absent || existAtt.Status == EmpAttendanceStatus.Holiday || existAtt.Status == EmpAttendanceStatus.Offday))
                        {
                            existAtt.Status = EmpAttendanceStatus.Leave;
                            updateEmpLeaves.Add(existAtt);
                            continue;
                        }

                        var leaveModel = new EmpAttendance();

                        leaveModel.AttendDate = leave;
                        leaveModel.Status = EmpAttendanceStatus.Leave;
                        leaveModel.EmployeeId = application.EmployeeId;
                        leaveModel.ActionById = CurrentUserId;
                        leaveModel.ActionDate = DU.Utility.GetBdDateTimeNow();

                        empLeaves.Add(leaveModel);
                    }
                }

                if (empLeaves.Count == 0 && updateEmpLeaves.Count == 0) throw new Exception("Leave Already Added..!!");

                var leaveTypeSetup = await _iLeaveSetupRepository.GetFirstOrDefaultAsync(c => c.LeaveTypeId == application.LeaveTypeId);
                if (leaveTypeSetup == null) throw new Exception("Leave Setup Not Found..!!");

                LeaveCf earnLeave = null;

                if (leaveTypeSetup.IsCarryForward)
                {
                    var empCf = await _iLeaveCfRepository.GetFirstOrDefaultAsync(c => c.EmployeeId == application.EmployeeId && c.LeaveYear == application.SubmitDate.Year);
                    if (empCf == null) throw new Exception("Employee Earn Leave Setup Not Found...!");

                    empCf.LeaveEnjoyed = empCf.LeaveEnjoyed + application.TotalApprovalLeave;

                    earnLeave = empCf;
                }

                #endregion
            }

            using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await Repository.UpdateAsync(leaveReviewer);
            var isUpdated = await _iUnitOfWork.CompleteAsync();

            await _iEmpLeaveApplicationRepository.UpdateAsync(application);

            if (empLeaves.Count > 0) await _iEmpAttendanceRepository.AddRangeAsync(empLeaves);
            if (updateEmpLeaves.Count > 0) await _iEmpAttendanceRepository.UpdateRangeAsync(updateEmpLeaves);

            var isAppUpdated = await _iUnitOfWork.CompleteAsync();


            if (nextReviewer != null && nextReviewer.ReviewerId > 0)
            {
                nextReviewer.LeaveAppId = leaveReviewer.LeaveAppId;
                await Repository.AddAsync(nextReviewer);
                var isReviewAdded = await _iUnitOfWork.CompleteAsync();
                if (!isReviewAdded) return false;
            }

            if (!isUpdated && !isAppUpdated) return false;
            ts.Complete();
            return true;
        }

        #endregion

        #region AppReject

        public async Task<bool> LeaveAppReject(LvAppReviewerVm vm)
        {
            if (!(vm.Id > 0)) throw new Exception("No Reviewer Info Found...!");

            var leaveReviewer = await Repository.GetFirstOrDefaultAsync(c => c.Id == vm.Id && c.Status == (short)LvAppReviewerStatusEnum.NotResponse && !c.IsDeleted);
            if (leaveReviewer == null) throw new Exception("No Reviewer Found...!");

            var application = await _iEmpLeaveApplicationRepository.GetFirstOrDefaultAsync(c => c.Id == leaveReviewer.LeaveAppId && !c.IsDeleted, e => e.Employee);
            if (application == null) throw new Exception("No Application Info Found...!");

            leaveReviewer.Status = vm.Status;
            leaveReviewer.ResponseTime = DU.Utility.GetBdDateTimeNow();
            leaveReviewer.Remarks = vm.Remarks;

            application.Status = (short)EmpLeaveAppOnlineStatusEnum.REJECT;

            using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await Repository.UpdateAsync(leaveReviewer);
            var isUpdated = await _iUnitOfWork.CompleteAsync();

            await _iEmpLeaveApplicationRepository.UpdateAsync(application);
            var isAppUpdated = await _iUnitOfWork.CompleteAsync();

            if (!isUpdated && !isAppUpdated) return false;
            ts.Complete();
            return true;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<LvAppReviewerSearchVm, LvAppReviewerSearchVm>>
            SearchAsync(DataTablePagination<LvAppReviewerSearchVm, LvAppReviewerSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion
    }
}
