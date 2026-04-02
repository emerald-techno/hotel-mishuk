using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Entities.Pf;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.EmpLoan;
using Interface.Repository.Accounts;
using Interface.Repository.Hr;
using Interface.Repository.Pf;
using Interface.Services.Accounts;
using Interface.Services.Admin;
using Interface.Services.Pf;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.Pf;

public class EmpLoanMstService : BaseService<EmpLoanMst>, IEmpLoanMstService
{
    #region Config

    private IEmpLoanMstRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IEmpLoanDtlService _iEmpLoanDtlService;
    private readonly IEmployeeRepository _iEmployeeRepository;
    private readonly IAccTranMstRepository _iAccTranMstRepository;

    private readonly IAutoVoucherService _iAutoVoucherService;
    private readonly IDepartmentService _iDepartmentService;

    public EmpLoanMstService(IEmpLoanMstRepository iRepository,
        IMapper iMapper, IUnitOfWork iUnitOfWork, IEmpLoanDtlService iEmpLoanDtlService,
        IEmployeeRepository iEmployeeRepository,
        IAutoVoucherService iAutoVoucherService,
        IDepartmentService iDepartmentService,
        IAccTranMstRepository iAccTranMstRepository) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iEmpLoanDtlService = iEmpLoanDtlService;
        _iEmployeeRepository = iEmployeeRepository;
        _iAutoVoucherService = iAutoVoucherService;
        _iDepartmentService = iDepartmentService;
        _iAccTranMstRepository = iAccTranMstRepository;
    }

    #endregion

    #region AddAsync

    public async Task<bool> AddAsync(EmpLoanMstVm vm)
    {
        if (vm == null && !(vm.EmployeeId > 0))
            throw new Exception("Loan Information Is Not Correct..!!");

        if (vm.EmpLoanDtls.Count > 0)
        {
            foreach (var dtl in vm.EmpLoanDtls)
            {
                dtl.InsDate = (DateTime)(!string.IsNullOrEmpty(dtl.InsDateStr) ? DU.Utility.ConvertStrToDate(dtl.InsDateStr) : dtl.InsDate);
            }
        }

        var loanModel = _iMapper.Map<EmpLoanMst>(vm);

        var employeeInfo = await _iEmployeeRepository.GetFirstOrDefaultAsync(x => x.Id == loanModel.EmployeeId && !x.IsDeleted, d => d.Department, ds => ds.Designation);
        if (employeeInfo == null)
            throw new Exception("Employee Information Not Found..!!");
        if (!employeeInfo.IsEnable)
            throw new Exception("Employee Is Not Available...!!");

        var ledgerCode = _iDepartmentService.GetLedgerCodeByDptCode(employeeInfo.Department.Code);

        loanModel.LoanPassDate = (DateTime)(!string.IsNullOrEmpty(vm.LoanPassDateStr) ? DU.Utility.ConvertStrToDate(vm.LoanPassDateStr) : vm.LoanPassDate);
        loanModel.LoanPayDate = (DateTime)(!string.IsNullOrEmpty(vm.LoanPayDateStr) ? DU.Utility.ConvertStrToDate(vm.LoanPayDateStr) : vm.LoanPayDate);
        loanModel.FirstInsDate = (DateTime)(!string.IsNullOrEmpty(vm.FirstInsDateStr) ? DU.Utility.ConvertStrToDate(vm.FirstInsDateStr) : vm.FirstInsDate);
        loanModel.ActionById = CurrentUserId;
        loanModel.ActionDate = DU.Utility.GetBdDateTimeNow();

        AccTranMst generateVoucher = null;

        if (AppUtility.IsAutoVoucherLoan)
        {
            generateVoucher = await _iAutoVoucherService.GetAdvanceGivenQuickVoucher(loanModel, CurrentUserId, ledgerCode);
            if (generateVoucher == null)
                throw new Exception("Somthing Went Wrong Creating Voucher..!!");
            generateVoucher.Narration = $"Advance Made To Stuffs. {employeeInfo.Name},{employeeInfo.Designation.Name}";
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await Repository.AddAsync(loanModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();

        if (generateVoucher != null)
        {
            generateVoucher.EmpLoanMstId = loanModel.Id;
            await _iAccTranMstRepository.AddAsync(generateVoucher);
            var isVoucherAdded = await _iUnitOfWork.CompleteAsync();
            if (!isAdded || !isVoucherAdded)
                return false;
        }

        if (!isAdded) return false;
        ts.Complete();
        return true;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<EmpLoanMstSearchVm, EmpLoanMstSearchVm>> SearchAsync(DataTablePagination<EmpLoanMstSearchVm, EmpLoanMstSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region AdvanceLoanCalculation

    public async Task<bool> AdvanceLoanCalculation(long id, double advAmount)
    {
        var data = await Repository.GetFirstOrDefaultAsync(c => c.Id == id, e => e.Employee, c => c.EmpLoanDtls);

        if (data == null) throw new Exception("No Loan Found");
        if (data.EmpLoanDtls == null && !(data.EmpLoanDtls.Count > 0)) throw new Exception("No Loan Instalment Found");

        var isAdded = false;
        var isUpdated = false;
        var isDeleted = false;

        var addableList = new List<EmpLoanDtl>();

        var unpaidInstList = data.EmpLoanDtls.Where(c => !c.IsPaid && !c.IsDeleted).OrderBy(s => s.Serial).ToList();

        var currentInst = unpaidInstList.FirstOrDefault(); // this instalment will update for advance amount

        //var previosSerial = currentInst.Serial > 1 ? currentInst.Serial - 1 : currentInst.Serial;
        //var previousInst = data.EmpLoanDtls.FirstOrDefault(c => c.Serial == previosSerial && !c.IsDeleted);
        //var loanAmount = currentInst.Serial > 1 ? previousInst.LoanAmount : ((currentInst.InsAmount - currentInst.InterestAmount) + currentInst.LoanAmount);

        var dueAmount = currentInst.InsAmount + currentInst.LoanAmount; // Current Loan Due Amount

        if (advAmount > dueAmount) throw new Exception("Advance Amount Is Higher Than Due Amount");

        if (advAmount <= dueAmount)
        {
            var newLoanAmount = dueAmount - advAmount; // Remain Amount after giving advance amount

            currentInst.InsAmount = advAmount;
            currentInst.LoanAmount = newLoanAmount;
            currentInst.IsPaid = true;

            var newInstValue = Math.Ceiling(newLoanAmount / data.InsAmount); // Total Instalment Count After Advance Payment

            if (newInstValue > 0)
            {
                var amount = newLoanAmount;
                var newSerial = currentInst.Serial + 1;

                for (int i = 0; i < newInstValue; i++)
                {
                    var dtlModel = new EmpLoanDtl();

                    dtlModel.LoanId = data.Id;
                    dtlModel.InsDate = currentInst.InsDate.AddMonths(i + 1);

                    double interest = 0.0;

                    if (i == 0)
                    {
                        interest = Math.Ceiling(Utility.PercentCalculation(data.InterestRate, newLoanAmount) / 12);
                    }
                    else
                    {
                        interest = Math.Ceiling(Utility.PercentCalculation(data.InterestRate, addableList[i - 1].LoanAmount) / 12);
                    }

                    dtlModel.InterestAmount = interest;

                    if (amount >= data.InsAmount)
                    {
                        dtlModel.InsAmount = data.InsAmount + interest;
                        amount = amount - data.InsAmount;
                    }
                    else
                    {
                        dtlModel.InsAmount = amount + interest;
                        amount = amount - amount;
                    }

                    dtlModel.LoanAmount = amount;
                    dtlModel.Serial = (short)newSerial;

                    addableList.Add(dtlModel);

                    newSerial++;
                }
            }
        }

        /* Delete All Loan Instalment Without Advance Payment (Current) Instalment */

        var deleteInstList = unpaidInstList.Where(c => c.Serial != currentInst.Serial).ToList();


        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iEmpLoanDtlService.UpdateAsync(currentInst);
        isUpdated = await _iUnitOfWork.CompleteAsync();

        _iEmpLoanDtlService.RemoveRange(deleteInstList);
        isDeleted = await _iUnitOfWork.CompleteAsync();

        await _iEmpLoanDtlService.AddRangeAsync(addableList);
        isAdded = await _iUnitOfWork.CompleteAsync();

        if (!isUpdated && !isDeleted && !isAdded)
        {
            return false;
        }

        ts.Complete();
        return true;
    }

    #endregion

    #region LoanPaid

    public async Task<bool> LoanPaid(InstalmentPaidVm vm)
    {
        var data = await _iEmpLoanDtlService.GetFirstOrDefaultAsync(x => x.Id == vm.Id && !x.IsDeleted, m => m.Loan);
        if (data == null)
            throw new Exception("No Data Found !!");

        var employeeInfo = await _iEmployeeRepository.GetFirstOrDefaultAsync(x => x.Id == data.Loan.EmployeeId && !x.IsDeleted, d => d.Department, ds => ds.Designation);
        if (employeeInfo == null)
            throw new Exception("Employee Information Not Found..!!");
        if (!employeeInfo.IsEnable)
            throw new Exception("Employee Is Not Available...!!");

        var ledgerCode = _iDepartmentService.GetLedgerCodeByDptCode(employeeInfo.Department.Code);

        data.IsPaid = true;
        data.PaidDate = (DateTime)(!string.IsNullOrEmpty(vm.PaidDateStr) ? DU.Utility.ConvertStrToDate(vm.PaidDateStr) : DateTime.Now);
        data.PaidSetById = CurrentUserId;
        data.WaiverAmount = vm.WaiverAmount;
        data.Remarks = !string.IsNullOrEmpty(vm.Remarks) ? vm.Remarks : $"Load Paid.Waiver Amount: {data.WaiverAmount.ToString("N2")}";
        data.PaidAmount = vm.WaiverAmount > 0 ? (data.InsAmount - data.WaiverAmount) : data.InsAmount;

        AccTranMst generateVoucher = null;

        if (AppUtility.IsAutoVoucherLoan)
        {
            generateVoucher = await _iAutoVoucherService.GetAdvanceReceivedQuickVoucher(data, CurrentUserId, ledgerCode);
            if (generateVoucher == null)
                throw new Exception("Somthing Went Wrong Creating Voucher..!!");
            generateVoucher.Narration = $"Advance Recovered Staffs. {employeeInfo.Name},{employeeInfo.Designation.Name}";
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iEmpLoanDtlService.UpdateAsync(data);

        if (generateVoucher != null)
        {
            generateVoucher.EmpLoanDtlId = data.Id;
            await _iAccTranMstRepository.AddAsync(generateVoucher);
        }

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) return false;
        ts.Complete();
        return true;
    }

    #endregion

    #region EmpLoanDetailsReportHtml

    public async Task<string> EmpLoanDetailsReportHtml(long id)
    {
        var data = await Repository.GetFirstOrDefaultAsync(c => c.Id == id, e => e.Employee, c => c.EmpLoanDtls);
        var model = _iMapper.Map<EmpLoanMstVm>(data);

        var fullHtml = "";
        fullHtml += "<br/>";
        fullHtml += "<br/>";
        fullHtml += "<br/>";

        fullHtml += "<div style='padding-bottom: 10px;'>";
        fullHtml += $@"<table>
                                <tbody>
                                    <tr>
                                        <td style='width:20%; font-size:10px'><b>Employee</b></td>
                                        <td style='width:20%; font-size:12px'>{model.EmployeeName}</td>
                                        <td style='width:20%; font-size:10px'><b>Loan Pass Date</b></td>
                                        <td style='width:20%; font-size:12px'>{@Utility.ConvertDateToStr(model.LoanPassDate)}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:20%; font-size:10px'><b>Loan Pay Date</b></td>
                                        <td style='width:20%; font-size:12px'>{@Utility.ConvertDateToStr(model.LoanPayDate)}</td>
                                        <td style='width:20%; font-size:10px'><b>Loan Amount</b></td>
                                        <td style='width:20%; font-size:12px'>{model.LoanAmount}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:20%; font-size:10px'><b>Interest Rate (%)</b></td>
                                        <td style='width:20%; font-size:12px'>{model.InterestRate} </td>
                                        <td style='width:20%; font-size:10px'><b>Instalment Amount</b></td>
                                        <td style='width:20%; font-size:12px'>{model.InsAmount} </td>
                                    </tr>
                                    <tr>
                                        <td style='width:20%; font-size:10px'><b>First Instalment Date</b></td>
                                        <td style='width:20%; font-size:12px'>{@Utility.ConvertDateToStr(model.FirstInsDate)}</td>
                                        <td style='width:20%; font-size:10px'><b>Remarks</b></td>
                                        <td style='width:20%; font-size:12px'>{model.Remarks}</td>
                                    </tr>
                                </tbody>
                            </table>";

        fullHtml += "</div>";

        fullHtml += "<table class='table' style='width:100%;text-align: center'>";
        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th style='width:4%'>Sl.</th>";
        fullHtml += "<th style='width:16%'>Instalment Date</th>";
        fullHtml += "<th style='width:16%'>Instalment</th>";
        fullHtml += "<th style='width:16%'>Interest</th>";
        fullHtml += "<th style='width:16%'>Net Amount</th>";
        fullHtml += "<th style='width:16%'>Loan Due Amount</th>";
        fullHtml += "<th style='width:16%'>Payment Status</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        if (model.EmpLoanDtls.Count > 0)
        {
            foreach (var (v, i) in model.EmpLoanDtls.GetItemWithIndex())
            {
                var capitalInstalment = model.LoanAmount / model.EmpLoanDtls.Count();

                fullHtml += "<tr>";
                fullHtml += $@"<td>{i + 1}</td>";
                fullHtml += $@"<td style='width:4%' class='text-center'><b>{@Utility.ConvertDateToStr(v.InsDate)}</b></td>";
                fullHtml += $@"<td style='width:16%' class='text-center'>{v.InsAmount}</td>";
                fullHtml += $@"<td style='width:16%' class='text-center'>{v.InterestAmount}</td>";
                fullHtml += $@"<td style='width:16%' class='text-center'>{v.InsAmount + v.InterestAmount}</td>";
                fullHtml += $@"<td style='width:16%' class='text-center'>{v.LoanAmount - v.InterestAmount}</td>";
                fullHtml += $@"<td style='width:16%' class='text-center'>{(v.IsPaid ? "Paid" : "Not Paid")}</td>";
                fullHtml += "</tr>";
            }
        }
        else
        {
            fullHtml += "<tr>";
            fullHtml += "<td style='text-align:center' colspan='7'><b>Employee Provident Fund Not Found</b></td>";
            fullHtml += "</tr>";
        }

        fullHtml += "";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        return fullHtml;
    }

    #endregion

    #region EmployeeLoanReportHtml
    public async Task<string> EmployeeLoanReportHtml(EmployeeLoanReportVm vm)
    {
        string fullHtml = await Repository.EmployeeLoanReportHtml(vm);
        return fullHtml;
    }
    #endregion
}
