using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Entities.HotelManagement;
using Domain.Entities.Payroll;
using Domain.Entities.Pf;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrEmpSalaryPart;
using Domain.ViewModel.Payroll.PrSalaryDtl;
using Domain.ViewModel.Payroll.PrSalaryPart;
using Interface.Repository.Accounts;
using Interface.Repository.Common;
using Interface.Repository.Hr;
using Interface.Repository.Payroll;
using Interface.Repository.Pf;
using Interface.Repository.Restaurant;
using Interface.Services.Accounts;
using Interface.Services.Admin;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;

namespace Services.Payroll;

public class PrSalaryDtlService : BaseService<PrSalaryDtl>, IPrSalaryDtlService
{
    #region Config

    private readonly IPrSalaryDtlRepository Repository;
    private readonly IPrSalaryPartService _iPartService;
    private readonly IPrEmpSalaryPartService _iPrEmpSalaryPartService;
    private readonly IPrArrearMstRepository _iArrearMstRepository;
    private readonly IPrArrearDtlRepository _iArrearDtlRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IFoodOrderRepository _iFoodOrderRepository;
    private readonly IRsOrderPaymentRepository _iOrderPaymentRepository;
    private readonly ICustomerRepository _iCustomerRepository;
    private readonly IEmployeeRepository _iEmployeeRepository;
    private readonly IAccTranMstRepository _iAccTranMstRepository;
    private readonly IEmpLoanMstRepository _iEmpLoanMstRepository;
    private readonly IEmpLoanDtlRepository _iEmpLoanDtlRepository;
    private readonly IAutoCodeRepository _iAutoCodeRepository;

    private readonly IAutoVoucherService _iAutoVoucherService;
    private readonly IDepartmentService _iDepartmentService;

    public PrSalaryDtlService(IPrSalaryDtlRepository iRepository,
        IMapper iMapper, IUnitOfWork iUnitOfWork,
        IPrEmpSalaryPartService iPrEmpSalaryPartService,
        IPrSalaryPartService iPartService,
        IPrArrearMstRepository iArrearMstRepository,
        IPrArrearDtlRepository iArrearDtlRepository,
        IFoodOrderRepository iFoodOrderRepository,
        IRsOrderPaymentRepository iOrderPaymentRepository,
        ICustomerRepository iCustomerRepository,
        IEmployeeRepository iEmployeeRepository,
        IDepartmentService iDepartmentService,
        IAutoVoucherService iAutoVoucherService,
        IAccTranMstRepository iAccTranMstRepository,
        IEmpLoanMstRepository iEmpLoanMstRepository,
        IEmpLoanDtlRepository iEmpLoanDtlRepository,
        IAutoCodeRepository iAutoCodeRepository) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iPrEmpSalaryPartService = iPrEmpSalaryPartService;
        _iPartService = iPartService;
        _iArrearMstRepository = iArrearMstRepository;
        _iArrearDtlRepository = iArrearDtlRepository;
        _iFoodOrderRepository = iFoodOrderRepository;
        _iOrderPaymentRepository = iOrderPaymentRepository;
        _iCustomerRepository = iCustomerRepository;
        _iEmployeeRepository = iEmployeeRepository;
        _iDepartmentService = iDepartmentService;
        _iAutoVoucherService = iAutoVoucherService;
        _iAccTranMstRepository = iAccTranMstRepository;
        _iEmpLoanMstRepository = iEmpLoanMstRepository;
        _iEmpLoanDtlRepository = iEmpLoanDtlRepository;
        _iAutoCodeRepository = iAutoCodeRepository;
    }

    #endregion

    #region GeneratePayslip

    public async Task<PayslipVm> GeneratePayslip(long id)
    {
        if (id == 0) throw new Exception("Payslip Generate Failed..!!");
        var data = await Repository.GetFirstOrDefaultAsync(c => c.Id == id, s => s.SalaryMst, e => e.Employee, d => d.Employee.Designation, dp => dp.Employee.Department);
        if (data == null) throw new Exception("No Data Found For Generate Payslip..!!");
        var model = _iMapper.Map<PayslipVm>(data);
        model.SalaryMonth = data.SalaryMst.Month;
        model.SalaryYear = data.SalaryMst.Year;
        model.SalaryDate = data.SalaryMst.SalaryDate;
        model.EmployeeName = data.Employee.Name;
        model.EmployeeCode = data.Employee.Code;
        model.EmpDepartmentName = data.Employee?.Department?.Name;
        model.EmpDesignationName = data.Employee?.Designation?.Name;
        model.EmpPhotoUrl = data.Employee.PhotoUrl;
        model.EmpJoinDate = data.Employee.JoinDate;

        var enableSalaryParts = await _iPartService.GetAsync(c => c.IsEnable);
        model.PrSalaryParts = _iMapper.Map<List<PrSalaryPartVm>>(enableSalaryParts);

        var empSalaryParts = await _iPrEmpSalaryPartService.GetAsync(c => c.EmployeeId == data.EmployeeId, p => p.SalaryPart);
        var partModelList = _iMapper.Map<List<PrEmpSalaryPartVm>>(empSalaryParts);

        if (partModelList.Count > 0)
        {
            foreach (var item in partModelList)
            {
                var filterData = empSalaryParts.FirstOrDefault(c => c.Id == item.Id);

                item.EmpPartName = filterData.SalaryPart.PartName;
                item.EmpPartCode = filterData.SalaryPart.PartCode;
            }
        }

        model.PrEmpSalaryParts = partModelList;

        return model;
    }

    #endregion

    #region PayMultiSalary

    public async Task<bool> PayMultiSalary(List<PrSalaryDtl> salaryDtls)
    {
        var arrearList = new List<PrArrearDtl>();
        List<RsFoodOrder> updateRsFoodOrders = new List<RsFoodOrder>();
        List<RsOrderPayments> rsFoodPayments = new List<RsOrderPayments>();
        List<EmpLoanMst> fullLoanPaidList = new List<EmpLoanMst>();
        List<EmpLoanDtl> instalmentList = new List<EmpLoanDtl>();

        List<AccTranMst> foodVoucherList = new List<AccTranMst>();
        List<AccTranMst> loanVoucherList = new List<AccTranMst>();
        List<AccTranMst> salaryVoucherList = new List<AccTranMst>();
        List<AccTranMst> deductVoucherList = new List<AccTranMst>();

        if (salaryDtls != null && salaryDtls.Count > 0)
        {
            var prMst = salaryDtls.FirstOrDefault().SalaryMst;

            var salaryMonth = new DateTime(prMst.Year, prMst.Month, 1);
            var preMonth = salaryMonth.AddMonths(-1);

            var lastVoucherNo = await _iAutoCodeRepository.GetLastVoucherNo(VoucherTypeCode.JournalVoucher, DateTime.Now);
            int lastVcNo = Convert.ToInt32(lastVoucherNo);

            var employeeListIds = salaryDtls.Select(x => x.EmployeeId).ToList();
            var employeeList = await _iEmployeeRepository.GetAsync(x => employeeListIds.Contains(x.Id) && !x.IsDeleted, d => d.Department, ds => ds.Designation);
            var employeeCustomerList = await _iCustomerRepository.GetAsync(x => employeeListIds.Contains((long)x.EmployeeId) && !x.IsDeleted);

            var prArrear = await _iArrearMstRepository.GetFirstOrDefaultAsync(c => c.Year == preMonth.Year && c.Month == preMonth.Month && c.IsApproved, d => d.PrArrearDtls);

            foreach (var dtl in salaryDtls)
            {
                var employee = employeeList.FirstOrDefault(x => x.Id == dtl.EmployeeId);
                if (employee == null)
                    throw new Exception("Employee Information Not Found..!!");
                if (!employee.IsEnable)
                    throw new Exception("No Longer Employee Here..!!");

                var ledgerCode = _iDepartmentService.GetLedgerCodeByDptCode(employee.Department.Code);

                var existEmpArrears = prArrear != null && prArrear.PrArrearDtls.Count > 0 ? prArrear.PrArrearDtls.Where(c => c.EmployeeId == dtl.EmployeeId && !c.IsPaid).ToList() : null;

                if (existEmpArrears != null && existEmpArrears.Count > 0)
                {
                    foreach (var arrear in existEmpArrears)
                    {
                        arrear.IsPaid = true;
                        arrear.PaidDate = dtl.PaidDate;

                        arrearList.Add(arrear);
                    }
                }

                #region EmployeeFoodBillPayment

                if (dtl.ColE > 0 && AppUtility.IsAutoFoodBill == true)
                {
                    var customer = employeeCustomerList.FirstOrDefault(x => x.EmployeeId == dtl.EmployeeId);
                    if (customer == null)
                        continue;

                    //var unpaidFoodOrders = await _iFoodOrderRepository.GetAsync(x => x.CustomerId == customer.Id && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment);

                    var queryDate = new DateTime(2024, 10, 1); // oct-2024

                    var unpaidFoodOrders = await _iFoodOrderRepository.GetAsync(x => x.CustomerId == customer.Id && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment
                    && x.OrderStatus != RsOrderStatusEnum.Canceled && x.OrderDate.Date >= queryDate && x.OrderDate.Date < salaryMonth.AddMonths(1).Date);

                    if (unpaidFoodOrders.Count > 0)
                    {
                        foreach (var unpaidOrder in unpaidFoodOrders)
                        {
                            var foodPaidList = _iOrderPaymentRepository.Get(c => c.OrderId == unpaidOrder.Id && c.PaymentType == RsOrderPaymentTypeEnum.Receive && !c.IsDeleted).ToList();
                            var alreadyFoodPaidAmount = foodPaidList.Sum(x => x.PaidAmount);

                            var dueAmount = unpaidOrder.NetAmount > alreadyFoodPaidAmount ? unpaidOrder.NetAmount - alreadyFoodPaidAmount : 0;

                            var rsPaymentModel = new RsOrderPayments();
                            rsPaymentModel.OrderId = unpaidOrder.Id;
                            rsPaymentModel.PaidDate = (DateTime)dtl.PaidDate;
                            rsPaymentModel.PayMode = PayModeEnum.Cash;
                            rsPaymentModel.PaidAmount = dueAmount;
                            rsPaymentModel.ActionById = CurrentUserId;
                            rsPaymentModel.ActionDate = Utility.GetBdDateTimeNow();
                            rsPaymentModel.Remarks = $"Amount Is Paid From Employee Salary.";

                            var rsReportDate = Utility.GenerateReportDate(rsPaymentModel.PaidDate);
                            rsPaymentModel.ReportDate = rsReportDate;

                            if (rsPaymentModel.PaidAmount > 0)
                            {
                                unpaidOrder.PaymentStatus = RsOrderPaymentStatusEnum.FullPayment;

                                rsFoodPayments.Add(rsPaymentModel);
                                updateRsFoodOrders.Add(unpaidOrder);
                            }
                        }
                    }

                    if (AppUtility.IsAutoVoucherFoodBill)
                    {
                        var generateFoodVoucher = await _iAutoVoucherService.GetEmpFoodBillQuickVoucher(dtl, CurrentUserId, ledgerCode);
                        if (generateFoodVoucher == null)
                            throw new Exception("Somthing Went Wrong Creating Food Voucher..!!");
                        generateFoodVoucher.Narration = $"Food Bill. {employee.Name},{employee.Designation.Name}";
                        generateFoodVoucher.VcNo = $"JV/{lastVcNo.ToString().PadLeft(6, '0')}/{salaryMonth.ToString("yy")}";
                        foodVoucherList.Add(generateFoodVoucher);
                        lastVcNo = lastVcNo + 1;
                    }
                }

                #endregion

                #region EmployeeLoan

                if (dtl.ColJ > 0 && AppUtility.IsLoanCutFromPayroll == true)
                {
                    //var existEmpLoan = await _iEmpLoanMstRepository.GetFirstOrDefaultAsync(c => c.EmployeeId == employee.Id 
                    //&& prMst.SalaryDate.Date > c.FirstInsDate.Date && c.Status == (short)LoanStatusEnum.Running, d => d.EmpLoanDtls);

                    var existEmpLoanList = await _iEmpLoanMstRepository.GetAsync(c => c.EmployeeId == employee.Id
                    && prMst.SalaryDate.Date > c.FirstInsDate.Date && c.Status == (short)LoanStatusEnum.Running, d => d.EmpLoanDtls);

                    if (existEmpLoanList != null && existEmpLoanList.Count > 0)
                    {
                        //var empInstalmentList = existEmpLoan.EmpLoanDtls.Where(c => !c.IsPaid && !c.IsDeleted).OrderBy(s => s.Serial).ToList();

                        var empInstalmentList = existEmpLoanList.SelectMany(x => x.EmpLoanDtls).Where(c => !c.IsPaid && !c.IsDeleted).OrderBy(s => s.Serial).ToList();

                        //var thisMonthInstalment = empInstalmentList.FirstOrDefault(x => x.InsDate.Month == prMst.SalaryDate.Month);
                        var thisMonthInstalmentList = empInstalmentList.Where(x => x.InsDate.Month == prMst.SalaryDate.Month).ToList();

                        if(thisMonthInstalmentList != null && thisMonthInstalmentList.Count > 0)
                        {
                            foreach (var instalment in thisMonthInstalmentList)
                            {
                                instalment.PrDtlId = dtl.Id;
                                instalment.PaidSetById = CurrentUserId;
                                instalment.IsPaid = true;
                                instalment.PaidDate = dtl.PaidDate;
                                instalment.PaidAmount = dtl.ColJ;
                                instalment.Remarks = $"Auto Payment From {Utility.GetMonthName(prMst.Month).ToUpper()}-{prMst.Year} Payroll";
                                instalmentList.Add(instalment);

                                if (instalment.LoanAmount == 0)
                                {
                                    var existEmpLoan = existEmpLoanList.FirstOrDefault(x => x.Id == instalment.LoanId);

                                    existEmpLoan.Status = (short)LoanStatusEnum.Complete;
                                    fullLoanPaidList.Add(existEmpLoan);
                                }

                                if (AppUtility.IsAutoVoucherLoan)
                                {
                                    var generateLoanVoucher = await _iAutoVoucherService.GetAdvanceReceivedQuickVoucher(instalment, CurrentUserId, ledgerCode);
                                    if (generateLoanVoucher == null)
                                        throw new Exception("Somthing Went Wrong Creating Loan Voucher..!!");
                                    generateLoanVoucher.EmpLoanDtlId = instalment.Id;
                                    generateLoanVoucher.PrSalaryDtlId = dtl.Id;
                                    generateLoanVoucher.Narration = $"Advance Recovered Staffs. {employee.Name},{employee.Designation.Name}";
                                    generateLoanVoucher.VcNo = $"JV/{lastVcNo.ToString().PadLeft(6, '0')}/{salaryMonth.ToString("yy")}";
                                    loanVoucherList.Add(generateLoanVoucher);
                                    lastVcNo = lastVcNo + 1;
                                }
                            }
                        }
                    }
                }

                #endregion

                #region EmployeeSalaryVoucher

                if (AppUtility.IsAutoVoucherSalaryPay)
                {
                    var generateSalaryVoucher = await _iAutoVoucherService.GetEmpSalaryPayQuickVoucher(dtl, CurrentUserId, ledgerCode);
                    if (generateSalaryVoucher == null)
                        throw new Exception("Somthing Went Wrong Creating Salary Voucher..!!");
                    generateSalaryVoucher.Narration = $"Salary Paid Staffs. {employee.Name},{employee.Designation.Name}";
                    generateSalaryVoucher.VcNo = $"JV/{lastVcNo.ToString().PadLeft(6, '0')}/{salaryMonth.ToString("yy")}";
                    salaryVoucherList.Add(generateSalaryVoucher);
                    lastVcNo = lastVcNo + 1;
                }

                #endregion

                #region EmployeeSalaryAttDeductVoucher

                if (dtl.ColU > 0 && AppUtility.IsAutoVoucherAttDeduct)
                {
                    var generateAttDeductVoucher = await _iAutoVoucherService.GetEmpAttDeductionQuickVoucher(dtl, CurrentUserId, ledgerCode);
                    if (generateAttDeductVoucher == null)
                        throw new Exception("Somthing Went Wrong Creating Salary Deduct Voucher..!!");
                    generateAttDeductVoucher.Narration = $"Salary Deduct. {employee.Name},{employee.Designation.Name}";
                    generateAttDeductVoucher.VcNo = $"JV/{lastVcNo.ToString().PadLeft(6, '0')}/{salaryMonth.ToString("yy")}";
                    deductVoucherList.Add(generateAttDeductVoucher);
                    lastVcNo = lastVcNo + 1;
                }

                #endregion
            }

            #region DepartmentWiseSalaryVoucher

            //var departmentGroupData = employeeList.GroupBy(x => new { x.DepartmentId, x.Department.Code })
            //    .Select(c => new { c.Key.DepartmentId, DepartmentCode = c.Key.Code, Count = c.Count() }).ToList();

            //List<AccTranMst> departmentVoucherList = new List<AccTranMst>();

            //if (departmentGroupData.Count > 0)
            //{
            //    foreach (var department in departmentGroupData)
            //    {
            //        var dptEmpListIds = employeeList.Where(x => x.DepartmentId == department.DepartmentId).Select(c => c.Id).ToList();
            //        var salaryAmount = salaryDtls.Where(x => dptEmpListIds.Contains(x.EmployeeId)).Sum(x => x.NetSalary);

            //        var salaryVcModel = new PayrollAutoVoucherVm();
            //        salaryVcModel.PayrollId = prMst.Id;
            //        salaryVcModel.PaidDate = (DateTime)salaryDtls.FirstOrDefault().PaidDate;
            //        salaryVcModel.PaidAmount = salaryAmount;
            //        salaryVcModel.LedgerCode = _iDepartmentService.GetLedgerCodeByDptCode(department.DepartmentCode);

            //        var generateSalaryVoucher = await _iAutoVoucherService.GetSalaryPayQuickVoucher(salaryVcModel, CurrentUserId);
            //        if (generateSalaryVoucher == null)
            //            throw new Exception("Somthing Went Wrong Creating Loan Voucher..!!");
            //        generateSalaryVoucher.Narration = $"";

            //        departmentVoucherList.Add(generateSalaryVoucher);
            //    }
            //}

            #endregion
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (salaryDtls != null && salaryDtls.Count > 0)
        {
            await Repository.UpdateRangeAsync(salaryDtls);
        }

        if (arrearList != null && arrearList.Count > 0)
        {
            _iArrearDtlRepository.UpdateRange(arrearList);
        }

        if (updateRsFoodOrders.Count > 0 && rsFoodPayments.Count > 0)
        {
            _iFoodOrderRepository.UpdateRange(updateRsFoodOrders);
            _iOrderPaymentRepository.AddRange(rsFoodPayments);
        }

        if (foodVoucherList.Count > 0)
        {
            await _iAccTranMstRepository.AddRangeAsync(foodVoucherList);
        }

        if (instalmentList != null && instalmentList.Count > 0)
        {
            _iEmpLoanDtlRepository.UpdateRange(instalmentList);
        }

        if (fullLoanPaidList != null && fullLoanPaidList.Count > 0)
        {
            _iEmpLoanMstRepository.UpdateRange(fullLoanPaidList);
        }

        if (loanVoucherList.Count > 0)
        {
            await _iAccTranMstRepository.AddRangeAsync(loanVoucherList);
        }

        if (salaryVoucherList.Count > 0)
        {
            await _iAccTranMstRepository.AddRangeAsync(salaryVoucherList);
        }

        if (deductVoucherList.Count > 0)
        {
            await _iAccTranMstRepository.AddRangeAsync(deductVoucherList);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted)
        {
            return false;
        }

        ts.Complete();
        return true;
    }

    #endregion

    #region PaySlipHtml

    public async Task<string> PayslipHtml(long id)
    {
        if (id == 0) throw new Exception("Payslip Generate Failed..!!");
        var model = await GeneratePayslip(id);

        var empAdditionParts = model.PrSalaryParts.Where(c => c.PartType == "A" && c.PartLink != PrSalaryPartLink.GratuityA).ToList();
        var empDiductionParts = model.PrSalaryParts.Where(c => c.PartType == "D" && c.PartLink != PrSalaryPartLink.GratuityD).ToList();

        var fullHtml = "";
        fullHtml += "<table class='PayslipPrintTable' style='width:100%'>";
        fullHtml += "<tbody>";

        fullHtml += "<tr>";
        fullHtml += "<td style='width:20%'>Employee Name</td>";
        fullHtml += $"<td style='width:30%'>{model.EmployeeName}</td>";
        fullHtml += "<td style='width:20%'>Date Of Join</td>";
        fullHtml += $"<td style='width:30%'>{model.EmpJoinDate?.ToString("dd MMMM,yyyy")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += "<td style='width:20%'>Employee Code</td>";
        fullHtml += $"<td style='width:30%'>{model.EmployeeCode}</td>";
        fullHtml += "<td style='width:20%'>Worked Days</td>";
        fullHtml += $"<td style='width:30%'></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += "<td style='width:20%'>Employee Designation</td>";
        fullHtml += $"<td style='width:30%'>{model.EmpDesignationName}</td>";
        fullHtml += "<td colspan='2'></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += "<td style='width:20%'>Employee Department</td>";
        fullHtml += $"<td style='width:30%'>{model.EmpDepartmentName}</td>";
        fullHtml += "<td colspan='2'></td>";
        fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        fullHtml += $@"<table class='addition_print-table'>
                            <thead>
                                <tr>
                                    <th style='width:80%'> Earnings </th>
                                    <th style='width:20%'> Amount </th>
                                </tr>
                            </thead>";
        fullHtml += "<tbody>";

        if (empAdditionParts.Count > 0)
        {
            empAdditionParts = empAdditionParts.OrderBy(c => c.PartCode).ToList();

            foreach (var addItem in empAdditionParts)
            {
                fullHtml += "<tr>";
                fullHtml += $"<td>{addItem.PartName}</td>";
                if (addItem.PartCode == "BASIC")
                {
                    fullHtml += $"<td>{model.BasicSalary}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColA))
                {
                    fullHtml += $"<td>{model.ColA}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColB))
                {
                    fullHtml += $"<td>{model.ColB}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColC))
                {
                    fullHtml += $"<td>{model.ColC}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColD))
                {
                    fullHtml += $"<td>{model.ColD}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColE))
                {
                    fullHtml += $"<td>{model.ColE}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColF))
                {
                    fullHtml += $"<td>{model.ColF}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColG))
                {
                    fullHtml += $"<td>{model.ColG}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColH))
                {
                    fullHtml += $"<td>{model.ColH}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColI))
                {
                    fullHtml += $"<td>{model.ColI}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColK))
                {
                    fullHtml += $"<td>{model.ColK}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColL))
                {
                    fullHtml += $"<td>{model.ColL}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColM))
                {
                    fullHtml += $"<td>{model.ColM}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColN))
                {
                    fullHtml += $"<td>{model.ColN}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColO))
                {
                    fullHtml += $"<td>{model.ColO}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColP))
                {
                    fullHtml += $"<td>{model.ColP}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColQ))
                {
                    fullHtml += $"<td>{model.ColQ}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColR))
                {
                    fullHtml += $"<td>{model.ColR}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColS))
                {
                    fullHtml += $"<td>{model.ColS}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColT))
                {
                    fullHtml += $"<td>{model.ColT}</td>";
                }
                else if (addItem.PartCode == nameof(model.ColU))
                {
                    fullHtml += $"<td>{model.ColU}</td>";
                }

                fullHtml += "</tr>";
            }

            fullHtml += "<tr style='font-weight: bold;background-color: aliceblue;'>";
            fullHtml += "<td>Gross Pay</td>";
            fullHtml += $"<td>{model.GrossSalary}</td>";
            fullHtml += "</tr>";
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        fullHtml += $@"<table class='deduction_print-table'>
                            <thead>
                                <tr>
                                    <th style='width:80%'> Deductions </th>
                                    <th style='width:20%'> Amount </th>
                                </tr>
                            </thead>";
        fullHtml += "<tbody>";

        if (empDiductionParts.Count > 0)
        {
            empDiductionParts = empDiductionParts.OrderBy(c => c.PartCode).ToList();

            foreach (var deductItem in empDiductionParts)
            {
                fullHtml += "<tr>";
                fullHtml += $"<td>{deductItem.PartName}</td>";
                if (deductItem.PartCode == "BASIC")
                {
                    fullHtml += $"<td>{model.BasicSalary}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColA))
                {
                    fullHtml += $"<td>{model.ColA}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColB))
                {
                    fullHtml += $"<td>{model.ColB}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColC))
                {
                    fullHtml += $"<td>{model.ColC}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColD))
                {
                    fullHtml += $"<td>{model.ColD}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColE))
                {
                    fullHtml += $"<td>{model.ColE}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColF))
                {
                    fullHtml += $"<td>{model.ColF}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColG))
                {
                    fullHtml += $"<td>{model.ColG}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColH))
                {
                    fullHtml += $"<td>{model.ColH}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColJ))
                {
                    fullHtml += $"<td>{model.ColJ}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColK))
                {
                    fullHtml += $"<td>{model.ColK}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColL))
                {
                    fullHtml += $"<td>{model.ColL}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColM))
                {
                    fullHtml += $"<td>{model.ColM}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColN))
                {
                    fullHtml += $"<td>{model.ColN}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColO))
                {
                    fullHtml += $"<td>{model.ColO}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColP))
                {
                    fullHtml += $"<td>{model.ColP}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColQ))
                {
                    fullHtml += $"<td>{model.ColQ}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColR))
                {
                    fullHtml += $"<td>{model.ColR}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColS))
                {
                    fullHtml += $"<td>{model.ColS}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColT))
                {
                    fullHtml += $"<td>{model.ColT}</td>";
                }
                else if (deductItem.PartCode == nameof(model.ColU))
                {
                    fullHtml += $"<td>{model.ColU}</td>";
                }

                fullHtml += "</tr>";
            }

            fullHtml += "<tr style='font-weight: bold;background-color: aliceblue;'>";
            fullHtml += "<td>Total Deduction</td>";
            fullHtml += $"<td>{model.TotalDeduction}</td>";
            fullHtml += "</tr>";
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        fullHtml += @$"<table class='netpay_print-table'>
                            <tbody>
                                <tr>
                                    <td style='width:80%'>NET PAY</td>
                                    <td style='width:20%'>{model.NetSalary}</td>
                                </tr>
                            </tbody>
                        </table>";

        return fullHtml;
    }

    #endregion
}
